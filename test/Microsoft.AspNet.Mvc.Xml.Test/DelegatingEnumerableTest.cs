// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Xml
{
    public class DelegatingEnumerableTest
    {
        [Fact]
        public void Dispose_CalledOn_InnerEnumerable_InnerEnumerator()
        {
            // Arrange
            var innerEnumerator = new Mock<IEnumerator<int>>();
            innerEnumerator.Setup(ie => ie.Dispose())
                            .Verifiable();
            var innerEnumerable = new Mock<IEnumerable<int>>();
            innerEnumerable.Setup(ie => ie.GetEnumerator())
                            .Returns(innerEnumerator.Object);
            var delegatingEnumerable = new DelegatingEnumerable<int, int>(
                                                                    innerEnumerable.Object, 
                                                                    wrapperProvider: null);

            // Act
            foreach(var i in delegatingEnumerable)
            {
            }

            // Assert
            innerEnumerator.Verify();
        }
    }
}