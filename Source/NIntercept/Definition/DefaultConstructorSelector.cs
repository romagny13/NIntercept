using System;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        public virtual ConstructorInfo Select(Type type)
        {
            return type.GetConstructors().FirstOrDefault();
        }
    }
}
