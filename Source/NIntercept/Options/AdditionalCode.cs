using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept
{
    public class AdditionalCode
    {
        public virtual void BeforeDefine(ProxyScope proxyScope) { }

        public virtual void AfterDefine(ProxyScope proxyScope) { }

        public virtual void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition) { }

        public virtual void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition) { }
    }
}
