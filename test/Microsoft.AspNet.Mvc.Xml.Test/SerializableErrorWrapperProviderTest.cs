// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;
using Microsoft.AspNet.Mvc;

namespace Microsoft.AspNet.Mvc.Xml
{
    public class SerializableErrorWrapperProviderTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Gets_SerializableErrorWrapper_AsWrappingType(bool isSerialization)
        {
            // Arrange
            var wrapperProvider = new SerializableErrorWrapperProvider(
                                        new WrapperProviderContext(typeof(SerializableError), isSerialization));

            // Act and Assert
            Assert.Equal(typeof(SerializableErrorWrapper), wrapperProvider.WrappingType);
        }

        [Fact]
        public void Wraps_SerializableErrorInstance()
        {
            // Arrange
            var wrapperProvider = new SerializableErrorWrapperProvider(
                                        new WrapperProviderContext(typeof(SerializableError), isSerialization: true));
            var serializableError = new SerializableError();

            // Act
            var wrapped = wrapperProvider.Wrap(serializableError);

            // Assert
            Assert.NotNull(wrapped);
            var errorWrapper = wrapped as SerializableErrorWrapper;
            Assert.NotNull(errorWrapper);
            Assert.Same(serializableError, errorWrapper.SerializableError);
        }

        [Fact]
        public void DoesNotWrap_NullInstance()
        {
            // Arrange
            var wrapperProvider = new SerializableErrorWrapperProvider(
                                        new WrapperProviderContext(typeof(SerializableError), isSerialization: true));
            
            // Act
            var wrapped = wrapperProvider.Wrap(null);

            // Assert
            Assert.Null(wrapped);
        }

        [Fact]
        public void DoesNotWrap_NonSerializableErrorInstances()
        {
            // Arrange
            var wrapperProvider = new SerializableErrorWrapperProvider(
                                        new WrapperProviderContext(typeof(SerializableError), isSerialization: true));
            var person = new Person() { Id = 10, Name = "John" };

            // Act
            var wrapped = wrapperProvider.Wrap(person);

            // Assert
            Assert.Same(person, wrapped);
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}