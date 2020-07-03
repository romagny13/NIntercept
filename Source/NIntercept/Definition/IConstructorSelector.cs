using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public interface IConstructorSelector
    {
        ConstructorInfo Select(Type type);
    }
}