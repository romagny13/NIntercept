using System;
using System.Reflection;

namespace NIntercept
{
    public static class InvocationMethods
    {
        public static readonly ConstructorInfo InvocationDefaultConstructor = typeof(Invocation).GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[] { typeof(object), typeof(IInterceptor[]), typeof(MemberInfo), typeof(MethodInfo), typeof(object), typeof(object[]) }, null);
    }
}