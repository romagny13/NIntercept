using System;
using System.Reflection;

namespace NIntercept.Reflection
{
    public class PropertyAccessor : IMemberAccessor
    {
        private TypeAccessor typeAccessor;
        private PropertyInfo property;

        public PropertyAccessor(TypeAccessor typeAccessor, PropertyInfo property)
        {
            if (typeAccessor is null)
                throw new ArgumentNullException(nameof(typeAccessor));
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            this.typeAccessor = typeAccessor;
            this.property = property;
        }

        public TypeAccessor TypeAccessor
        {
            get { return typeAccessor; }
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public string Name
        {
            get { return property.Name; }
        }

        public virtual object GetValue(object[] index)
        {
            if (!property.CanRead)
                throw new InvalidOperationException($"Property '{property.Name}' cannot read.");

            if (property.GetMethod.IsStatic)
                return property.GetValue(null, index);
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                return property.GetValue(target, index);
            }
        }

        public object GetValue()
        {
            return GetValue(null);
        }

        public TReturnValue GetValue<TReturnValue>()
        {
            return (TReturnValue)GetValue(null);
        }

        public TReturnValue GetValue<TReturnValue>(object[] index)
        {
            return (TReturnValue)GetValue(index);
        }

        public virtual void SetValue(object value, object[] index)
        {
            if (!property.CanWrite)
                throw new InvalidOperationException($"Property '{property.Name}' cannot write.");

            if (property.SetMethod.IsStatic)
                property.SetValue(null, value, index);
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                property.SetValue(target, value, index);
            }
        }

        public void SetValue(object value)
        {
            SetValue(value, null);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual object InterceptGet(object[] parameters, params IInterceptor[] interceptors)
        {
            if (!property.CanRead)
                throw new InvalidOperationException($"Property '{property.Name}' cannot read.");

            var method = property.GetMethod;
            var target = method.IsStatic ? null : typeAccessor.Target;
            return ReflectionInterception.Intercept(property, method, target, parameters, typeof(IGetterInterceptorProvider), interceptors);
        }

        public TReturnValue InterceptGet<TReturnValue>(object[] parameters, params IInterceptor[] interceptors)
        {
            return (TReturnValue)InterceptGet(parameters, interceptors);
        }

        public virtual void InterceptSet(object[] parameters, params IInterceptor[] interceptors)
        {
            if (!property.CanWrite)
                throw new InvalidOperationException($"Cannot write Property '{property.Name}'");

            var method = property.SetMethod;
            var target = method.IsStatic ? null : typeAccessor.Target;
            ReflectionInterception.Intercept(property, method, target, parameters, typeof(ISetterInterceptorProvider), interceptors);
        }
    }
}
