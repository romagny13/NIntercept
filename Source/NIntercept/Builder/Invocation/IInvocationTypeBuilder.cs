using NIntercept.Definition;
using System;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public interface IInvocationTypeBuilder
    {
        Type CreateType(ModuleScope moduleScope, InvocationTypeDefinition invocationTypeDefinition, MethodBuilder callbackMethodBuilder);
    }

}
