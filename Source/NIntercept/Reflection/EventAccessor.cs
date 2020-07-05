using System;
using System.Reflection;

namespace NIntercept.Reflection
{
    public class EventAccessor : IMemberAccessor
    {
        private TypeAccessor typeAccessor;
        private EventInfo @event;

        public EventAccessor(TypeAccessor typeAccessor, EventInfo @event)
        {
            if (typeAccessor is null)
                throw new ArgumentNullException(nameof(typeAccessor));
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));

            this.typeAccessor = typeAccessor;
            this.@event = @event;
        }

        public TypeAccessor TypeAccessor
        {
            get { return typeAccessor; }
        }

        public EventInfo Event
        {
            get { return @event; }
        }

        public string Name
        {
            get { return @event.Name; }
        }

        public virtual void AddEventHandler(Delegate @delegate)
        {
            Invoke(@event.AddMethod, @delegate);
        }

        public virtual void RemoveEventHandler(Delegate @delegate)
        {
            Invoke(@event.RemoveMethod, @delegate);
        }

        protected virtual void Invoke(MethodInfo method, Delegate @delegate)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (@delegate is null)
                throw new ArgumentNullException(nameof(@delegate));

            if (method.IsStatic)
                method.Invoke(null, new object[] { @delegate });
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                method.Invoke(target, new object[] { @delegate });
            }
        }

        public virtual void InterceptAdd(object[] parameters, params IInterceptor[] interceptors)
        {
            var method = @event.AddMethod;
            var target = method.IsStatic ? null : typeAccessor.Target;
            ReflectionInterception.Intercept(@event, method, target, parameters, typeof(IAddOnInterceptorProvider), interceptors);
        }

        public virtual void InterceptRemove(object[] parameters, params IInterceptor[] interceptors)
        {
            var method = @event.RemoveMethod;
            var target = method.IsStatic ? null : typeAccessor.Target;
            ReflectionInterception.Intercept(@event, method, target, parameters, typeof(IRemoveOnInterceptorProvider), interceptors);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
