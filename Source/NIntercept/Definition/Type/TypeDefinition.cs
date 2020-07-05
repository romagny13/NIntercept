using NIntercept.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public abstract class TypeDefinition
    {
        private const string DefaultTargetFieldName = "_target";
        private Type type;
        private Type targetType;
        private ModuleDefinition moduleDefinition;
        private PropertyDefinition[] propertyDefinitions;
        private MethodDefinition[] methodDefinitions;
        private EventDefinition[] eventDefinitions;
        private InterceptorAttributeDefinition[] interceptorAttributes;
        private string name;

        public TypeDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType)
        {
            if (moduleDefinition is null)
                throw new ArgumentNullException(nameof(moduleDefinition));
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            this.moduleDefinition = moduleDefinition;
            this.type = type;
            this.targetType = targetType;
        }

        public ModuleDefinition ModuleDefinition
        {
            get { return moduleDefinition; }
        }

        public Type Type
        {
            get { return type; }
        }

        public virtual string TargetFieldName
        {
            get { return DefaultTargetFieldName; }
        }

        public Type TargetType
        {
            get { return targetType; }
        }

        public bool IsInterface
        {
            get { return type.IsInterface; }
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
            get { return Name; }
        }

        public virtual TypeAttributes TypeAttributes
        {
            get { return TypeAttributes.Public; }
        }

        public virtual PropertyDefinition[] PropertyDefinitions
        {
            get
            {
                if (propertyDefinitions == null)
                    propertyDefinitions = GetProperties();
                return propertyDefinitions;
            }
        }

        public virtual MethodDefinition[] MethodDefinitions
        {
            get
            {
                if (methodDefinitions == null)
                    methodDefinitions = GetMethods();
                return methodDefinitions;
            }
        }

        public virtual EventDefinition[] EventDefinitions
        {
            get
            {
                if (eventDefinitions == null)
                    eventDefinitions = GetEvents();
                return eventDefinitions;
            }
        }

        public virtual InterceptorAttributeDefinition[] InterceptorAttributes
        {
            get
            {
                if (interceptorAttributes == null)
                    interceptorAttributes = AttributeDefinitionHelper.GetInterceptorDefinitions(type, typeof(IInterceptorProvider));
                return interceptorAttributes;
            }
        }

        protected virtual string GetName()
        {
            return type.Name;
        }

        protected virtual BindingFlags GetFlags()
        {
            return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }

        public abstract TypeDefinitionType TypeDefinitionType { get; }

        protected abstract TypeDefinition GetTypeDefinition();

        protected virtual PropertyDefinition[] GetProperties()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<PropertyInfo> properties = type.GetProperties(flags);

            int length = properties.Count();
            PropertyDefinition[] propertyDefinitions = new PropertyDefinition[length];
            for (int i = 0; i < length; i++)
                propertyDefinitions[i] = new PropertyDefinition(typeDefinition, properties.ElementAt(i));

            return propertyDefinitions;
        }

        protected virtual MethodDefinition[] GetMethods()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<MethodInfo> methods = type
                .GetMethods(flags)
                .Where(m => m.DeclaringType != typeof(object) && !m.IsSpecialName);

            int length = methods.Count();
            MethodDefinition[] methodDefinitions = new MethodDefinition[length];
            for (int i = 0; i < length; i++)
                methodDefinitions[i] = new MethodDefinition(typeDefinition, methods.ElementAt(i));

            return methodDefinitions;
        }

        protected virtual EventDefinition[] GetEvents()
        {
            BindingFlags flags = GetFlags();
            TypeDefinition typeDefinition = GetTypeDefinition();
            IEnumerable<EventInfo> events = type.GetEvents(flags);

            int length = events.Count();
            EventDefinition[] eventDefinitions = new EventDefinition[length];
            for (int i = 0; i < length; i++)
                eventDefinitions[i] = new EventDefinition(typeDefinition, events.ElementAt(i));

            return eventDefinitions;
        }

        public override string ToString()
        {
            return FullName;
        }
    }

}
