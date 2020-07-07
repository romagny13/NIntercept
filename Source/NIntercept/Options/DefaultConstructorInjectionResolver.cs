using System;
using System.Reflection;

namespace NIntercept
{
    public class DefaultConstructorInjectionResolver : IConstructorInjectionResolver
    {
        public virtual object Resolve(ParameterInfo parameter)
        {
            return Resolve(parameter.ParameterType);
        }

        public virtual object Resolve(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
