using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public class AddOnInterceptorAttribute : InterceptorAttributeBase, IAddOnInterceptorProvider
    {
        public AddOnInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
