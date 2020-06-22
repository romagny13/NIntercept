using NIntercept.Definition;
using System.Linq;
using System.Reflection;

namespace NIntercept.Helpers
{
    public static class NamingHelper
    {
        public static string GetUniqueName(MethodInfo method)
        {
            // MyClass_Method_Invocation | MyClass_Method_1_Invocation
            MethodInfo[] methods = ReflectionCache.Default.GetMethods(method.DeclaringType).Where(m => m.Name == method.Name).ToArray();

            int index = -1;
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i] == method)
                {
                    index = i;
                    break;
                }
            }

            if (index > 0)
                return $"{method.Name}_{index}";

            return $"{method.Name}";
        }

        public static string GetUniqueNameForInvocation(TypeDefinition typeDefinition, MethodInfo method)
        {
            return $"{typeDefinition.Name}_{GetUniqueName(method)}_Invocation";
        }

        public static string ToCamelCase(string value)
        {
            if (char.IsLower(value, 0))
                return value;

            var array = value.ToCharArray();
            array[0] = char.ToLowerInvariant(array[0]);
            return new string(array);
        }
    }
}
