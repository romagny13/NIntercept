using System;
using System.Reflection;

namespace NIntercept
{
    public interface IConstructorInjectionResolver
    {
        object Resolve(ParameterInfo parameter);
    }
}