using System.Reflection;

namespace NIntercept
{
    internal static class InvocationMethods
    {
        public static readonly MethodInfo get_ParametersMethod = typeof(Invocation).GetMethod("get_Parameters");

        public static readonly MethodInfo get_ProxyMethod = typeof(Invocation).GetMethod("get_Proxy");

        public static readonly MethodInfo get_InterceptorsMethod = typeof(Invocation).GetMethod("get_Interceptors", BindingFlags.NonPublic | BindingFlags.Instance);

        public static readonly MethodInfo get_ReturnValueMethod = typeof(Invocation).GetMethod("get_ReturnValue");

        public static readonly MethodInfo set_ReturnValueMethod = typeof(Invocation).GetMethod("set_ReturnValue");

        public static readonly MethodInfo ProceedMethod = typeof(Invocation).GetMethod("Proceed");
    }
}
