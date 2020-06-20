using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public class RemoveEventInterceptorAttribute : InterceptorAttributeBase, IRemoveEventInterceptorProvider
    {
        public RemoveEventInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
