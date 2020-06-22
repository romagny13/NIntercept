using NIntercept;

namespace System.Reflection.Interception
{
    public static class PropertyInfoExtensions
    {
        public static object InterceptGet(this PropertyInfo property, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (!property.CanRead)
                throw new InvalidOperationException($"Cannot read Property '{property.Name}'");

            return ReflectionInterception.Intercept(property, property.GetMethod, target, parameters, typeof(IPropertyGetInterceptorProvider), interceptors);
        }

        public static TReturnValue InterceptGet<TReturnValue>(this PropertyInfo property, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            return (TReturnValue)InterceptGet(property, target, parameters, interceptors);
        }

        public static void InterceptSet(this PropertyInfo property, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (!property.CanWrite)
                throw new InvalidOperationException($"Cannot write Property '{property.Name}'");

            ReflectionInterception.Intercept(property, property.SetMethod, target, parameters, typeof(IPropertySetInterceptorProvider), interceptors);
        }
    }
}
