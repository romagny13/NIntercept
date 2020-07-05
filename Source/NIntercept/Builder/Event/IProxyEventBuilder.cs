using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public interface IProxyEventBuilder
    {
        EventBuilder CreateEvent(ProxyScope proxyScope, EventDefinition eventDefinition);
    }
}