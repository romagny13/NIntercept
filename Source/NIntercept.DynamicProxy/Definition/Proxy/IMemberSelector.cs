using System.Reflection;

namespace NIntercept.Definition
{
    public interface IMemberSelector
    {
        bool IncludeEvent(EventInfo @event);
        bool IncludeMethod(MethodInfo method);
        bool IncludeProperty(PropertyInfo property);
    }
}
