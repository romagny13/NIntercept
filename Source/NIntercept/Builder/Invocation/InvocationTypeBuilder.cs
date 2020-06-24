using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class InvocationTypeBuilder : IInvocationTypeBuilder
    {
        public virtual Type CreateType(ModuleBuilder moduleBuilder, TypeBuilder proxyTypeBuilder, InvocationTypeDefinition invocationTypeDefinition, MethodBuilder callbackMethodBuilder)
        {
            TypeBuilder typeBuilder = DefineType(moduleBuilder, invocationTypeDefinition);

            GenericTypeParameterBuilder[] genericTypeParameters = typeBuilder.DefineGenericParameters(invocationTypeDefinition.GenericArguments);

            DefineConstructors(typeBuilder, invocationTypeDefinition);

            // implement InterceptorProviderType
            ImplementInterceptorProviderTypeProperty(typeBuilder, invocationTypeDefinition.MethodDefinition);

            // method InvokeMethodOnTarget
            InvokeMethodOnTargetDefinition methodOnTargetDefinition = invocationTypeDefinition.MethodOnTargetDefinition;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodOnTargetDefinition.Name, methodOnTargetDefinition.Attributes);

            var il = methodBuilder.GetILGenerator();

            Type returnType = methodOnTargetDefinition.ReturnType;
            var method = methodOnTargetDefinition.Method;

            // locals
            var refLocals = new Dictionary<int, LocalBuilder>();
            LocalBuilder proxyLocalBuilder = il.DeclareLocal(typeof(object));
            LocalBuilder resultLocalBuilder = null;
            if (returnType != typeof(void))
            {
                if (returnType.ContainsGenericParameters)
                    resultLocalBuilder = il.DeclareLocal(genericTypeParameters.First(p => p.Name == returnType.Name));
                else
                    resultLocalBuilder = il.DeclareLocal(returnType);
            }

            // Call proxy callback method
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            // il.Emit(OpCodes.Castclass, proxyTypeBuilder); // Cast?
            il.Emit(OpCodes.Call, InvocationMethods.get_ProxyMethod);
            il.Emit(OpCodes.Stloc, proxyLocalBuilder);
            il.Emit(OpCodes.Ldloc, proxyLocalBuilder);

            SetArgsWithParameters(il, methodOnTargetDefinition, genericTypeParameters, refLocals);

            CallInvokeMethodOnTarget(il, method, callbackMethodBuilder, genericTypeParameters);
            SetParametersWithRefs(il, refLocals);
            SetInvocationReturnValue(il, method, genericTypeParameters, returnType, resultLocalBuilder);

            il.Emit(OpCodes.Ret);

            return typeBuilder.BuildType();
        }

        protected PropertyBuilder ImplementInterceptorProviderTypeProperty(TypeBuilder typeBuilder, MethodDefinition methodDefinition)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("InterceptorProviderType",
                PropertyAttributes.None, typeof(Type), Type.EmptyTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_InterceptorProviderType",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(Type), Type.EmptyTypes);

            var il = getMethodBuilder.GetILGenerator();

            LocalBuilder interceptorTypeLocalBuilder = il.DeclareLocal(typeof(Type));
            var label = il.DefineLabel();

            var interceptorProviderType = GetInterceptorProviderType(methodDefinition);

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldtoken, interceptorProviderType);

            MethodInfo getTypeFromHandle = Methods.GetTypeFromHandle;
            il.Emit(OpCodes.Call, getTypeFromHandle);
            il.Emit(OpCodes.Stloc, interceptorTypeLocalBuilder);
            il.Emit(OpCodes.Br_S, label);
            il.MarkLabel(label);
            il.Emit(OpCodes.Ldloc, interceptorTypeLocalBuilder);
            il.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);

            return propertyBuilder;
        }

        protected Type GetInterceptorProviderType(MethodDefinition methodDefinition)
        {
            switch (methodDefinition)
            {
                case PropertyGetMethodDefinition _:
                    return typeof(IPropertyGetInterceptorProvider);
                case PropertySetMethodDefinition _:
                    return typeof(IPropertySetInterceptorProvider);
                case AddEventMethodDefinition _:
                    return typeof(IAddEventInterceptorProvider);
                case RemoveEventMethodDefinition _:
                    return typeof(IRemoveEventInterceptorProvider);
                default:
                    return typeof(IMethodInterceptorProvider);
            }
        }

        protected virtual TypeBuilder DefineType(ModuleBuilder moduleBuilder, InvocationTypeDefinition invocationTypeDefinition)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(invocationTypeDefinition.FullName, invocationTypeDefinition.Attributes);
            typeBuilder.SetParent(typeof(Invocation));
            return typeBuilder;
        }

        protected virtual void DefineConstructors(TypeBuilder typeBuilder, InvocationTypeDefinition invocationTypeDefinition)
        {
            var target = invocationTypeDefinition.Target;
            var targetType = target != null ? target.GetType() : typeof(object);
            typeBuilder.AddConstructor(new Type[] { targetType, typeof(IInterceptor[]), typeof(MemberInfo), typeof(MethodInfo), typeof(object), typeof(object[]) }, Constructors.InvocationDefaultConstructor);
        }

        protected void SetArgsWithParameters(ILGenerator il, InvokeMethodOnTargetDefinition invokeMethodOnTargetDefinition,
            GenericTypeParameterBuilder[] genericTypeParameters, Dictionary<int, LocalBuilder> refLocals)
        {
            foreach (var parameterDefinition in invokeMethodOnTargetDefinition.ParameterDefinitions)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, InvocationMethods.get_ParametersMethod);
                il.EmitLdc_I4(parameterDefinition.Index);
                il.Emit(OpCodes.Ldelem_Ref);

                if (parameterDefinition.IsByRef)
                {
                    LocalBuilder refLocalBuilder = il.DeclareLocal(parameterDefinition.ElementType);
                    refLocals.Add(parameterDefinition.Index, refLocalBuilder);
                    il.EmitUnboxOrCast(parameterDefinition.ElementType);
                    il.Emit(OpCodes.Stloc, refLocalBuilder);
                    il.Emit(OpCodes.Ldloca, refLocalBuilder);
                }
                else if (parameterDefinition.ParameterType.ContainsGenericParameters)
                    il.Emit(OpCodes.Unbox_Any, genericTypeParameters.First(p => p.Name == parameterDefinition.ParameterType.Name));
                else
                    il.EmitUnboxOrCast(parameterDefinition.ParameterType);
            }
        }

        protected virtual void CallInvokeMethodOnTarget(ILGenerator il, MethodInfo method, MethodBuilder callbackMethodBuilder, GenericTypeParameterBuilder[] genericTypeParameters)
        {
            // invoke a call back method on proxy
            if (method.IsGenericMethod)
                il.EmitCall(OpCodes.Call, callbackMethodBuilder.MakeGenericMethod(genericTypeParameters), null);
            else
                il.EmitCall(OpCodes.Call, callbackMethodBuilder, null);
        }

        protected void SetParametersWithRefs(ILGenerator il, Dictionary<int, LocalBuilder> refLocals)
        {
            foreach (var refLocal in refLocals)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, InvocationMethods.get_ParametersMethod);
                il.EmitLdc_I4(refLocal.Key);
                il.Emit(OpCodes.Ldloc, refLocal.Value);
                if (refLocal.Value.LocalType.IsValueType)
                    il.Emit(OpCodes.Box, refLocal.Value.LocalType);
                il.Emit(OpCodes.Stelem_Ref);
            }
        }

        protected void SetInvocationReturnValue(ILGenerator il, MethodInfo method, GenericTypeParameterBuilder[] genericTypeParameters, Type returnType, LocalBuilder resultLocalBuilder)
        {
            if (returnType != typeof(void))
            {
                il.Emit(OpCodes.Stloc, resultLocalBuilder);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc, resultLocalBuilder);

                if (returnType.ContainsGenericParameters)
                    il.Emit(OpCodes.Box, genericTypeParameters.First(p => p.Name == returnType.Name));
                else if (method.ReturnType.IsValueType)
                    il.Emit(OpCodes.Box, returnType);

                il.EmitCall(OpCodes.Call, InvocationMethods.set_ReturnValueMethod, null);
            }
        }
    }
}
