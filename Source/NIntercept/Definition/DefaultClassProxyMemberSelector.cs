using NIntercept.Helpers;
using System.Reflection;

namespace NIntercept.Definition
{
    public class DefaultClassProxyMemberSelector : IClassProxyMemberSelector
    {
        public virtual bool IncludeProperty(PropertyInfo property)
        {
            MethodInfo method = property.CanRead ? property.GetMethod : property.SetMethod;
            return method.IsVirtual && !MethodAttributesHelper.IsInternal(method);
        }

        public virtual bool IncludeMethod(MethodInfo method)
        {
            return method.IsVirtual && !method.IsAbstract && !method.IsFinal && method.DeclaringType != typeof(object) && !MethodAttributesHelper.IsInternal(method);
        }

        public virtual bool IncludeEvent(EventInfo @event)
        {
            MethodInfo method = @event.AddMethod;
            return method.IsVirtual && !method.IsFinal && !MethodAttributesHelper.IsInternal(method);
        }
    }
}
