using System.Reflection.Emit;
using NIntercept.Definition;

namespace NIntercept
{
    public interface ICallbackMethodBuilder
    {
        MethodBuilder CreateMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition, FieldBuilder[] fields);
    }
}