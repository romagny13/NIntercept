using NIntercept.Definition;
using System.Linq;
using System.Reflection;

namespace NIntercept
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

        public static string GetUniqueNameForInvocation(ProxyTypeDefinition typeDefinition, MethodInfo method)
        {
            return $"{typeDefinition.Name}_{GetUniqueName(method)}_Invocation";
        }
    }
}
