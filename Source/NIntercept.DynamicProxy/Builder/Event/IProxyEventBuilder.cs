using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyEventBuilder
    {
        IProxyMethodBuilder ProxyMethodBuilder { get; set; }

        EventBuilder CreateEvent(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyEventDefinition eventDefinition, FieldBuilder[] fields);
    }
}