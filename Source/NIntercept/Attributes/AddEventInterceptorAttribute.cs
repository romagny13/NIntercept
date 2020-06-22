using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public class AddEventInterceptorAttribute : InterceptorAttributeBase, IAddEventInterceptorProvider
    {
        public AddEventInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
