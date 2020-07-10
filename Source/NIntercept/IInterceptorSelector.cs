using System;
using System.Reflection;

namespace NIntercept
{
    public interface IInterceptorSelector
    {
        IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors);
    }
}
