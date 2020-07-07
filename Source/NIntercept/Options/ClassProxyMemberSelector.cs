using NIntercept.Helpers;
using System.Reflection;

namespace NIntercept
{
    public class ClassProxyMemberSelector 
    {
        public bool IncludeProperty(PropertyInfo property)
        {
            MethodInfo method = property.CanRead ? property.GetMethod : property.SetMethod;
            return method.IsVirtual && !MethodAttributesHelper.IsInternal(method) && Filter(property);
        }

        public bool IncludeMethod(MethodInfo method)
        {
            return method.IsVirtual && !method.IsAbstract && !method.IsFinal && !MethodAttributesHelper.IsInternal(method) && Filter(method);
        }

        public bool IncludeEvent(EventInfo @event)
        {
            MethodInfo method = @event.AddMethod;
            return method.IsVirtual && !method.IsFinal && !MethodAttributesHelper.IsInternal(method) && Filter(@event);
        }

        public virtual bool Filter(MemberInfo member)
        {
            return true;
        } 
    }
}
