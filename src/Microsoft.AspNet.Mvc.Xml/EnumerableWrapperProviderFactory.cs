// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Creates an <see cref="EnumerableWrapperProvider"/> for interface types implementing the 
    /// <see cref="IEnumerable{T}"/> type.
    /// </summary>
    public class EnumerableWrapperProviderFactory : IWrapperProviderFactory
    {
        private readonly IEnumerable<IWrapperProviderFactory> _wrapperProviderFactories;

        /// <summary>
        /// Initializes an <see cref="EnumerableWrapperProviderFactory"/> with the  
        /// </summary>
        /// <param name="wrapperProviderFactories"></param>
        public EnumerableWrapperProviderFactory(IEnumerable<IWrapperProviderFactory> wrapperProviderFactories)
        {
            _wrapperProviderFactories = wrapperProviderFactories;
        }

        /// <summary>
        /// Gets an <see cref="EnumerableWrapperProvider"/> for the provided context.
        /// </summary>
        /// <param name="context">The <see cref="WrapperProviderContext"/>.</param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// </remarks>
        public IWrapperProvider GetProvider([NotNull] WrapperProviderContext context)
        {
            if (context.IsSerialization)
            {
                var declaredType = context.DeclaredType;

                // We only for types which are interfaces (ex: IEnumerable<>, IQueryable<> etc.) and not
                // concrete types like List<T>, Collection<T> which implement IEnumerable<T>.
                if (declaredType != null && declaredType.IsInterface() && declaredType.IsGenericType())
                {
                    var enumerableOfT = declaredType.ExtractGenericInterface(typeof(IEnumerable<>));
                    if (enumerableOfT != null)
                    {
                        return new EnumerableWrapperProvider(
                                                        enumerableOfT,
                                                        _wrapperProviderFactories,
                                                        context);
                    }
                }
            }

            return null;
        }
    }
}