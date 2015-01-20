// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.WebApiCompatShim;
using XmlFormattersWebSite.Models;

namespace XmlFormattersWebSite.Controllers
{
    public class PeopleController : ApiController
    {
        public IQueryable<Person> GetAll()
        {
            return (new[] {
                new Person() { Id = 10, Name = "Mike" },
                new Person() { Id = 11, Name = "Jimmy" }
            }).AsQueryable();
        }
    }
}