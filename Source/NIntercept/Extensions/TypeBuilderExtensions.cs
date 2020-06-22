using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class TypeBuilderExtensions
    {
        private const CallingConventions DefaultEventCallingConvention = CallingConventions.Standard | CallingConventions.HasThis;
        private const MethodAttributes DefaultEventMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.Virtual;

        public static void AddInterfacesToImplement(this TypeBuilder typeBuilder, Type[] interfaces)
        {
            foreach (var @interface in interfaces)
                typeBuilder.AddInterfaceImplementation(@interface);
        }

        public static GenericTypeParameterBuilder[] DefineGenericParameters(this TypeBuilder typeBuilder, Type[] genericArguments)
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

        public static void AddCustomAttributes(this TypeBuilder typeBuilder, CustomAttributeData[] customAttributes)
        {
            foreach (var customAttribute in customAttributes)
                AddCustomAttribute(typeBuilder, customAttribute);
        }

        public static void AddCustomAttribute(this TypeBuilder typeBuilder, CustomAttributeData customAttributeData)
        {
            CustomAttributeBuilder customAttributeBuilder = GetCustomAttributeBuilder(customAttributeData);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        private static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData customAttributeData)
        {
            ConstructorInfo constructor = customAttributeData.Constructor;
            var args = customAttributeData.ConstructorArguments.Select(p => p.Value).ToArray();
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, args);
            return customAttributeBuilder;
        }

        public static void AddCustomAttribute(this TypeBuilder typeBuilder, Type attributeType)
        {
            if (attributeType is null)
                throw new ArgumentNullException(nameof(attributeType));

            ConstructorInfo constructor = attributeType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException($"No constructor with empty parameters found for '{attributeType.Name}'");

            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, new object[0]);
            typeBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        #region ctor

        public static ConstructorBuilder AddConstructor(this TypeBuilder typeBuilder, FieldBuilder[] fields, ConstructorInfo baseCtor)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, fields, baseCtor);
        }

        public static ConstructorBuilder AddConstructor(this TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, FieldBuilder[] fields, ConstructorInfo baseCtor)
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

        public static ConstructorBuilder AddConstructor(this TypeBuilder typeBuilder, FieldBuilder[] fields)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, fields, null);
        }

        public static ConstructorBuilder AddConstructor(this TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, ConstructorInfo baseCtor)
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

        public static ConstructorBuilder AddConstructor(this TypeBuilder typeBuilder, Type[] parameterTypes, ConstructorInfo baseCtor)
        {
            return AddConstructor(typeBuilder, MethodAttributes.Public, CallingConventions.Standard, parameterTypes, baseCtor);
        }

        #endregion

        #region Property

        public static MethodBuilder DefineGetMethod(this TypeBuilder typeBuilder,
             string propertyName, MethodAttributes methodAttributes, Type returnType, FieldBuilder field)
        {
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyName}", methodAttributes, returnType, Type.EmptyTypes);

            ILGenerator il = getMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        public static MethodBuilder DefineGetMethod(this TypeBuilder typeBuilder, string propertyName, Type returnType, FieldBuilder field)
        {
            return DefineGetMethod(typeBuilder, propertyName, MethodAttributes.Public, returnType, field);
        }

        public static MethodBuilder DefineSetMethod(this TypeBuilder typeBuilder,
            string propertyName, MethodAttributes methodAttributes, Type[] parameterTypes, FieldBuilder field)
        {
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyName}", methodAttributes, null, parameterTypes);

            ILGenerator ilL = setMethodBuilder.GetILGenerator();
            ilL.Emit(OpCodes.Ldarg_0);
            ilL.Emit(OpCodes.Ldarg_1);
            ilL.Emit(OpCodes.Stfld, field);
            ilL.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        public static MethodBuilder DefineSetMethod(this TypeBuilder typeBuilder, string propertyName, Type[] parameterTypes, FieldBuilder field)
        {
            return DefineSetMethod(typeBuilder, propertyName, MethodAttributes.Public, parameterTypes, field);
        }

        public static PropertyBuilder DefineFullProperty(this TypeBuilder typeBuilder,
            FieldBuilder field, string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes, MethodAttributes methodAttributes)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (returnType is null)
                throw new ArgumentNullException(nameof(returnType));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));

            PropertyBuilder property = typeBuilder.DefineProperty(propertyName, propertyAttributes, returnType, parameterTypes);

            MethodBuilder getMethod = DefineGetMethod(typeBuilder, propertyName, methodAttributes, returnType, field);
            MethodBuilder setMethod = DefineSetMethod(typeBuilder, propertyName, methodAttributes, parameterTypes, field);

            property.SetGetMethod(getMethod);
            property.SetSetMethod(setMethod);

            return property;
        }

        public static PropertyBuilder DefineFullProperty(this TypeBuilder typeBuilder,
           FieldBuilder field, string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes)
        {
            return DefineFullProperty(typeBuilder, field, propertyName, propertyAttributes, returnType, parameterTypes, MethodAttributes.Public);
        }

        public static PropertyBuilder DefineFullProperty(this TypeBuilder typeBuilder, FieldBuilder field,
            string propertyName, Type returnType, Type[] parameterTypes)
        {
            return DefineFullProperty(typeBuilder, field, propertyName, PropertyAttributes.None, returnType, parameterTypes, MethodAttributes.Public);
        }

        public static PropertyBuilder DefineReadOnlyProperty(this TypeBuilder typeBuilder,
         FieldBuilder field, string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes, MethodAttributes methodAttributes)
        {
            if (field is null)
                throw new ArgumentNullException(nameof(field));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));
            if (returnType is null)
                throw new ArgumentNullException(nameof(returnType));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));

            PropertyBuilder property = typeBuilder.DefineProperty(propertyName, propertyAttributes, returnType, parameterTypes);

            MethodBuilder getMethod = DefineGetMethod(typeBuilder, propertyName, methodAttributes, returnType, field);

            property.SetGetMethod(getMethod);

            return property;
        }

        public static PropertyBuilder DefineReadOnlyProperty(this TypeBuilder typeBuilder,
            FieldBuilder field, string propertyName, PropertyAttributes propertyAttributes, Type returnType, Type[] parameterTypes)
        {
            return DefineReadOnlyProperty(typeBuilder, field, propertyName, propertyAttributes, returnType, parameterTypes, MethodAttributes.Public);
        }

        public static PropertyBuilder DefineReadOnlyProperty(this TypeBuilder typeBuilder,
         FieldBuilder field, string propertyName, Type returnType, Type[] parameterTypes)
        {
            return DefineReadOnlyProperty(typeBuilder, field, propertyName, PropertyAttributes.None, returnType, parameterTypes, MethodAttributes.Public);
        }

        #endregion

        #region Events

        public static MethodBuilder DefineAddMethod(this TypeBuilder typeBuilder, string eventName, MethodAttributes methodAttributes, CallingConventions callingConvention, Type eventType, FieldBuilder field)
        {
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (eventName is null)
                throw new ArgumentNullException(nameof(eventName));
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            MethodBuilder methodBuilder = typeBuilder.DefineMethod($"add_{eventName}", methodAttributes, callingConvention, typeof(void), new Type[] { eventType });

            ILGenerator il = methodBuilder.GetILGenerator();

            MethodInfo combineMethod = Methods.CombineMethod;

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, combineMethod);
            il.Emit(OpCodes.Castclass, eventType);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        public static MethodBuilder DefineAddMethod(this TypeBuilder typeBuilder, string eventName, MethodAttributes methodAttributes, Type eventType, FieldBuilder field)
        {
            return DefineAddMethod(typeBuilder, eventName, methodAttributes, DefaultEventCallingConvention, eventType, field);
        }

        public static MethodBuilder DefineAddMethod(this TypeBuilder typeBuilder, string eventName, Type eventType, FieldBuilder field)
        {
            return DefineAddMethod(typeBuilder, eventName, DefaultEventMethodAttributes, DefaultEventCallingConvention, eventType, field);
        }

        public static MethodBuilder DefineRemoveMethod(this TypeBuilder typeBuilder, string eventName, MethodAttributes methodAttributes, CallingConventions callingConvention, Type eventType, FieldBuilder field)
        {
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (eventName is null)
                throw new ArgumentNullException(nameof(eventName));
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            MethodBuilder methodBuilder = typeBuilder.DefineMethod($"remove_{eventName}", methodAttributes, callingConvention, typeof(void), new Type[] { eventType });

            ILGenerator il = methodBuilder.GetILGenerator();

            MethodInfo removeMethod = Methods.RemoveMethod;

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, removeMethod);
            il.Emit(OpCodes.Castclass, eventType);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        public static MethodBuilder DefineRemoveMethod(this TypeBuilder typeBuilder, string eventName, MethodAttributes methodAttributes, Type eventType, FieldBuilder field)
        {
            return DefineRemoveMethod(typeBuilder, eventName, methodAttributes, DefaultEventCallingConvention, eventType, field);
        }

        public static MethodBuilder DefineRemoveMethod(this TypeBuilder typeBuilder, string eventName, Type eventType, FieldBuilder field)
        {
            return DefineRemoveMethod(typeBuilder, eventName, DefaultEventMethodAttributes, DefaultEventCallingConvention, eventType, field);
        }

        public static EventBuilder DefineFullEvent(this TypeBuilder typeBuilder, string eventName,
            MethodAttributes methodAttributes, CallingConventions callingConvention, EventAttributes eventAttributes, Type eventType, FieldBuilder field)
        {
            if (typeBuilder is null)
                throw new ArgumentNullException(nameof(typeBuilder));
            if (eventName is null)
                throw new ArgumentNullException(nameof(eventName));
            if (eventType is null)
                throw new ArgumentNullException(nameof(eventType));
            if (field is null)
                throw new ArgumentNullException(nameof(field));

            EventBuilder eventBuilder = typeBuilder.DefineEvent(eventName, eventAttributes, eventType);

            MethodBuilder addMethodBuilder = DefineAddMethod(typeBuilder, eventName, methodAttributes, callingConvention, eventType, field);
            eventBuilder.SetAddOnMethod(addMethodBuilder);

            MethodBuilder removeMethodBuilder = DefineRemoveMethod(typeBuilder, eventName, methodAttributes, callingConvention, eventType, field);
            eventBuilder.SetRemoveOnMethod(removeMethodBuilder);

            return eventBuilder;
        }

        public static EventBuilder DefineFullEvent(this TypeBuilder typeBuilder, string eventName, MethodAttributes methodAttributes, EventAttributes eventAttributes, Type eventType, FieldBuilder field)
        {
            return DefineFullEvent(typeBuilder, eventName, methodAttributes, DefaultEventCallingConvention, eventAttributes, eventType, field);
        }

        public static EventBuilder DefineFullEvent(this TypeBuilder typeBuilder, string eventName, EventAttributes eventAttributes, Type eventType, FieldBuilder field)
        {
            return DefineFullEvent(typeBuilder, eventName, DefaultEventMethodAttributes, DefaultEventCallingConvention, eventAttributes, eventType, field);
        }

        #endregion

        public static Type BuildType(this TypeBuilder typeBuilder)
        {
#if NET45 || NET472
            return typeBuilder.CreateType();
#else
            return typeBuilder.CreateTypeInfo();
#endif
        }
    }
}
