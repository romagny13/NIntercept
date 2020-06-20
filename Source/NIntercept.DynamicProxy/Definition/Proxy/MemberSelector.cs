using System.Reflection;

namespace NIntercept.Definition
{
    public class MemberSelector : IMemberSelector
    {
        public virtual bool IncludeProperty(PropertyInfo property)
        {
            MethodInfo method = property.CanRead ? property.GetMethod : property.SetMethod;
            return method.IsVirtual && !MethodBuilderHelper.IsInternal(property);
        }

        public virtual bool IncludeMethod(MethodInfo method)
        {
            return method.IsVirtual && !method.IsAbstract && !method.IsFinal && method.DeclaringType != typeof(object) && !MethodBuilderHelper.IsInternal(method);
        }

        public virtual bool IncludeEvent(EventInfo @event)
        {
            MethodInfo method = @event.AddMethod;
            return method.IsVirtual && !method.IsFinal && !MethodBuilderHelper.IsInternal(method);
        }
    }
}
