using System;

namespace NIntercept
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class InterceptorAttributeBase : Attribute, IInterceptorProvider
    {
        private Type interceptorType;

        protected InterceptorAttributeBase(Type interceptorType)
        {
            CheckInterceptorType(interceptorType);

            this.interceptorType = interceptorType;
        }

        public Type InterceptorType
        {
            get { return interceptorType; }
        }

        protected virtual void CheckInterceptorType(Type interceptorType)
        {
            if (interceptorType is null)
                throw new ArgumentNullException(nameof(interceptorType));
            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
                throw new InvalidOperationException($"The interceptor type for the attribute '{nameof(InterceptorAttributeBase)}' must implement IInterceptor.");
        }
    }
}
