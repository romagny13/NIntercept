using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyPropertyBuilder
    {
        IProxyMethodBuilder ProxyMethodBuilder { get; set; }

        PropertyBuilder CreateProperty(ModuleScope moduleScope, TypeBuilder typeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields);
    }
}
