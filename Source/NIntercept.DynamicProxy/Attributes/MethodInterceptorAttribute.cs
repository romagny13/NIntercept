using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodInterceptorAttribute : InterceptorAttributeBase, IMethodInterceptorProvider
    {
        public MethodInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
