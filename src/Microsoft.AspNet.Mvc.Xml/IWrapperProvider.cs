// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Defines an interface for wrapping objects for serialization or deserialization into xml.
    /// </summary>
    public interface IWrapperProvider
    {
        /// <summary>
        /// Gets the type which wraps the given type.
        /// </summary>
        /// <param name="declaredType">The declared type which needs to be wrapped.</param>
        /// <returns>The wrapping type if the provider decides to wrap the given type, else null.</returns>
        Type WrappingType { get; }

        /// <summary>
        /// Wraps the given object to the wrapping type provided by <see cref="WrappingType"/>.
        /// </summary>
        /// <param name="original">The original non-wrapped object</param>
        /// <returns></returns>
        object Wrap(object original);
    }
}