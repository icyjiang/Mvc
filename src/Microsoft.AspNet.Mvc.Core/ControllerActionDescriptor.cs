// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.AspNet.Mvc
{
    [DebuggerDisplay("CA {DisplayName}(RC-{RouteConstraints.Count})")]
    public class ControllerActionDescriptor : ActionDescriptor
    {
        public string ControllerName { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public TypeInfo ControllerTypeInfo { get; set; }

        public override string DisplayName
        {
            get
            {
                if (base.DisplayName == null && ControllerTypeInfo != null && MethodInfo != null)
                {
                    base.DisplayName = string.Format(
                        "{0}.{1}",
                        ControllerTypeInfo.FullName,
                        MethodInfo.Name);
                }

                return base.DisplayName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                base.DisplayName = value;
            }
        }
    }
}
