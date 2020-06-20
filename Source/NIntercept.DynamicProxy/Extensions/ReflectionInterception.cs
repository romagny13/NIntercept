using NIntercept;

namespace System.Reflection.Interception
{
    internal static class ReflectionInterception
    {
        public static object Intercept(MemberInfo member, MethodInfo callerMethod, object target, object[] parameters, Type interceptorProviderType, params IInterceptor[] interceptors)
        {
            if (member is null)
                throw new ArgumentNullException(nameof(member));
            if (callerMethod is null)
                throw new ArgumentNullException(nameof(callerMethod));
            if (interceptorProviderType is null)
                throw new ArgumentNullException(nameof(interceptorProviderType));

            try
            {
                var invocation = new ReflectionInvocation(target, interceptors, member, callerMethod, interceptorProviderType, parameters);
                invocation.Proceed();
                return invocation.ReturnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
