using NIntercept.Definition;
using System;
using System.Reflection.Emit;

namespace NIntercept.Helpers
{
    public class AttributeHelper
    {
        public static void AddInterceptorAttributes(TypeBuilder typeBuilder, InterceptorAttributeDefinition[] attributes)
        {
            foreach (var customAttribute in attributes)
                typeBuilder.AddCustomAttribute(customAttribute.AttributeData);
        }

        public static void AddInterceptorAttributes(MethodBuilder methodBuilder, InterceptorAttributeDefinition[] attributes)
        {
            foreach (var customAttribute in attributes)
                methodBuilder.AddCustomAttribute(customAttribute.AttributeData);
        }

        //public static void AddCustomAttribute(TypeBuilder typeBuilder, CustomAttributeBuilder customAttributeBuilder)
        //{
        //    typeBuilder.SetCustomAttribute(customAttributeBuilder);
        //}

        //private static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeBuilderInfo customAttributeBuilderInfo)
        //{
        //    if (customAttributeBuilderInfo is null)
        //        throw new ArgumentNullException(nameof(customAttributeBuilderInfo));

        //    CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(customAttributeBuilderInfo.Constructor, customAttributeBuilderInfo.Args);
        //    return customAttributeBuilder;
        //}
    }
}
