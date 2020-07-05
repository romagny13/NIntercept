using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class AllInterceptorAttribute : InterceptorAttributeBase, 
        IGetterInterceptorProvider, 
        ISetterInterceptorProvider,
        IMethodInterceptorProvider,
        IAddOnInterceptorProvider,
        IRemoveOnInterceptorProvider
    {
        public AllInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
