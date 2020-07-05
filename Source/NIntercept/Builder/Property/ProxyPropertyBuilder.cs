using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public class ProxyPropertyBuilder : IProxyPropertyBuilder
    {
        public virtual PropertyBuilder CreateProperty(ProxyScope proxyScope, PropertyDefinition propertyDefinition)
        {
            Type returnType = propertyDefinition.PropertyType;
            PropertyBuilder propertyBuilder = DefineProperty(proxyScope.TypeBuilder, propertyDefinition, returnType);

            DefineGetMethod(proxyScope, propertyDefinition, propertyBuilder);

            DefineSetMethod(proxyScope, propertyDefinition, propertyBuilder);

            return propertyBuilder;
        }

        protected PropertyBuilder DefineProperty(TypeBuilder proxyTypeBuilder, PropertyDefinition propertyDefinition, Type returnType)
        {
            return proxyTypeBuilder.DefineProperty(propertyDefinition.Name, propertyDefinition.Attributes, returnType, new Type[] { returnType });
        }

        protected virtual void DefineGetMethod(ProxyScope proxyScope, PropertyDefinition propertyDefinition, PropertyBuilder propertyBuilder)
        {
            PropertyGetMethodDefinition getMethodDefinition = propertyDefinition.GetMethodDefinition;
            if (getMethodDefinition != null)
            {
                MethodBuilder getMethodBuilder = proxyScope.CreateMethod(getMethodDefinition, propertyDefinition.Property);
                if (ShouldAddInterceptionAttributes(getMethodDefinition))
                    AttributeHelper.AddInterceptorAttributes(getMethodBuilder, propertyDefinition.PropertyGetInterceptorAttributes);
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }
        }

        protected virtual void DefineSetMethod(ProxyScope proxyScope, PropertyDefinition propertyDefinition, PropertyBuilder propertyBuilder)
        {
            PropertySetMethodDefinition setMethodDefinition = propertyDefinition.SetMethodDefinition;
            if (setMethodDefinition != null)
            {
                MethodBuilder setMethodBuilder = proxyScope.CreateMethod(setMethodDefinition, propertyDefinition.Property);
                if (ShouldAddInterceptionAttributes(setMethodDefinition))
                    AttributeHelper.AddInterceptorAttributes(setMethodBuilder, propertyDefinition.PropertySetInterceptorAttributes);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

        protected virtual bool ShouldAddInterceptionAttributes(MethodDefinition methodDefinition)
        {
            return true;
        }
    }
}
