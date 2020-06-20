using NIntercept.Definition;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyMethodBuilder
    {
        IInvocationTypeBuilder InvocationTypeBuilder { get; set; }
        ICallbackMethodBuilder MethodCallbackBuilder { get; set; }

        MethodBuilder CreateMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyMethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields);
    }

}
