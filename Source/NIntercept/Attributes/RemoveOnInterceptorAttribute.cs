using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public class RemoveOnInterceptorAttribute : InterceptorAttributeBase, IRemoveOnInterceptorProvider
    {
        public RemoveOnInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
