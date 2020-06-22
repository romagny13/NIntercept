using NIntercept.Definition;
using System.Reflection.Emit;

namespace NIntercept.Helpers
{
    public class InterceptorAttributeHelper
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
    }
}
