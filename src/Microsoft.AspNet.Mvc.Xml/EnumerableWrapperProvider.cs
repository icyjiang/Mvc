// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc.Xml
{
    public class EnumerableWrapperProvider : IWrapperProvider
    {
        private readonly WrapperProviderContext _context;
        private readonly Type _sourceEnumerableOfT;
        private readonly IEnumerable<IWrapperProviderFactory> _wrapperProviderFactories;

        public EnumerableWrapperProvider(
            [NotNull] Type sourceEnumerableOfT,
            [NotNull] IEnumerable<IWrapperProviderFactory> wrapperProviderFactories,
            [NotNull] WrapperProviderContext context)
        {
            _sourceEnumerableOfT = sourceEnumerableOfT;
            _wrapperProviderFactories = wrapperProviderFactories;
            _context = context;
        }

        /// <inheritdoc />
        public Type WrappingType
        {
            get
            {
                IWrapperProvider elementWrapperProvider = null;
                return GetWrappingEnumerableOfT(out elementWrapperProvider);
            }
        }

        /// <inheritdoc />
        public object Wrap(object original)
        {
            if (original == null)
            {
                return null;
            }

            IWrapperProvider elementWrapperProvider;
            var wrappingEnumerableType = GetWrappingEnumerableOfT(out elementWrapperProvider);

            var wrappingEnumerableTypeConstructor = wrappingEnumerableType.GetConstructor(new[]
                                                                                        {
                                                                                            _sourceEnumerableOfT,
                                                                                            typeof(IWrapperProvider)
                                                                                        });

            return wrappingEnumerableTypeConstructor.Invoke(new object[] { original, elementWrapperProvider });
        }

        private Type GetWrappingEnumerableOfT(out IWrapperProvider elementWrapperProvider)
        {
            var declaredElementType = _sourceEnumerableOfT.GetGenericArguments()[0];

            // Since the T itself could be wrapped, get the wrapping type for it
            var wrapperProviderContext = new WrapperProviderContext(declaredElementType, _context.IsSerialization);

            var wrappedElementType = declaredElementType;

            elementWrapperProvider = FormattingUtilities.GetWrapperProvider(_wrapperProviderFactories, wrapperProviderContext);
            if (elementWrapperProvider != null && elementWrapperProvider.WrappingType != null)
            {
                wrappedElementType = elementWrapperProvider.WrappingType;
            }

            return typeof(DelegatingEnumerable<,>).MakeGenericType(wrappedElementType, declaredElementType);
        }
    }
}