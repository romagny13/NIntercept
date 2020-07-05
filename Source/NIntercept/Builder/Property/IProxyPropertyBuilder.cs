using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public interface IProxyPropertyBuilder
    {
        PropertyBuilder CreateProperty(ProxyScope proxyScope, PropertyDefinition propertyDefinition);
    }
}
