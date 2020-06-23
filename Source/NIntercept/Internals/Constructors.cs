using System;
using System.Reflection;
using System.Xml.Serialization;

namespace NIntercept
{
    internal static class Constructors
    {
        public static readonly ConstructorInfo InvocationDefaultConstructor = typeof(Invocation).GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[] { typeof(object), typeof(IInterceptor[]), typeof(MemberInfo), typeof(MethodInfo), typeof(object), typeof(object[]) }, null);

        public static readonly ConstructorInfo XmlIncludeAttributeConstructor = typeof(XmlIncludeAttribute).GetConstructor(new Type[] { typeof(Type) });

        public static readonly ConstructorInfo XmlIgnoreAttributeConstructor = typeof(XmlIgnoreAttribute).GetConstructor(Type.EmptyTypes);

        public static readonly ConstructorInfo SerializableAttributeConstructor = typeof(SerializableAttribute).GetConstructor(Type.EmptyTypes);
    }
}