using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class TypeBuilderHelper
    {
       
        public static GenericTypeParameterBuilder[] DefineGenericParameters(TypeBuilder typeBuilder, Type[] genericArguments)
        {
            if (genericArguments.Length > 0)
            {
                string[] genericTypeNames = genericArguments.Select(p => p.Name).ToArray(); // Name T
                GenericTypeParameterBuilder[] genericTypeParameterBuilders = typeBuilder.DefineGenericParameters(genericTypeNames);

                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    genericTypeParameterBuilders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes & ~GenericParameterAttributes.VarianceMask);
                    var interfaceConstraints = new List<Type>();
                    foreach (Type constraint in genericArguments[i].GetGenericParameterConstraints())
                    {
                        if (constraint.IsClass)
                            genericTypeParameterBuilders[i].SetBaseTypeConstraint(constraint);
                        else
                            interfaceConstraints.Add(constraint);
                    }
                    if (interfaceConstraints.Count > 0)
                        genericTypeParameterBuilders[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
                }
                return genericTypeParameterBuilders;
            }
            return null;
        }

        public static ConstructorBuilder AddConstructor(TypeBuilder typeBuilder, FieldBuilder[] fields, ConstructorInfo baseCtor)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, fields, baseCtor);
        }

        public static ConstructorBuilder AddConstructor(TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, FieldBuilder[] fields, ConstructorInfo baseCtor)
        {
            if (fields is null)
                throw new ArgumentNullException(nameof(fields));

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(attributes, callingConvention, fields.Select(f => f.FieldType).ToArray());

            var ctorIl = constructorBuilder.GetILGenerator();

            for (int i = 0; i < fields.Length; i++)
            {
                ctorIl.Emit(OpCodes.Ldarg_0);

                ctorIl.EmitLdarg(i + 1);
                ctorIl.Emit(OpCodes.Stfld, fields[i]);
            }

            if (baseCtor != null)
            {
                Type[] parameterTypes = baseCtor.GetParameters().Select(p => p.ParameterType).ToArray();
                ctorIl.Emit(OpCodes.Ldarg_0);

                for (int i = 0; i < parameterTypes.Length; i++)
                    ctorIl.EmitLdarg(i + 1);

                ctorIl.Emit(OpCodes.Call, baseCtor);
            }

            ctorIl.Emit(OpCodes.Ret);

            return constructorBuilder;
        }

        public static ConstructorBuilder AddConstructor(TypeBuilder typeBuilder, FieldBuilder[] fields)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, fields, null);
        }

        public static ConstructorBuilder AddConstructor(TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, ConstructorInfo baseCtor)
        {
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(attributes, callingConvention, parameterTypes);
            var ctorIl = constructorBuilder.GetILGenerator();

            ctorIl.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < parameterTypes.Length; i++)
                ctorIl.EmitLdarg(i + 1);


            if (baseCtor != null)
                ctorIl.Emit(OpCodes.Call, baseCtor);

            ctorIl.Emit(OpCodes.Ret);

            return constructorBuilder;
        }

        public static ConstructorBuilder AddConstructor(TypeBuilder typeBuilder, Type[] parameterTypes, ConstructorInfo baseCtor)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, parameterTypes, baseCtor);
        }

        public static void AddInterceptionAttributes(TypeBuilder typeBuilder, InterceptorAttributeDefinition[] attributes)
        {
            foreach (var customAttribute in attributes)
                AddInterceptionAttribute(typeBuilder, customAttribute.AttributeData);
        }
       
        public static void AddInterceptionAttribute(TypeBuilder typeBuilder, CustomAttributeData interceptorAttribute)
        {
            CustomAttributeBuilder customAttributeBuilder = GetCustomAttributeBuilder(interceptorAttribute);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        private static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData customAttributeData)
        {
            ConstructorInfo constructor = customAttributeData.Constructor;
            var args = customAttributeData.ConstructorArguments.Select(c => c.Value).ToArray();
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, args);
            return customAttributeBuilder;
        }

        public static void AddCustomAttribute(TypeBuilder typeBuilder, Type attributeType)
        {
            if (attributeType is null)
                throw new ArgumentNullException(nameof(attributeType));

            ConstructorInfo constructor = attributeType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException($"No constructor with empty parameters found for '{attributeType.Name}'");

            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, new object[0]);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        public static Type BuildType(TypeBuilder typeBuilder)
        {
#if NET45 || NET472
            return typeBuilder.CreateType();
#else
            return typeBuilder.CreateTypeInfo();
#endif
        }
    }
}
