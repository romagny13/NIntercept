using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyPropertyBuilder
    {
        IProxyMethodBuilder ProxyMethodBuilder { get; set; }

        PropertyBuilder CreateProperty(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyPropertyDefinition propertyDefinition, FieldBuilder[] fields);
    }
}
