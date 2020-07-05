using System;
using System.Reflection;

namespace NIntercept.Reflection
{
    public class ReflectionInvocation : IInvocation
    {
        private IInterceptor[] interceptors;
        private readonly MemberInfo member;
        private readonly MethodInfo callerMethod;
        private readonly Type interceptorProviderType;
        private object target;
        private object[] parameters;
        private object returnValue;
        private int currentInterceptorIndex;

        public ReflectionInvocation(object target, IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, Type interceptorProviderType, object[] parameters)
        {
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));
            if (member is null)
                throw new ArgumentNullException(nameof(member));
            if (callerMethod is null)
                throw new ArgumentNullException(nameof(callerMethod));
            if (interceptorProviderType is null)
                throw new ArgumentNullException(nameof(interceptorProviderType));
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            this.target = target;
            this.member = member;
            this.callerMethod = callerMethod;
            this.parameters = parameters;
            this.interceptorProviderType = interceptorProviderType;
            this.interceptors = GetAllInterceptors(member, interceptors, interceptorProviderType);
            this.currentInterceptorIndex = -1;
        }

        public object Target
        {
            get { return target; }
        }

        public MemberInfo Member
        {
            get { return member; }
        }

        public MethodInfo CallerMethod
        {
            get { return callerMethod; }
        }

        public object[] Parameters
        {
            get { return parameters; }
        }

        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        public object Proxy
        {
            get { return null; }
        }

        public Type InterceptorProviderType
        {
            get { return interceptorProviderType; }
        }

        public virtual T GetParameter<T>(int index)
        {
            return (T)parameters[index];
        }

        protected virtual IInterceptor[] GetAllInterceptors(MemberInfo member, IInterceptor[] interceptors, Type interceptorProviderType)
        {
            return InterceptorResolver.GetAllInterceptors(member, interceptors, interceptorProviderType);
        }

        public virtual void Proceed()
        {
            currentInterceptorIndex++;
            try
            {
                if (currentInterceptorIndex == interceptors.Length)
                    InvokeMethodOnTarget();
                else if (currentInterceptorIndex < interceptors.Length)
                {
                    interceptors[currentInterceptorIndex].Intercept(this);
                }
                else
                    throw new IndexOutOfRangeException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                currentInterceptorIndex--;
            }
        }

        class AwaitableInvocation : IAwaitableInvocation
        {
            private readonly ReflectionInvocation invocation;
            private readonly int originalInterceptorIndex;

            public AwaitableInvocation(ReflectionInvocation invocation)
            {
                this.invocation = invocation;
                this.originalInterceptorIndex = invocation.currentInterceptorIndex;
            }

            public void Proceed()
            {
                int invocationInterceptorIndex = invocation.currentInterceptorIndex;
                try
                {
                    invocation.currentInterceptorIndex = originalInterceptorIndex;
                    invocation.Proceed();
                }
                finally
                {
                    invocation.currentInterceptorIndex = invocationInterceptorIndex;
                }
            }
        }

        public IAwaitableInvocation GetAwaitableContext()
        {
            return new AwaitableInvocation(this);
        }

        protected virtual void InvokeMethodOnTarget()
        {
            ReturnValue = callerMethod.Invoke(Target, parameters);
        }
    }
}
