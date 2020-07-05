using NIntercept.Definition;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public interface IProxyMethodBuilder
    {
        MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member);
    }

}
