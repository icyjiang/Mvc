// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
#if ASPNET50
using System.Runtime.Serialization;
#endif
using System.Xml;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Contains methods which are used by Xml input formatters.
    /// </summary>
    public static class FormattingUtilities
    {
        public static readonly int DefaultMaxDepth = 32;

#if ASPNET50
        public static readonly XsdDataContractExporter XsdDataContractExporter = new XsdDataContractExporter();
#endif

        /// <summary>
        /// Gets the default Reader Quotas for XmlReader.
        /// </summary>
        /// <returns>XmlReaderQuotas with default values</returns>
        public static XmlDictionaryReaderQuotas GetDefaultXmlReaderQuotas()
        {
            return new XmlDictionaryReaderQuotas()
            {
                MaxArrayLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue,
                MaxDepth = DefaultMaxDepth,
                MaxNameTableCharCount = int.MaxValue,
                MaxStringContentLength = int.MaxValue
            };
        }

        /// <summary>
        /// Gets the default XmlWriterSettings.
        /// </summary>
        /// <returns>Default <see cref="XmlWriterSettings"/></returns>
        public static XmlWriterSettings GetDefaultXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                CloseOutput = false,
                CheckCharacters = false
            };
        }

        /// <summary>
        /// Gets an instance of <see cref="IWrapperProvider"/> for the supplied
        /// type.
        /// </summary>
        /// <param name="wrapperProviderFactories">A list of <see cref="IWrapperProviderFactory"/>.</param>
        /// <param name="wrapperProviderContext">The <see cref="WrapperProviderContext"/>.</param>
        /// <returns>An instance of <see cref="IWrapperProvider"/> if there is a wrapping provider for the
        /// supplied type, else null.</returns>
        public static IWrapperProvider GetWrapperProvider(
            [NotNull] IEnumerable<IWrapperProviderFactory> wrapperProviderFactories,
            [NotNull] WrapperProviderContext wrapperProviderContext)
        {
            foreach (var wrapperProviderFactory in wrapperProviderFactories)
            {
                var wrapperProvider = wrapperProviderFactory.GetProvider(wrapperProviderContext);
                if (wrapperProvider != null)
                {
                    return wrapperProvider;
                }
            }

            return null;
        }
    }
}
