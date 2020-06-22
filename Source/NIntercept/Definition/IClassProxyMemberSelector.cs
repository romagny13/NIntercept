using System.Reflection;

namespace NIntercept.Definition
{
    public interface IClassProxyMemberSelector
    {
        bool IncludeEvent(EventInfo @event);
        bool IncludeMethod(MethodInfo method);
        bool IncludeProperty(PropertyInfo property);
    }
}
