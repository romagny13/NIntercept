using System.Reflection.Emit;
using NIntercept.Definition;

namespace NIntercept.Builder
{
    public interface ICallbackMethodBuilder
    {
        MethodBuilder CreateMethod(ProxyScope proxyScope, CallbackMethodDefinition callbackMethodDefinition);
    }
}