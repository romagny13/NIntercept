using NIntercept;

namespace System.Reflection.Interception
{
    public static class EventInfoExtensions
    {
        public static void InterceptAdd(this EventInfo @event, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            ReflectionInterception.Intercept(@event, @event.AddMethod, target, parameters, typeof(IAddEventInterceptorProvider), interceptors);
        }

        public static void InterceptRemove(this EventInfo @event, object target, object[] parameters, params IInterceptor[] interceptors)
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            ReflectionInterception.Intercept(@event, @event.RemoveMethod, target, parameters, typeof(IRemoveEventInterceptorProvider), interceptors);
        }
    }
}
