using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NIntercept
{
    public abstract class AsyncInterceptor : Interceptor
    {
        private static readonly MethodInfo ExecuteAsyncWithReturnValueMethod =
            typeof(AsyncInterceptor).GetMethod(nameof(ExecuteAsyncWithReturnValue), BindingFlags.NonPublic | BindingFlags.Instance);
        
        private static readonly MethodInfo GetDefaultMethod =
            typeof(AsyncInterceptor).GetMethod("GetDefault", BindingFlags.NonPublic | BindingFlags.Instance);

        public override void Intercept(IInvocation invocation)
        {
            var returnType = invocation.CallerMethod.ReturnType;
            if (returnType == typeof(Task))
            {
                ProcessOnEnterAndProceed(invocation);
                ProcessAsynchronous(invocation);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                ProcessOnEnterAndProceed(invocation);
                ProcessAsynchronousWithReturnValue(invocation, returnType);
            }
            else
                ProcessSynchronous(invocation);
        }

        protected Task<T> GetDefault<T>()
        {
            return Task.FromResult<T>(default(T));
        }

        protected virtual void ProcessAsynchronous(IInvocation invocation)
        {
            if (invocation.ReturnValue == null) // null if throw exception on enter
            {
                invocation.ReturnValue = Task.FromResult<object>(null);
                ProcessOnExit(invocation);
            }
            else
                invocation.ReturnValue = ExecuteAsync((Task)invocation.ReturnValue, invocation);
        }

        protected virtual void ProcessAsynchronousWithReturnValue(IInvocation invocation, Type returnType)
        {
            Type taskReturnType = returnType.GetGenericArguments()[0];
            if (invocation.ReturnValue == null)
            {
                var method = GetDefaultMethod.MakeGenericMethod(taskReturnType);
                invocation.ReturnValue = method.Invoke(this, new object[0]);
                ProcessOnExit(invocation);
            }
            else
            {
                var method = ExecuteAsyncWithReturnValueMethod.MakeGenericMethod(taskReturnType);
                invocation.ReturnValue = method.Invoke(this, new object[] { invocation.ReturnValue, invocation });
            }
        }

        protected virtual async Task ExecuteAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnException(invocation, ex);
                if (Throw)
                    throw ex;
            }
            ProcessOnExit(invocation);
        }

        protected virtual async Task<TReturnValue> ExecuteAsyncWithReturnValue<TReturnValue>(Task<TReturnValue> task, IInvocation invocation)
        {
            TReturnValue result = default(TReturnValue);
            try
            {
                result = await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnException(invocation, ex);
                if (Throw)
                    throw ex;
            }

            ProcessOnExit(invocation);

            return result;
        }
    }
}
