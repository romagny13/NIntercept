using System;
using System.Reflection;

namespace NIntercept.Reflection
{
    public class MethodAccessor : IMemberAccessor
    {
        private MethodInfo method;
        private TypeAccessor typeAccessor;

        public MethodAccessor(TypeAccessor typeAccessor, MethodInfo method)
        {
            if (typeAccessor is null)
                throw new ArgumentNullException(nameof(typeAccessor));
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            this.typeAccessor = typeAccessor;
            this.method = method;
        }

        public TypeAccessor TypeAccessor
        {
            get { return typeAccessor; }
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public string Name
        {
            get { return method.Name; }
        }

        public virtual MethodAccessor MakeGenericMethod(Type[] typeArguments)
        {
            method = method.MakeGenericMethod(typeArguments);
            return this;
        }

        public virtual object Invoke(object[] parameters)
        {
            if (method.IsStatic)
                return method.Invoke(null, parameters);
            else
            {
                var target = typeAccessor.Target;
                if (target == null)
                    throw new InvalidOperationException("Target cannot be null");
                return method.Invoke(target, parameters);
            }
        }

        public TReturnValue Invoke<TReturnValue>(object[] parameters)
        {
            return (TReturnValue)Invoke(parameters);
        }

        public override string ToString()
        {
            return Name;
        }

        public object Intercept(object[] parameters, params IInterceptor[] interceptors)
        {
            var target = method.IsStatic ? null : typeAccessor.Target;
            return ReflectionInterception.Intercept(method, method, target, parameters, typeof(IMethodInterceptorProvider), interceptors);
        }
    }
}
