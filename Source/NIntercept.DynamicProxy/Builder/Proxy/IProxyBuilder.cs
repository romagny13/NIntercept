using System.Reflection.Emit;
using NIntercept.Definition;

namespace NIntercept
{
    public interface IProxyBuilder
    {
        IProxyEventBuilder ProxyEventBuilder { get; set; }
        IProxyMethodBuilder ProxyMethodBuilder { get; set; }
        IProxyModuleBuilder ProxyModuleBuilder { get; }
        IProxyPropertyBuilder ProxyPropertyBuilder { get; set; }

        TypeBuilder CreateType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors);
    }
}