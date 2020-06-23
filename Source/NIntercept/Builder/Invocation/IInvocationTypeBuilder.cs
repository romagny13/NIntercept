using NIntercept.Definition;
using System;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IInvocationTypeBuilder
    {
        Type CreateType(ModuleBuilder moduleBuilder, TypeBuilder proxyTypeBuilder, InvocationTypeDefinition invocationTypeDefinition, MethodBuilder callbackMethodBuilder);
    }

}
