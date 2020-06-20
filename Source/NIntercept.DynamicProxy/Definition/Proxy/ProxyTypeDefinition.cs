using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public class ProxyTypeDefinition
    {
        private const string DefaultProxiesNamespace = "Interception.Proxies";
        private ModuleDefinition moduleDefinition;
        private Type type;
        private object target;
        private string name;
        private string @namespace;
        private Type[] interfaces;
        private ProxyPropertyDefinition[] propertyDefinitions;
        private ProxyMethodDefinition[] methodDefinitions;
        private ProxyEventDefinition[] eventDefinitions;
        private NestedTypeDefinition[] typesToImplement;
        private InterceptorAttributeDefinition[] interceptorAttributes;

        public ProxyTypeDefinition(ModuleDefinition moduleDefinition, Type type, object target)
        {
            if (moduleDefinition is null)
                throw new ArgumentNullException(nameof(moduleDefinition));
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            this.@namespace = DefaultProxiesNamespace;
            this.moduleDefinition = moduleDefinition;
            this.type = type;
            this.target = target;
        }

        public ModuleDefinition ModuleDefinition
        {
            get { return moduleDefinition; }
        }

        public Type Type
        {
            get { return type; }
        }

        public virtual TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Public; }
        }

        public object Target
        {
            get { return target; }
            protected internal set { target = value; }
        }

        public Type TargetType
        {
            get { return target != null ? target.GetType() : null; }
        }

        public virtual string Namespace
        {
            get { return @namespace; }
            set { @namespace = value; }
        }

        public virtual string Name
        {
            get
            {
                if (name == null)
                    name = GetName();
                return name;
            }
        }

        public virtual string FullName
        {
            get { return !string.IsNullOrWhiteSpace(Namespace) ? $"{Namespace}.{Name}" : Name; }
        }

        public bool IsInterface
        {
            get { return type.IsInterface; }
        }

        public Type[] Interfaces
        {
            get
            {

                if (interfaces == null)
                    interfaces = type.GetInterfaces();
                return interfaces;
            }
        }

        protected IMemberSelector MemberSelector
        {
            get { return moduleDefinition.MemberSelector; }
        }

        public virtual ProxyPropertyDefinition[] PropertyDefinitions
        {
            get
            {
                if (propertyDefinitions == null)
                    propertyDefinitions = GetProperties();
                return propertyDefinitions;
            }
        }

        public virtual ProxyMethodDefinition[] MethodDefinitions
        {
            get
            {
                if (methodDefinitions == null)
                    methodDefinitions = GetMethods();
                return methodDefinitions;
            }
        }

        public virtual ProxyEventDefinition[] EventDefinitions
        {
            get
            {
                if (eventDefinitions == null)
                    eventDefinitions = GetEvents();
                return eventDefinitions;
            }
        }

        public virtual NestedTypeDefinition[] TypesToImplement
        {
            get
            {
                if (IsInterface && typesToImplement == null)
                    typesToImplement = GetTypesToImplement();
                return typesToImplement;
            }
        }

        public virtual InterceptorAttributeDefinition[] InterceptorAttributes
        {
            get
            {
                if (interceptorAttributes == null)
                    interceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(this, type, typeof(IInterceptorProvider));
                return interceptorAttributes;
            }
        }

        protected virtual string GetName()
        {
            int index = moduleDefinition.TypeDefinitions.IndexOf(this);
            if (index == -1 && GetTypeDefinition() == this)
                throw new ArgumentException("Unable to find this TypeDefinition. It's probably a bug. Please, open an issue to fix it.");

            if (index > 0)
                return $"{type.Name}_{index}_Proxy";

            return $"{type.Name}_Proxy";
        }

        protected virtual BindingFlags GetFlags()
        {
            return type.IsInterface ? BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                : BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }

        protected virtual ProxyTypeDefinition GetTypeDefinition()
        {
            return this;
        }

        public virtual ProxyPropertyDefinition[] GetProperties()
        {
            BindingFlags flags = GetFlags();
            ProxyTypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<PropertyInfo> properties = type.GetProperties(flags);

            if (!type.IsInterface)
                properties = properties.Where(MemberSelector.IncludeProperty); // virtual

            int length = properties.Count();
            ProxyPropertyDefinition[] propertyDefinitions = new ProxyPropertyDefinition[length];
            for (int i = 0; i < length; i++)
                propertyDefinitions[i] = new ProxyPropertyDefinition(typeDefinition, properties.ElementAt(i));

            return propertyDefinitions;
        }

        protected virtual ProxyMethodDefinition[] GetMethods()
        {
            BindingFlags flags = GetFlags();
            ProxyTypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<MethodInfo> methods = type
                .GetMethods(flags)
                .Where(m => m.DeclaringType != typeof(object) && !m.IsSpecialName);

            if (!type.IsInterface)
                methods = methods.Where(MemberSelector.IncludeMethod); // virtual

            int length = methods.Count();
            ProxyMethodDefinition[] methodDefinitions = new ProxyMethodDefinition[length];
            for (int i = 0; i < length; i++)
                methodDefinitions[i] = new ProxyMethodDefinition(typeDefinition, methods.ElementAt(i));

            return methodDefinitions;
        }

        protected virtual ProxyEventDefinition[] GetEvents()
        {
            BindingFlags flags = GetFlags();
            ProxyTypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<EventInfo> events = type.GetEvents(flags);

            if (!type.IsInterface)
                events = events.Where(MemberSelector.IncludeEvent); // virtual

            int length = events.Count();
            ProxyEventDefinition[] eventDefinitions = new ProxyEventDefinition[length];
            for (int i = 0; i < length; i++)
                eventDefinitions[i] = new ProxyEventDefinition(typeDefinition, events.ElementAt(i));

            return eventDefinitions;
        }

        protected virtual NestedTypeDefinition[] GetTypesToImplement()
        {
            Type[] interfaces = Interfaces;
            int length = interfaces.Length;
            var typesToImplement = new NestedTypeDefinition[length];
            for (int i = 0; i < length; i++)
                typesToImplement[i] = new NestedTypeDefinition(moduleDefinition, interfaces[i], target, this);
            return typesToImplement;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
