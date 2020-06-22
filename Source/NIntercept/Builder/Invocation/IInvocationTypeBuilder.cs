using NIntercept.Definition;
using System;
using System.Reflection.Emit;

namespace NIntercept
{
    public interface IInvocationTypeBuilder
    {
        Type CreateType(ModuleBuilder moduleBuilder, InvocationTypeDefinition invocationTypeDefinition, MethodBuilder callbackMethodBuilder);
    }

}
