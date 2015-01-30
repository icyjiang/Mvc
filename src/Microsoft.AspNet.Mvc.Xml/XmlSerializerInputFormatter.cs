﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// This class handles deserialization of input XML data
    /// to strongly-typed objects using <see cref="XmlSerializer"/>
    /// </summary>
    public class XmlSerializerInputFormatter : IInputFormatter
    {
        private readonly XmlDictionaryReaderQuotas _readerQuotas = FormattingUtilities.GetDefaultXmlReaderQuotas();
        private IList<IWrapperProviderFactory> _wrapperProviderFactories;

        /// <summary>
        /// Initializes a new instance of XmlSerializerInputFormatter.
        /// </summary>
        public XmlSerializerInputFormatter()
        {
            SupportedEncodings = new List<Encoding>();
            SupportedEncodings.Add(Encodings.UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(Encodings.UTF16EncodingLittleEndian);

            SupportedMediaTypes = new List<MediaTypeHeaderValue>();
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/xml"));

            WrapperProviderFactories = new List<IWrapperProviderFactory>();
            WrapperProviderFactories.Add(new EnumerableWrapperProviderFactory(WrapperProviderFactories));
            WrapperProviderFactories.Add(new SerializableErrorWrapperProviderFactory());
        }

        /// <summary>
        /// Gets or sets the list of <see cref="IWrapperProviderFactory"/> to
        /// provide the wrapping type for de-serialization.
        /// </summary>
        public IList<IWrapperProviderFactory> WrapperProviderFactories
        {
            get
            {
                return _wrapperProviderFactories;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _wrapperProviderFactories = value;
            }
        }

        /// <inheritdoc />
        public IList<MediaTypeHeaderValue> SupportedMediaTypes { get; private set; }

        /// <inheritdoc />
        public IList<Encoding> SupportedEncodings { get; private set; }

        /// <summary>
        /// Indicates the acceptable input XML depth.
        /// </summary>
        public int MaxDepth
        {
            get { return _readerQuotas.MaxDepth; }
            set { _readerQuotas.MaxDepth = value; }
        }

        /// <summary>
        /// The quotas include - DefaultMaxDepth, DefaultMaxStringContentLength, DefaultMaxArrayLength,
        /// DefaultMaxBytesPerRead, DefaultMaxNameTableCharCount
        /// </summary>
        public XmlDictionaryReaderQuotas XmlDictionaryReaderQuotas
        {
            get { return _readerQuotas; }
        }

        /// <inheritdoc />
        public bool CanRead(InputFormatterContext context)
        {
            var contentType = context.ActionContext.HttpContext.Request.ContentType;
            MediaTypeHeaderValue requestContentType;
            if (!MediaTypeHeaderValue.TryParse(contentType, out requestContentType))
            {
                return false;
            }

            return SupportedMediaTypes
                .Any(supportedMediaType => supportedMediaType.IsSubsetOf(requestContentType));
        }

        /// <summary>
        /// Reads the input XML.
        /// </summary>
        /// <param name="context">The input formatter context which contains the body to be read.</param>
        /// <returns>Task which reads the input.</returns>
        public async Task<object> ReadAsync(InputFormatterContext context)
        {
            var request = context.ActionContext.HttpContext.Request;
            if (request.ContentLength == 0)
            {
                return GetDefaultValueForType(context.ModelType);
            }

            return await ReadInternalAsync(context);
        }

        /// <summary>
        /// Gets the type to which the XML will be deserialized.
        /// </summary>
        /// <param name="declaredType">The declared type.</param>
        /// <returns>The type to which the XML will be deserialized.</returns>
        protected virtual Type GetSerializableType([NotNull] Type declaredType)
        {
            IWrapperProvider wrapperProvider = FormattingUtilities.GetWrapperProvider(
                                                    _wrapperProviderFactories,
                                                    new WrapperProviderContext(declaredType, isSerialization: false));

            if (wrapperProvider != null && wrapperProvider.WrappingType != null)
            {
                return wrapperProvider.WrappingType;
            }

            return declaredType;
        }

        /// <summary>
        /// Called during deserialization to get the <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
        /// <returns>The <see cref="XmlReader"/> used during deserialization.</returns>
        protected virtual XmlReader CreateXmlReader([NotNull] Stream readStream)
        {
            return XmlDictionaryReader.CreateTextReader(
                readStream, _readerQuotas);
        }

        /// <summary>
        /// Called during deserialization to get the <see cref="XmlSerializer"/>.
        /// </summary>
        /// <returns>The <see cref="XmlSerializer"/> used during deserialization.</returns>
        protected virtual XmlSerializer CreateSerializer(Type type)
        {
            return new XmlSerializer(type);
        }

        private object GetDefaultValueForType(Type modelType)
        {
            if (modelType.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(modelType);
            }

            return null;
        }

        private Task<object> ReadInternalAsync(InputFormatterContext context)
        {
            var request = context.ActionContext.HttpContext.Request;

            using (var xmlReader = CreateXmlReader(new DelegatingStream(request.Body)))
            {
                var type = GetSerializableType(context.ModelType);

                var serializer = CreateSerializer(type);

                var deserializedObject = serializer.Deserialize(xmlReader);

                // Unwrap only if the original type was wrapped.
                if (type != context.ModelType)
                {
                    IUnwrappable unwrappable = deserializedObject as IUnwrappable;
                    if (unwrappable != null)
                    {
                        deserializedObject = unwrappable.Unwrap(declaredType: context.ModelType);
                    }
                }

                return Task.FromResult(deserializedObject);
            }
        }
    }
}