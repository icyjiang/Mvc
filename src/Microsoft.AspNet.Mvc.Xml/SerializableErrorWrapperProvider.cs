// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Wraps the object of type <see cref="Microsoft.AspNet.Mvc.SerializableError"/>.
    /// </summary>
    public class SerializableErrorWrapperProvider : IWrapperProvider
    {
        private readonly WrapperProviderContext _context;

        /// <summary>
        /// Initializes a <see cref="SerializableErrorWrapperProvider"/> with 
        /// the <see cref="WrapperProviderContext"/>
        /// </summary>
        /// <param name="context">The <see cref="WrapperProviderContext"/></param>
        public SerializableErrorWrapperProvider([NotNull] WrapperProviderContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Type WrappingType
        {
            get
            {
                return typeof(SerializableErrorWrapper);
            }
        }

        /// <inheritdoc />
        public object Wrap(object original)
        {
            var error = original as SerializableError;
            if (error == null)
            {
                return original;
            }

            return new SerializableErrorWrapper(error);
        }
    }
}