using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NIntercept
{
    public class ReflectionCache
    {
        private ConcurrentDictionary<Type, FieldInfo[]> fields;
        private ConcurrentDictionary<Type, ConstructorInfo[]> constructors;
        private ConcurrentDictionary<Type, PropertyInfo[]> properties;
        private ConcurrentDictionary<Type, MethodInfo[]> methods;
        private ConcurrentDictionary<Type, EventInfo[]> events;
        private ConcurrentDictionary<Type, Attribute[]> customAttributesOnTypes;
        private ConcurrentDictionary<MemberInfo, Attribute[]> customAttributesOnMembers;
        private static ReflectionCache @default;

        public virtual BindingFlags Flags
        {
            get { return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }

        public static ReflectionCache Default
        {
            get { return @default; }
        }

        static ReflectionCache()
        {
            @default = new ReflectionCache();
        }

        public ReflectionCache()
        {
            fields = new ConcurrentDictionary<Type, FieldInfo[]>();
            constructors = new ConcurrentDictionary<Type, ConstructorInfo[]>();
            properties = new ConcurrentDictionary<Type, PropertyInfo[]>();
            methods = new ConcurrentDictionary<Type, MethodInfo[]>();
            events = new ConcurrentDictionary<Type, EventInfo[]>();
            customAttributesOnTypes = new ConcurrentDictionary<Type, Attribute[]>();
            customAttributesOnMembers = new ConcurrentDictionary<MemberInfo, Attribute[]>();
        }

        public virtual FieldInfo[] GetFields(Type type)
        {
            return fields.GetOrAdd(type, AddFieldsToCache);
        }

        public virtual ConstructorInfo[] GetConstructors(Type type)
        {
            return constructors.GetOrAdd(type, AddContructorsToCache);
        }

        public virtual PropertyInfo[] GetProperties(Type type)
        {
            return properties.GetOrAdd(type, AddPropertiesToCache);
        }

        public virtual MethodInfo[] GetMethods(Type type)
        {
            return methods.GetOrAdd(type, AddMethodsToCache);
        }

        public virtual EventInfo[] GetEvents(Type type)
        {
            return events.GetOrAdd(type, AddEventsToCache);
        }

        public virtual Attribute[] GetCustomAttributes(Type type)
        {
            return customAttributesOnTypes.GetOrAdd(type, AddCustomAttributesOnTypesToCache);
        }

        private FieldInfo[] AddFieldsToCache(Type type)
        {
            return type.GetFields(Flags).Where(m => m.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) == null).ToArray();
        }

        private ConstructorInfo[] AddContructorsToCache(Type type)
        {
            return type.GetConstructors(Flags);
        }

        private Attribute[] AddCustomAttributesOnTypesToCache(Type type)
        {
            return type.GetCustomAttributes().ToArray();
        }

        public virtual Attribute[] GetCustomAttributes(MemberInfo member)
        {
            return customAttributesOnMembers.GetOrAdd(member, AddCustomAttributesOnMembersToCache);
        }

        protected virtual Attribute[] AddCustomAttributesOnMembersToCache(MemberInfo member)
        {
            return member.GetCustomAttributes().ToArray();
        }

        protected virtual PropertyInfo[] AddPropertiesToCache(Type type)
        {
            return type.GetProperties(Flags).ToArray();
        }

        protected virtual MethodInfo[] AddMethodsToCache(Type type)
        {
            return type.GetMethods(Flags).Where(m => m.DeclaringType != typeof(object) && !m.IsSpecialName).ToArray();
        }

        protected virtual EventInfo[] AddEventsToCache(Type type)
        {
            return type.GetEvents(Flags).ToArray();
        }

        public bool ContainsType(Type type)
        {
            return methods.ContainsKey(type);
        }

        public virtual void ClearCache()
        {
            fields.Clear();
            constructors.Clear();
            properties.Clear();
            methods.Clear();
            events.Clear();
            customAttributesOnTypes.Clear();
            customAttributesOnMembers.Clear();
        }
    }
}
