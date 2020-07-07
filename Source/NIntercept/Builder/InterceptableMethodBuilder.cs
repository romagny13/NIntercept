using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Builder
{
    public class InterceptableMethodBuilder
    {
        private readonly CallbackMethodBuilder callbackMethodBuilder;

        public InterceptableMethodBuilder()
        {
            this.callbackMethodBuilder = new CallbackMethodBuilder();
        }

        public virtual MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder memberField, FieldBuilder callerMethodField)
        {
            MethodInfo method = methodDefinition.Method;

            // callback method
            MethodBuilder callbackMethodBuilder = CreateCallbackMethod(proxyScope, methodDefinition.CallbackMethodDefinition);

            // invocation type
            Type invocationType = GetOrCreateInvocationType(proxyScope, methodDefinition, callbackMethodBuilder);

            // method
            MethodBuilder methodBuilder = proxyScope.DefineMethod(methodDefinition.Name, methodDefinition.MethodAttributes);
            methodBuilder.SetReturnType(methodDefinition.ReturnType);

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
            Type localBuilderType = targetType != null ? targetType : typeof(object);
            LocalBuilder targetLocalBuilder = il.DeclareLocal(localBuilderType); // target
            LocalBuilder interceptorsLocalBuilder = il.DeclareLocal(typeof(IInterceptor[])); // interceptors
            LocalBuilder memberLocalBuilder = il.DeclareLocal(typeof(MemberInfo)); // MemberInfo
            LocalBuilder callerMethodLocalBuilder = il.DeclareLocal(typeof(MethodInfo)); // proxy method
            LocalBuilder proxyLocalBuilder = il.DeclareLocal(typeof(object)); // proxy
            LocalBuilder parametersLocalBuilder = il.DeclareLocal(typeof(object[])); // parameters
            LocalBuilder invocationLocalBuilder = il.DeclareLocal(invocationType); // invocation
            LocalBuilder returnValueLocalBuilder = null;
            if (returnType != typeof(void))
                returnValueLocalBuilder = il.DeclareLocal(returnType);

            FieldBuilder interceptorsField = proxyScope.ConstructorFields[0];
            if (targetType != null)
            {
                FieldBuilder targetField = FindConstructorField(proxyScope, methodDefinition.TypeDefinition.TargetFieldName);
                EmitHelper.StoreFieldToLocal(il, targetField, targetLocalBuilder);
            }
            EmitHelper.StoreFieldToLocal(il, interceptorsField, interceptorsLocalBuilder);
            EmitHelper.StoreFieldToLocal(il, memberField, memberLocalBuilder);
            EmitHelper.StoreFieldToLocal(il, callerMethodField, callerMethodLocalBuilder);
            EmitHelper.StoreThisToLocal(il, proxyLocalBuilder);
            StoreArgsToArray(il, methodDefinition.ParameterDefinitions, parametersLocalBuilder);

            EmitHelper.CreateInvocation(il, invocationType, targetLocalBuilder, interceptorsLocalBuilder, memberLocalBuilder, callerMethodLocalBuilder, proxyLocalBuilder, parametersLocalBuilder, invocationLocalBuilder);
            EmitHelper.CallProceed(il, invocationLocalBuilder);

            // set ref parameters values after Proceed called
            SetByRefArgs(il, methodDefinition.ParameterDefinitions, invocationLocalBuilder);

            EmitReturnValue(il, method, invocationLocalBuilder, returnValueLocalBuilder);

            if (method.DeclaringType.IsInterface)
                proxyScope.DefineMethodOverride(methodBuilder, method);

            return methodBuilder;
        }

        protected FieldBuilder FindConstructorField(ProxyScope proxyScope, string fieldName)
        {
            return proxyScope.ConstructorFields.First(p => p.Name == fieldName);
        }

        protected Type GetOrCreateInvocationType(ProxyScope proxyScope, MethodDefinition methodDefinition, MethodBuilder callbackMethodBuilder)
        {
            Type invocationType = proxyScope.ModuleScope.GetOrCreateInvocationType(methodDefinition.InvocationTypeDefinition, callbackMethodBuilder);
            if (invocationType.IsGenericType)
                invocationType = invocationType.MakeGenericType(methodDefinition.Method.GetGenericArguments());
            return invocationType;
        }

        protected MethodBuilder CreateCallbackMethod(ProxyScope proxyScope, CallbackMethodDefinition callbackMethodDefinition)
        {
            return callbackMethodBuilder.CreateMethod(proxyScope, callbackMethodDefinition);
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
    }

}
