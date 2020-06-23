using System.Reflection.Emit;
using NIntercept.Definition;

namespace NIntercept
{
    public interface IProxyBuilder
    {
        IProxyEventBuilder ProxyEventBuilder { get;  }
        IProxyMethodBuilder ProxyMethodBuilder { get; }
        ModuleScope ModuleScope { get; }
        IProxyPropertyBuilder ProxyPropertyBuilder { get; }

        TypeBuilder CreateType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors);
    }
}