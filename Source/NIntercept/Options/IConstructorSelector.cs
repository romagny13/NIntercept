using System;
using System.Reflection;

namespace NIntercept
{
    public interface IConstructorSelector
    {
        ConstructorInfo Select(Type type);
    }
}