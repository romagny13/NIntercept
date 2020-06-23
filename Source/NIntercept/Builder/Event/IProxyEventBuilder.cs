using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyEventBuilder
    {
        IProxyMethodBuilder ProxyMethodBuilder { get; set; }

        EventBuilder CreateEvent(ModuleScope moduleScope, TypeBuilder proxyTypeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields);
    }
}