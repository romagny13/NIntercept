using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Builder
{

    public class CallbackMethodBuilder : ICallbackMethodBuilder
    {
        public virtual MethodBuilder CreateMethod(ProxyScope proxyScope, CallbackMethodDefinition callbackMethodDefinition)
        {
            // method
            MethodBuilder methodBuilder = DefineMethod(proxyScope.TypeBuilder, callbackMethodDefinition);

            // parameters
            GenericTypeParameterBuilder[] genericTypeParameters = methodBuilder.DefineGenericParameters(callbackMethodDefinition.GenericArguments);
            if (callbackMethodDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, callbackMethodDefinition);

            // body
            var il = methodBuilder.GetILGenerator();

            // locals
            LocalBuilder resultLocalBuilder = null;
            LocalBuilder targetLocalBuilder = null;
            Type returnType = callbackMethodDefinition.ReturnType;
            if (returnType != typeof(void))
            {
                if (returnType.ContainsGenericParameters)
                    resultLocalBuilder = il.DeclareLocal(genericTypeParameters.First(p => p.Name == returnType.Name));
                else
                    resultLocalBuilder = il.DeclareLocal(returnType);
            }
            Type targetType = callbackMethodDefinition.TypeDefinition.TargetType;
            if (targetType != null)
                targetLocalBuilder = il.DeclareLocal(targetType);

            il.Emit(OpCodes.Nop);

            BeforeInvoke(proxyScope, il, callbackMethodDefinition);

            if (targetType != null)
            {
                // return ((TargetType)target).Method(p1, p2 ...);
                string fieldName = callbackMethodDefinition.TypeDefinition.TargetFieldName;
                EmitHelper.StoreFieldInLocal(il, proxyScope.FieldBuilders.First(p => p.Name == fieldName), targetLocalBuilder);
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

            if (callbackMethodDefinition.TypeDefinition.TypeDefinitionType == TypeDefinitionType.InterfaceProxy && targetType == null)
                ThrowInterfaceProxyWithoutTargetException(il, callbackMethodDefinition);
            else
            {
                CallMethodOnTarget(il, callbackMethodDefinition, genericTypeParameters);

                if (returnType != typeof(void))
                    il.Emit(OpCodes.Stloc, resultLocalBuilder);

                AfterInvoke(proxyScope, il, callbackMethodDefinition);

                if (returnType != typeof(void))
                    il.Emit(OpCodes.Ldloc, resultLocalBuilder);

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ret);
            }

            return methodBuilder;
        }

        protected MethodBuilder DefineMethod(TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(callbackMethodDefinition.Name, callbackMethodDefinition.MethodAttributes);
            methodBuilder.SetReturnType(callbackMethodDefinition.ReturnType);
            return methodBuilder;
        }

        protected void DefineParameters(MethodBuilder methodBuilder, CallbackMethodDefinition methodCallbackDefinition)
        {
            methodBuilder.SetParameters(methodCallbackDefinition.ParameterDefinitions.Select(p => p.ParameterType).ToArray());

            int index = 1;
            foreach (var parameterDefinition in methodCallbackDefinition.ParameterDefinitions)
                methodBuilder.DefineParameter(index++, parameterDefinition.Attributes, parameterDefinition.Name);
        }

        protected virtual void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            AdditionalCode additionalCode = (callbackMethodDefinition.TypeDefinition as ProxyTypeDefinition)?.Options?.AdditionalCode;
            if (additionalCode != null)
                additionalCode.BeforeInvoke(proxyScope, il, callbackMethodDefinition);
        }

        protected virtual void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            AdditionalCode additionalCode = (callbackMethodDefinition.TypeDefinition as ProxyTypeDefinition)?.Options?.AdditionalCode;
            if (additionalCode != null)
                additionalCode.AfterInvoke(proxyScope, il, callbackMethodDefinition);
        }

        protected void ThrowInterfaceProxyWithoutTargetException(ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldstr, $"InterfaceProxy without target '{callbackMethodDefinition.TypeDefinition.Name}', member '{callbackMethodDefinition.Method.Name}' . Use an interceptor to define the behavior and the return value. But do not call Proceed method with the interceptor.");
            il.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(new Type[] { typeof(string) }));
            il.Emit(OpCodes.Throw);
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
