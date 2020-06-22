using NIntercept.Helpers;
using System.Reflection;

namespace NIntercept.Definition
{
    public class ClassProxyMemberSelector : IClassProxyMemberSelector
    {
        public virtual bool IncludeProperty(PropertyInfo property)
        {
            MethodInfo method = property.CanRead ? property.GetMethod : property.SetMethod;
            return method.IsVirtual && !MethodInfoHelper.IsInternal(method);
        }

        public virtual bool IncludeMethod(MethodInfo method)
        {
            return method.IsVirtual && !method.IsAbstract && !method.IsFinal && method.DeclaringType != typeof(object) && !MethodInfoHelper.IsInternal(method);
        }

        public virtual bool IncludeEvent(EventInfo @event)
        {
            MethodInfo method = @event.AddMethod;
            return method.IsVirtual && !method.IsFinal && !MethodInfoHelper.IsInternal(method);
        }
    }
}
