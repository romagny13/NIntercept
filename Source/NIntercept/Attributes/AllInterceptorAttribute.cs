using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class AllInterceptorAttribute : InterceptorAttributeBase, 
        IPropertyGetInterceptorProvider, 
        IPropertySetInterceptorProvider,
        IMethodInterceptorProvider,
        IAddEventInterceptorProvider,
        IRemoveEventInterceptorProvider
    {
        public AllInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
