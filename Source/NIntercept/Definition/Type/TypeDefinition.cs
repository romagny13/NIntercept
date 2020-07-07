using NIntercept.Helpers;
using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public abstract class TypeDefinition
    {
        private const string DefaultTargetFieldName = "_target";
        private ModuleDefinition moduleDefinition;
        private Type type;
        private Type targetType;
        private string name;
        private PropertyDefinition[] propertyDefinitions;
        private MethodDefinition[] methodDefinitions;
        private EventDefinition[] eventDefinitions;
        private InterceptorAttributeDefinition[] interceptorAttributes;

        protected TypeDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType)
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

        public Type TargetType
        {
            get { return targetType; }
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

        public virtual string TargetFieldName
        {
            get { return DefaultTargetFieldName; }
        }

        public bool IsInterface
        {
            get { return type.IsInterface; }
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

        public abstract TypeDefinitionType TypeDefinitionType { get; }

        protected virtual string GetName()
        {
            return type.Name;
        }

        public override string ToString()
        {
            return FullName;
        }

        protected abstract PropertyDefinition[] GetProperties();
        protected abstract MethodDefinition[] GetMethods();
        protected abstract EventDefinition[] GetEvents();
    }
}
