using System;
using System.Reflection;

namespace NIntercept
{
    internal static class Methods
    {
        public static readonly MethodInfo CombineMethod = typeof(Delegate).GetMethod("Combine", new Type[] { typeof(Delegate), typeof(Delegate) });

        public static readonly MethodInfo RemoveMethod = typeof(Delegate).GetMethod("Remove", new Type[] { typeof(Delegate), typeof(Delegate) });

        public static readonly MethodInfo GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        public static readonly MethodInfo GetCurrentMethod = typeof(MethodBase).GetMethod("GetCurrentMethod", new Type[0]);

        public static readonly MethodInfo GetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(RuntimeTypeHandle) }, null);

        public static readonly MethodInfo GetProperty = typeof(Type).GetMethod("GetProperty", new Type[] { typeof(string), typeof(BindingFlags) });

        public static readonly MethodInfo GetEvent = typeof(Type).GetMethod("GetEvent", new Type[] { typeof(string), typeof(BindingFlags) });
    }
}
