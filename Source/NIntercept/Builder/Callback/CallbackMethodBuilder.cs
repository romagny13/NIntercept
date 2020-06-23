using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class CallbackMethodBuilder : ICallbackMethodBuilder
    {
        public virtual MethodBuilder CreateMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition, FieldBuilder[] fields)
        {
            MethodBuilder methodBuilder = DefineMethod(typeBuilder, callbackMethodDefinition);

            GenericTypeParameterBuilder[] genericTypeParameters = methodBuilder.DefineGenericParameters(callbackMethodDefinition.GenericArguments);
            if (callbackMethodDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, callbackMethodDefinition);

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Nop);

            Type returnType = callbackMethodDefinition.ReturnType;
            // locals
            LocalBuilder resultLocalBuilder = null;
            if (returnType != typeof(void))
            {
                if (returnType.ContainsGenericParameters)
                    resultLocalBuilder = il.DeclareLocal(genericTypeParameters.First(p => p.Name == returnType.Name));
                else
                    resultLocalBuilder = il.DeclareLocal(returnType);
            }

            object target = callbackMethodDefinition.TypeDefinition.Target;
            if (target != null)
            {
                // return ((TargetType)target).Method(p1, p2 ...);
                LocalBuilder targetLocalBuilder = il.DeclareLocal(target.GetType());
                EmitHelper.StoreFieldInLocal(il, fields.ElementAt(1), targetLocalBuilder);
                il.Emit(OpCodes.Ldloc, targetLocalBuilder);
            }
            else
            {
                // return base.Method(p1, p2...);
                il.Emit(OpCodes.Ldarg_0);
            }

            var parameterDefinitions = callbackMethodDefinition.ParameterDefinitions;
            for (int i = 0; i < parameterDefinitions.Length; i++)
                il.EmitLdarg(i + 1);

            if (callbackMethodDefinition.TypeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy && target == null)
                ThrowInterfaceProxyWithoutTargetException(il, callbackMethodDefinition.TypeDefinition.Name);
            else
            {
                CallMethodOnTarget(il, callbackMethodDefinition, genericTypeParameters);

                if (returnType != typeof(void))
                {
                    il.Emit(OpCodes.Stloc, resultLocalBuilder);
                    il.Emit(OpCodes.Ldloc, resultLocalBuilder);
                }

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ret);
            }

            return methodBuilder;
        }

        protected void ThrowInterfaceProxyWithoutTargetException(ILGenerator il, string name)
        {
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldstr, $"InterfaceProxy {name} without Target. Set the return value with an interceptor and do not call Proceed.");
            il.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
        }

        protected virtual MethodBuilder DefineMethod(TypeBuilder typeBuilder, CallbackMethodDefinition methodCallbackDefinition)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodCallbackDefinition.Name, methodCallbackDefinition.MethodAttributes);
            methodBuilder.SetReturnType(methodCallbackDefinition.ReturnType);
            return methodBuilder;
        }

        protected void DefineParameters(MethodBuilder methodBuilder, CallbackMethodDefinition methodCallbackDefinition)
        {
            methodBuilder.SetParameters(methodCallbackDefinition.ParameterDefinitions.Select(p => p.ParameterType).ToArray());

            int index = 1;
            foreach (var parameterDefinition in methodCallbackDefinition.ParameterDefinitions)
                methodBuilder.DefineParameter(index++, parameterDefinition.Attributes, parameterDefinition.Name);
        }

        protected virtual void CallMethodOnTarget(ILGenerator il, CallbackMethodDefinition methodCallbackDefinition, GenericTypeParameterBuilder[] genericTypeParameters)
        {
            OpCode code = OpCodes.Call;
            if (methodCallbackDefinition.TypeDefinition.IsInterface) code = OpCodes.Callvirt;
            MethodInfo method = methodCallbackDefinition.Method;
            if (method.IsGenericMethod)
                il.Emit(code, method.MakeGenericMethod(genericTypeParameters));
            else
                il.Emit(code, method);
        }
    }
}
