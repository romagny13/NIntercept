using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public class MethodBuilderHelper
    {
        public static MethodAttributes GetMethodAttributes(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            var attributes = MethodAttributes.ReuseSlot;

            if (method.IsVirtual)
                attributes |= MethodAttributes.Virtual;

            if (method.IsFinal || method.DeclaringType.IsInterface)
                attributes |= MethodAttributes.NewSlot;

            if (method.IsPublic)
                attributes |= MethodAttributes.Public;

            if (method.IsHideBySig)
                attributes |= MethodAttributes.HideBySig;

            // internal ?
            if (IsInternal(method))
                attributes |= MethodAttributes.Assembly;

            if (method.IsFamilyAndAssembly)
                attributes |= MethodAttributes.FamANDAssem;
            else if (method.IsFamilyOrAssembly)
                attributes |= MethodAttributes.FamORAssem;
            else if (method.IsFamily)
                attributes |= MethodAttributes.Family;

            return attributes;
        }

        public static bool IsInternal(MethodInfo method)
        {
            return method.IsAssembly || (method.IsFamilyAndAssembly && !method.IsFamilyOrAssembly);
        }

        public static bool IsInternal(PropertyInfo property)
        {
            MethodInfo method = property.CanRead ? property.GetMethod : property.SetMethod;
            return IsInternal(method);
        }

        public static GenericTypeParameterBuilder[] DefineGenericParameters(MethodBuilder methodBuilder, Type[] genericArguments)
        {
            if (genericArguments.Length > 0)
            {
                string[] genericTypeNames = genericArguments.Select(p => p.Name).ToArray(); // Name T
                GenericTypeParameterBuilder[] genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(genericTypeNames);

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

        public static void AddInterceptionAttributes(MethodBuilder methodBuilder, InterceptorAttributeDefinition[] attributes)
        {
            foreach (var customAttribute in attributes)
                AddInterceptionAttribute(methodBuilder, customAttribute.AttributeData);
        }

        public static void AddInterceptionAttribute(MethodBuilder methodBuilder, CustomAttributeData interceptorAttribute)
        {
            CustomAttributeBuilder customAttributeBuilder = GetCustomAttributeBuilder(interceptorAttribute);
            methodBuilder.SetCustomAttribute(customAttributeBuilder);
        }

        private static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData customAttributeData)
        {
            ConstructorInfo constructor = customAttributeData.Constructor;
            var args = customAttributeData.ConstructorArguments.Select(c => c.Value).ToArray();
            CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(constructor, args);
            return customAttributeBuilder;
        }
    }
}
