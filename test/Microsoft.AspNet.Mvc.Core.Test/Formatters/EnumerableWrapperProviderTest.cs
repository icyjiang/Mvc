// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNet.Mvc;
using Xunit;

namespace Microsoft.AspNet.Mvc.Xml
{
    public class EnumerableWrapperProviderTest
    {
        [Theory]
        [InlineData(typeof(IEnumerable<SerializableError>),
            typeof(DelegatingEnumerable<SerializableErrorWrapper, SerializableError>))]
        //[InlineData(typeof(IQueryable<SerializableError>),
        //    typeof(DelegatingEnumerable<SerializableErrorWrapper, SerializableError>))]
        [InlineData(typeof(ICollection<SerializableError>),
            typeof(DelegatingEnumerable<SerializableErrorWrapper, SerializableError>))]
        [InlineData(typeof(IList<SerializableError>),
            typeof(DelegatingEnumerable<SerializableErrorWrapper, SerializableError>))]
        public void Gets_DelegatingWrappingType(Type declaredEnumerableOfT, Type expectedType)
        {
            // Arrange
            var wrapperProviderFactories = GetWrapperProviderFactories();
            var wrapperProvider = new EnumerableWrapperProvider(
                                        declaredEnumerableOfT,
                                        wrapperProviderFactories,
                                        new WrapperProviderContext(declaredEnumerableOfT, isSerialization: true));

            // Act
            var wrappingType = wrapperProvider.WrappingType;

            // Assert
            Assert.NotNull(wrappingType);
            Assert.Equal(expectedType, wrappingType);
        }
        
        [Fact]
        public void Wraps_EmptyCollections()
        {
            // Arrange
            var declaredEnumerableOfT = typeof(IEnumerable<int>);
            var wrapperProviderFactories = GetWrapperProviderFactories();
            var wrapperProvider = new EnumerableWrapperProvider(
                                        declaredEnumerableOfT,
                                        wrapperProviderFactories,
                                        new WrapperProviderContext(declaredEnumerableOfT, isSerialization: true));

            // Act
            var wrapped = wrapperProvider.Wrap(new int[] { });

            // Assert
            Assert.Equal(typeof(DelegatingEnumerable<int, int>), wrapperProvider.WrappingType);
            Assert.NotNull(wrapped);
            var delegatingEnumerable = wrapped as DelegatingEnumerable<int, int>;
            Assert.NotNull(delegatingEnumerable);
            Assert.Equal(0, delegatingEnumerable.Count());
        }

        [Fact]
        public void Ignores_NullInstances()
        {
            // Arrange
            var declaredEnumerableOfT = typeof(IEnumerable<int>);
            var wrapperProviderFactories = GetWrapperProviderFactories();
            var wrapperProvider = new EnumerableWrapperProvider(
                                        declaredEnumerableOfT,
                                        wrapperProviderFactories,
                                        new WrapperProviderContext(declaredEnumerableOfT, isSerialization: true));

            // Act
            var wrapped = wrapperProvider.Wrap(null);

            // Assert
            Assert.Equal(typeof(DelegatingEnumerable<int, int>), wrapperProvider.WrappingType);
            Assert.Null(wrapped);
        }

        private IEnumerable<IWrapperProviderFactory> GetWrapperProviderFactories()
        {
            var wrapperProviderFactories = new List<IWrapperProviderFactory>();
            wrapperProviderFactories.Add(new EnumerableWrapperProviderFactory(wrapperProviderFactories));
            wrapperProviderFactories.Add(new SerializableErrorWrapperProviderFactory());

            return wrapperProviderFactories;
        }
    }
}