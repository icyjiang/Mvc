// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Provides a <see cref="IWrapperProvider"/> for interface types which implement 
    /// <see cref="IEnumerable{T}"/>.
    /// </summary>
    public class EnumerableWrapperProvider : IWrapperProvider
    {
        private readonly IWrapperProvider _wrapperProvider;
        private readonly ConstructorInfo _wrappingTypeConstructor;

        /// <summary>
        /// Initializes an instance of <see cref="EnumerableWrapperProvider"/>.
        /// </summary>
        /// <param name="sourceEnumerableOfT">Type of the original <see cref="IEnumerable{T}" /> 
        /// that is being wrapped.</param>
        /// <param name="elementWrapperProvider">The <see cref="IWrapperProvider"/> for the element type.
        /// Can be null.</param>
        public EnumerableWrapperProvider(
            [NotNull] Type sourceEnumerableOfT,
            IWrapperProvider elementWrapperProvider)
        {
            Type declaredElementType;
            if (GetIEnumerableOfT(sourceEnumerableOfT, out declaredElementType) == null)
            {
                throw new ArgumentException(
                    Resources.FormatEnumerableWrapperProvider_InvalidSourceEnumerableOfT(nameof(sourceEnumerableOfT)));
            }

            _wrapperProvider = elementWrapperProvider;

            var wrappedElementType = elementWrapperProvider?.WrappingType ?? declaredElementType;

            WrappingType = typeof(DelegatingEnumerable<,>).MakeGenericType(wrappedElementType, declaredElementType);

            _wrappingTypeConstructor = WrappingType.GetConstructor(new[] {
                                                            sourceEnumerableOfT,
                                                            typeof(IWrapperProvider) });
        }

        /// <inheritdoc />
        public Type WrappingType
        {
            get;
        }

        /// <inheritdoc />
        public object Wrap(object original)
        {
            if (original == null)
            {
                return null;
            }

            return _wrappingTypeConstructor.Invoke(new object[] { original, _wrapperProvider });
        }

        public static Type GetIEnumerableOfT(Type declaredType, out Type elementType)
        {
            elementType = null;
            if (declaredType != null && declaredType.IsInterface() && declaredType.IsGenericType())
            {
                var enumerableOfT = declaredType.ExtractGenericInterface(typeof(IEnumerable<>));
                if(enumerableOfT != null)
                {
                    elementType = enumerableOfT.GetGenericArguments()[0];
                    return enumerableOfT;
                }
            }

            return null;
        }
    }
}