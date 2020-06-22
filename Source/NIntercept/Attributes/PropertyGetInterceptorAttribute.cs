using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropertyGetInterceptorAttribute : InterceptorAttributeBase, IPropertyGetInterceptorProvider
    {
        public PropertyGetInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
