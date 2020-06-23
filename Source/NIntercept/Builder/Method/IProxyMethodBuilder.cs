using NIntercept.Definition;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyMethodBuilder
    {
        IInvocationTypeBuilder InvocationTypeBuilder { get; }
        ICallbackMethodBuilder CallbackMethodBuilder { get; }

        MethodBuilder CreateMethod(ModuleScope moduleScope, TypeBuilder typeBuilder, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields);
    }

}
