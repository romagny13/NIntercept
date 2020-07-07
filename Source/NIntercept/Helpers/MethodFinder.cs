using System.Collections.Generic;
using System.Reflection;

namespace NIntercept.Helpers
{
    public static class MethodFinder
    {
        public static MethodInfo FindMethod(IEnumerable<MethodInfo> methods, MethodInfo method)
        {
            foreach (var yMethod in methods)
            {
                if (Equals(method, yMethod))
                    return yMethod;
            }
            return null;
        }

        public static bool Equals(MethodInfo xMethod, MethodInfo yMethod)
        {
            if (xMethod.Name != yMethod.Name)
                return false;

            if (xMethod.DeclaringType.IsGenericType != yMethod.DeclaringType.IsGenericType)
                return false;

            if (xMethod.GetGenericArguments().Length != yMethod.GetGenericArguments().Length)
                return false;

            var xParameters = xMethod.GetParameters();
            var yParameters = yMethod.GetParameters();
            if (xParameters.Length != yParameters.Length)
                return false;

            for (int i = 0; i < xParameters.Length; i++)
            {
                var xParameter = xParameters[i];
                var yParameter = yParameters[i];

                if (xParameter.ParameterType.ContainsGenericParameters)
                    continue;

                if (xParameter.ParameterType != yParameter.ParameterType)
                    return false;
            }
            return true;
        }
    }
}
