using NIntercept.Helpers;
using System;

namespace NIntercept.Definition
{
    public sealed class MixinDefinition : TypeDefinition
    {
        private Type[] interfaces;
        private ProxyTypeDefinition proxyTypeDefinition;
        private object mixinInstance;
        private string targetFieldName;

        public MixinDefinition(ModuleDefinition moduleDefinition, ProxyTypeDefinition proxyTypeDefinition, object mixinInstance)
            : base(moduleDefinition, mixinInstance.GetType(), mixinInstance.GetType())
        {
            if (proxyTypeDefinition is null)
                throw new ArgumentNullException(nameof(proxyTypeDefinition));

            this.proxyTypeDefinition = proxyTypeDefinition;
            this.mixinInstance = mixinInstance;
        }

        public object MixinInstance
        {
            get { return mixinInstance; }
        }

        public ProxyTypeDefinition ProxyTypeDefinition
        {
            get { return proxyTypeDefinition; }
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.Mixin; }
        }

        public override string FullName
        {
            get { return Name; }
        }

        public Type[] Interfaces
        {
            get
            {
                if (interfaces == null)
                    interfaces = Type.GetInterfaces();
                return interfaces;
            }
        }

        public override string TargetFieldName
        {
            get
            {
                if (targetFieldName == null)
                    targetFieldName = $"__{NamingHelper.ToCamelCase(Name)}";
                return targetFieldName;
            }
        }

        protected override PropertyDefinition[] GetProperties()
        {
            var properties = InterceptableMemberHelper.GetInterceptableProperties(Type, Interfaces);
            int length = properties.Count;
            PropertyDefinition[] propertyDefinitions = new PropertyDefinition[length];
            for (int i = 0; i < length; i++)
                propertyDefinitions[i] = new PropertyDefinition(this, properties[i]);
            return propertyDefinitions;
        }

        protected override MethodDefinition[] GetMethods()
        {
            var methods = InterceptableMemberHelper.GetInterceptableMethods(Type, Interfaces);
            int length = methods.Count;
            MethodDefinition[] methodDefinitions = new MethodDefinition[length];
            for (int i = 0; i < length; i++)
                methodDefinitions[i] = new MethodDefinition(this, methods[i]);
            return methodDefinitions;
        }

        protected override EventDefinition[] GetEvents()
        {
            var events = InterceptableMemberHelper.GetInterceptableEvents(Type, Interfaces);
            int length = events.Count;
            EventDefinition[] eventDefinitions = new EventDefinition[length];
            for (int i = 0; i < length; i++)
                eventDefinitions[i] = new EventDefinition(this, events[i]);
            return eventDefinitions;
        }
    }
}
