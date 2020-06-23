using NIntercept.Definition;
using NIntercept.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyMethodBuilder : IProxyMethodBuilder
    {
        public virtual IInvocationTypeBuilder InvocationTypeBuilder
        {
            get { return ProxyServiceLocator.Current.InvocationTypeBuilder; }
        }

        public virtual ICallbackMethodBuilder CallbackMethodBuilder
        {
            get { return ProxyServiceLocator.Current.CallbackMethodBuilder; }
        }

        public virtual MethodBuilder CreateMethod(ModuleScope moduleScope, TypeBuilder proxyTypeBuilder,
            MethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields)
        {
            ModuleBuilder moduleBuilder = moduleScope.Module;

            MethodBuilder callbackMethodBuilder = CreateCallbackMethod(moduleBuilder, proxyTypeBuilder, methodDefinition, fields);

            Type invocationType = GetOrCreateInvocationType(moduleScope, proxyTypeBuilder, methodDefinition, moduleBuilder, callbackMethodBuilder);

            if (invocationType.IsGenericType)
                invocationType = MakeGenericType(invocationType, methodDefinition.Method);

            MethodBuilder methodBuilder = DefineMethod(proxyTypeBuilder, methodDefinition);

            methodBuilder.SetReturnType(methodDefinition.ReturnType);

            // parameters
            methodBuilder.DefineGenericParameters(methodDefinition.GenericArguments);
            if (methodDefinition.ParameterDefinitions.Length > 0)
                DefineParameters(methodBuilder, methodDefinition);

            if (ShouldAddInterceptionAttributes(methodDefinition))
                AttributeHelper.AddInterceptorAttributes(methodBuilder, methodDefinition.InterceptorAttributes);

            // method body
            var il = methodBuilder.GetILGenerator();

            // locals
            var returnType = methodDefinition.ReturnType;
            var method = methodDefinition.Method;
            var target = methodDefinition.TypeDefinition.Target;
            Type targetType = target != null ? target.GetType() : typeof(object);
            LocalBuilder targetLocalBuilder = il.DeclareLocal(targetType); // target
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
            FieldBuilder interceptorsField = fields.First();

            // Store fields in locals
            if (target != null)
                EmitHelper.StoreFieldInLocal(il, fields.ElementAt(1), targetLocalBuilder);
            EmitHelper.StoreFieldInLocal(il, interceptorsField, interceptorsLocalBuilder);
            EmitHelper.StoreMemberToLocal(il, member, memberLocalBuilder);
            EmitHelper.StoreProxyMethodToLocal(il, proxyMethodLocalBuilder);
            EmitHelper.StoreThisInLocal(il, proxyLocalBuilder);
            StoreArgsToArray(il, methodDefinition.ParameterDefinitions, parametersLocalBuilder);

            EmitHelper.CreateInvocation(il, invocationType, targetLocalBuilder, interceptorsLocalBuilder, memberLocalBuilder, proxyMethodLocalBuilder, proxyLocalBuilder, parametersLocalBuilder, invocationLocalBuilder);
            EmitHelper.CallProceed(il, invocationType, invocationLocalBuilder);

            // set ref parameters values after Proceed called
            SetByRefArgs(il, methodDefinition.ParameterDefinitions, invocationType, invocationLocalBuilder);

            EmitReturnValue(il, method, invocationLocalBuilder, returnValueLocalBuilder);

            DefineMethodOverride(proxyTypeBuilder, methodBuilder, method);

            return methodBuilder;
        }

        protected Type GetOrCreateInvocationType(ModuleScope moduleScope, TypeBuilder proxyTypeBuilder, MethodDefinition methodDefinition, ModuleBuilder moduleBuilder, MethodBuilder callbackMethodBuilder)
        {
            Type invocationType = moduleScope.InvocationTypeRegistry.GetBuildType(methodDefinition.InvocationTypeDefinition.Name);
            if (invocationType == null)
            {
                invocationType = InvocationTypeBuilder.CreateType(moduleBuilder, proxyTypeBuilder, methodDefinition.InvocationTypeDefinition, callbackMethodBuilder);
                moduleScope.InvocationTypeRegistry.Add(methodDefinition.InvocationTypeDefinition.Name, invocationType);
            }
            return invocationType;
        }

        protected MethodBuilder CreateCallbackMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, MethodDefinition methodDefinition, FieldBuilder[] fields)
        {
            return CallbackMethodBuilder.CreateMethod(moduleBuilder, typeBuilder, methodDefinition.MethodCallbackDefinition, fields);
        }

        protected Type MakeGenericType(Type type, MethodInfo method)
        {
            return type.MakeGenericType(method.GetGenericArguments());
        }

        protected virtual MethodBuilder DefineMethod(TypeBuilder typeBuilder, MethodDefinition methodDefinition)
        {
            MethodInfo method = methodDefinition.Method;
            if (methodDefinition.TypeDefinition.IsInterface)
                return typeBuilder.DefineMethod(methodDefinition.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
            else
                return typeBuilder.DefineMethod(methodDefinition.Name, MethodInfoHelper.GetMethodAttributes(method));
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

        protected void SetByRefArgs(ILGenerator il, ParameterDefinition[] parameterDefinitions, Type invocationType, LocalBuilder invocationLocalBuilder)
        {
            foreach (var parameterDefinition in parameterDefinitions)
            {
                if (parameterDefinition.IsByRef)
                {
                    il.EmitLdarg(parameterDefinition.Index + 1);

                    il.Emit(OpCodes.Ldloc, invocationLocalBuilder);
                    il.Emit(OpCodes.Call, invocationType.GetMethod("get_Parameters"));
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
                il.Emit(OpCodes.Callvirt, typeof(Invocation).GetMethod("get_ReturnValue"));

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
