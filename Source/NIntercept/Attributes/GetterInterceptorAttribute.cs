using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GetterInterceptor : InterceptorAttributeBase, IGetterInterceptorProvider
    {
        public GetterInterceptor(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
