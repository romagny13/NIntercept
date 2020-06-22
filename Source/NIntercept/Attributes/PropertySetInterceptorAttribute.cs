using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropertySetInterceptorAttribute : InterceptorAttributeBase, IPropertySetInterceptorProvider
    {
        public PropertySetInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
