using System;

namespace NIntercept
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SetterInterceptor : InterceptorAttributeBase, ISetterInterceptorProvider
    {
        public SetterInterceptor(Type interceptorType) : base(interceptorType)
        {
        }
    }
}
