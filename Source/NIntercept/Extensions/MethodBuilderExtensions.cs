using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{
    public static class MethodBuilderExtensions
    {
        public static GenericTypeParameterBuilder[] DefineGenericParameters(this MethodBuilder methodBuilder, Type[] genericArguments)
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

        public static void AddCustomAttribute(this MethodBuilder methodBuilder, CustomAttributeData interceptorAttribute)
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
