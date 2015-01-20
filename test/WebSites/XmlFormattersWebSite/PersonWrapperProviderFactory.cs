using System;
using Microsoft.AspNet.Mvc.Xml;
using XmlFormattersWebSite.Models;

namespace XmlFormattersWebSite
{
    public class PersonWrapperProviderFactory : IWrapperProviderFactory
    {
        public IWrapperProvider GetProvider(WrapperProviderContext context)
        {
            if(context.DeclaredType == typeof(Person))
            {
                return new PersonWrapperProvider();
            }

            return null;
        }
    }
}