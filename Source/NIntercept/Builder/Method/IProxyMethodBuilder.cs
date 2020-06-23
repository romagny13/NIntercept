using NIntercept.Definition;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IProxyMethodBuilder
    {
        IInvocationTypeBuilder InvocationTypeBuilder { get; set; }
        ICallbackMethodBuilder CallbackMethodBuilder { get; set; }

        MethodBuilder CreateMethod(ModuleScope moduleScope, TypeBuilder typeBuilder, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields);
    }

}
