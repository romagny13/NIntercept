using NIntercept;

namespace System.Reflection.Interception
{
    public static class MethodInfoExtensions
    {
        public static object Intercept(this MethodInfo method, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            return ReflectionInterception.Intercept(method, method, target, parameters, typeof(IMethodInterceptorProvider), interceptors);
        }
    }
}
