using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public class ProxyMethodBuilder : IProxyMethodBuilder
    {
        public virtual MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member)
        {
            MethodInfo method = methodDefinition.Method;

            // callback method
            MethodBuilder callbackMethodBuilder = proxyScope.CreateCallbackMethod(methodDefinition.CallbackMethodDefinition);

            // invocation type
            Type invocationType = proxyScope.ModuleScope.GetOrCreateInvocationType(methodDefinition.InvocationTypeDefinition, callbackMethodBuilder);
            if (invocationType.IsGenericType)
                invocationType = invocationType.MakeGenericType(method.GetGenericArguments()); 

            // method
            MethodBuilder methodBuilder = DefineMethod(proxyScope.TypeBuilder, methodDefinition);

            // parameters
            methodBuilder.DefineGenericParameters(methodDefinition.GenericArguments);
            if (methodDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, methodDefinition);

            // attributes
            DefineAttributes(methodBuilder, methodDefinition);

            // method body
            var il = methodBuilder.GetILGenerator();

            // locals
            var returnType = methodDefinition.ReturnType;
            Type targetType = methodDefinition.TypeDefinition.TargetType;
            Type localBuilderTargetType = targetType != null ? targetType : typeof(object);
            LocalBuilder targetLocalBuilder = il.DeclareLocal(localBuilderTargetType); // target
            LocalBuilder interceptorsLocalBuilder = il.DeclareLocal(typeof(IInterceptor[])); // interceptors
            LocalBuilder memberLocalBuilder = il.DeclareLocal(typeof(MemberInfo)); // MemberInfo
            LocalBuilder proxyMethodLocalBuilder = il.DeclareLocal(typeof(MethodInfo)); // proxy method
            LocalBuilder proxyLocalBuilder = il.DeclareLocal(typeof(object)); // proxy
            LocalBuilder parametersLocalBuilder = il.DeclareLocal(typeof(object[])); // parameters
            LocalBuilder invocationLocalBuilder = il.DeclareLocal(invocationType); // invocation
            LocalBuilder returnValueLocalBuilder = null;
            if (returnType != typeof(void))
                returnValueLocalBuilder = il.DeclareLocal(returnType);

            // fields received
            FieldBuilder interceptorsField = proxyScope.FieldBuilders[0];

            // Store fields in locals
            if (targetType != null)
            {
                string fieldName = methodDefinition.TypeDefinition.TargetFieldName;
                FieldBuilder targetField = proxyScope.FieldBuilders.First(p => p.Name == fieldName);
                EmitHelper.StoreFieldInLocal(il, targetField, targetLocalBuilder);
            }
            EmitHelper.StoreFieldInLocal(il, interceptorsField, interceptorsLocalBuilder);
            EmitHelper.StoreMemberToLocal(il, member, memberLocalBuilder);
            EmitHelper.StoreProxyMethodToLocal(il, proxyMethodLocalBuilder);
            EmitHelper.StoreThisInLocal(il, proxyLocalBuilder);
            StoreArgsToArray(il, methodDefinition.ParameterDefinitions, parametersLocalBuilder);

            EmitHelper.CreateInvocation(il, invocationType, targetLocalBuilder, interceptorsLocalBuilder, memberLocalBuilder, proxyMethodLocalBuilder, proxyLocalBuilder, parametersLocalBuilder, invocationLocalBuilder);
            EmitHelper.CallProceed(il, invocationLocalBuilder);

            // set ref parameters values after Proceed called
            SetByRefArgs(il, methodDefinition.ParameterDefinitions, invocationLocalBuilder);

            EmitReturnValue(il, method, invocationLocalBuilder, returnValueLocalBuilder);

            DefineMethodOverride(proxyScope.TypeBuilder, methodBuilder, method);

            return methodBuilder;
        }

        protected MethodBuilder DefineMethod(TypeBuilder typeBuilder, MethodDefinition methodDefinition)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodDefinition.Name, methodDefinition.MethodAttributes);
            methodBuilder.SetReturnType(methodDefinition.ReturnType);
            return methodBuilder;
        }

        protected void DefineParameters(MethodBuilder methodBuilder, MethodDefinition methodDefinition)
        {
            methodBuilder.SetParameters(methodDefinition.ParameterDefinitions.Select(p => p.ParameterType).ToArray());

            int index = 1;
            foreach (var parameterDefinition in methodDefinition.ParameterDefinitions)
                methodBuilder.DefineParameter(index++, parameterDefinition.Attributes, parameterDefinition.Name);
        }

        protected virtual bool ShouldAddInterceptionAttributes(MethodDefinition methodDefinition)
        {
            return methodDefinition.TypeDefinition.IsInterface && methodDefinition.InterceptorAttributes.Length > 0;
        }

        protected virtual void DefineAttributes(MethodBuilder methodBuilder, MethodDefinition methodDefinition)
        {
            if (ShouldAddInterceptionAttributes(methodDefinition))
                AttributeHelper.AddInterceptorAttributes(methodBuilder, methodDefinition.InterceptorAttributes);
        }

        protected void StoreArgsToArray(ILGenerator il, ParameterDefinition[] parameterDefinitions, LocalBuilder localBuilder)
        {
            int length = parameterDefinitions.Length;
            il.EmitLdc_I4(length);
            il.Emit(OpCodes.Newarr, typeof(object));

            foreach (var parameterDefinition in parameterDefinitions)
            {
                il.Emit(OpCodes.Dup);
                il.EmitLdc_I4(parameterDefinition.Index);
                il.EmitLdarg(parameterDefinition.Index + 1);

                if (parameterDefinition.IsByRef)
                {
                    Type elementType = parameterDefinition.ElementType;
                    il.Emit(OpCodes.Ldobj, elementType);

                    if (elementType.IsValueType || elementType.IsGenericParameter)
                        il.Emit(OpCodes.Box, elementType);
                }
                else if (parameterDefinition.ParameterType.IsValueType || parameterDefinition.ParameterType.ContainsGenericParameters)
                    il.Emit(OpCodes.Box, parameterDefinition.ParameterType);

                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Stloc, localBuilder);
        }

        protected void SetByRefArgs(ILGenerator il, ParameterDefinition[] parameterDefinitions, LocalBuilder invocationLocalBuilder)
        {
            foreach (var parameterDefinition in parameterDefinitions)
            {
                if (parameterDefinition.IsByRef)
                {
                    il.EmitLdarg(parameterDefinition.Index + 1);

                    il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
                    il.Emit(OpCodes.Call, InvocationMethods.get_ParametersMethod);
                    il.EmitLdc_I4(parameterDefinition.Index);
                    il.Emit(OpCodes.Ldelem_Ref);

                    il.EmitUnboxOrCast(parameterDefinition.ElementType);
                    il.EmitStindOrStobj(parameterDefinition.ElementType);
                }
            }
        }

        protected void EmitReturnValue(ILGenerator il, MethodInfo method, LocalBuilder invocationLocalBuilder, LocalBuilder returnValueLocalBuilder)
        {
            if (method.ReturnType != typeof(void))
            {
                Label end = il.DefineLabel();

                // return (string)invocation.ReturnValue;
                il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
                il.Emit(OpCodes.Callvirt, InvocationMethods.get_ReturnValueMethod);

                il.EmitUnboxOrCast(method.ReturnType);

                il.Emit(OpCodes.Stloc, returnValueLocalBuilder);

                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(end);

                il.Emit(OpCodes.Ldloc, returnValueLocalBuilder);
            }

            il.Emit(OpCodes.Ret);
        }

        protected void DefineMethodOverride(TypeBuilder typeBuilder, MethodBuilder methodBuilder, MethodInfo method)
        {
            if (method.DeclaringType.IsInterface)
                typeBuilder.DefineMethodOverride(methodBuilder, method);
        }
    }

}
