using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyPropertyBuilder : IProxyPropertyBuilder
    {
        public IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return ProxyServiceLocator.Current.ProxyMethodBuilder; }
        }

        public virtual PropertyBuilder CreateProperty(ModuleScope moduleScope, TypeBuilder proxyTypeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            Type returnType = propertyDefinition.PropertyType;
            PropertyBuilder propertyBuilder = proxyTypeBuilder.DefineProperty(propertyDefinition.Name, propertyDefinition.Attributes, returnType, new Type[] { returnType });

            PropertyGetMethodDefinition propertyGetMethodDefinition = propertyDefinition.PropertyGetMethodDefinition;
            if (propertyGetMethodDefinition != null)
            {
                MethodBuilder getMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, proxyTypeBuilder, propertyGetMethodDefinition, propertyDefinition.Property, fields);
                // copy attributes to method
                AttributeHelper.AddInterceptorAttributes(getMethodBuilder, propertyDefinition.PropertyGetInterceptorAttributes);
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            PropertySetMethodDefinition propertySetMethodDefinition = propertyDefinition.PropertySetMethodDefinition;
            if (propertySetMethodDefinition != null)
            {
                MethodBuilder setMethodBuilder = ProxyMethodBuilder.CreateMethod(moduleScope, proxyTypeBuilder, propertySetMethodDefinition, propertyDefinition.Property, fields);
                AttributeHelper.AddInterceptorAttributes(setMethodBuilder, propertyDefinition.PropertySetInterceptorAttributes);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            return propertyBuilder;
        }
    }
}
