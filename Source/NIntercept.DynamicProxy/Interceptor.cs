using System;

namespace NIntercept
{
    public abstract class Interceptor : IInterceptor
    {
        private bool @throw;

        public virtual bool Throw
        {
            get { return @throw; }
            set { @throw = value; }
        }

        public virtual void Intercept(IInvocation invocation)
        {
            ProcessSynchronous(invocation);
        }

        protected virtual void ProcessSynchronous(IInvocation invocation)
        {
            ProcessOnEnterAndProceed(invocation);
            ProcessOnExit(invocation);
        }

        protected virtual void ProcessOnEnterAndProceed(IInvocation invocation)
        {
            ProcessSafe(invocation, () =>
            {
                OnEnter(invocation);
                invocation.Proceed();
            });
        }

        protected virtual void ProcessOnExit(IInvocation invocation)
        {
            ProcessSafe(invocation, () => OnExit(invocation));
        }

        protected virtual void ProcessSafe(IInvocation invocation, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                OnException(invocation, ex);
                if (@throw)
                    throw ex;
            }
        }

        protected abstract void OnEnter(IInvocation invocation);
        protected abstract void OnExit(IInvocation invocation);
        protected abstract void OnException(IInvocation invocation, Exception exception);
    }
}
