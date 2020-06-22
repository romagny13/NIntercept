using System;
using System.Reflection;

namespace NIntercept
{
    public abstract class Invocation : IInvocation
    {
        private IInterceptor[] interceptors;
        private readonly MemberInfo member;
        private object proxy;
        private object target;
        private MethodInfo callerMethod;
        private object[] parameters;
        private object returnValue;
        private int currentInterceptorIndex;

        public Invocation(object target, IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
        {
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));
            if (member is null)
                throw new ArgumentNullException(nameof(member));
            if (callerMethod is null)
                throw new ArgumentNullException(nameof(callerMethod));
            if (proxy is null)
                throw new ArgumentNullException(nameof(proxy));
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            this.target = target;
            this.member = member;
            this.callerMethod = callerMethod;
            this.proxy = proxy;
            this.parameters = parameters;
            currentInterceptorIndex = -1;
            this.interceptors = GetAllInterceptors(callerMethod, interceptors);
        }

        public MemberInfo Member
        {
            get { return member; }
        }

        public object[] Parameters
        {
            get { return parameters; }
        }

        public object Proxy
        {
            get { return proxy; }
        }

        public object Target
        {
            get { return target; }
        }

        public MethodInfo CallerMethod
        {
            get { return callerMethod; }
        }

        public object ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }

        public virtual T GetParameter<T>(int index)
        {
            return (T)parameters[index];
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

        protected virtual IInterceptor[] GetAllInterceptors(MethodInfo callerMethod, IInterceptor[] interceptors)
        {
            return InterceptorResolver.GetAllInterceptors(callerMethod, interceptors, InterceptorProviderType);
        }

        class AwaitableInvocation : IAwaitableInvocation
        {
            private readonly Invocation invocation;
            private readonly int originalInterceptorIndex;

            public AwaitableInvocation(Invocation invocation)
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

        public abstract Type InterceptorProviderType { get; }

        protected abstract void InvokeMethodOnTarget();
    }

}
