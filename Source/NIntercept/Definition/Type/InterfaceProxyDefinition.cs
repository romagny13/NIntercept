using System;
using System.Collections.Generic;

namespace NIntercept.Definition
{
    public sealed class InterfaceProxyDefinition : ProxyTypeDefinition
    {
        private Type[] interfaces;

        public InterfaceProxyDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType, ProxyGeneratorOptions options)
            : base(moduleDefinition, type, targetType, options)
        {
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

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.InterfaceProxy; }
        }

        protected override PropertyDefinition[] GetProperties()
        {
            var definitions = new List<PropertyDefinition>();

            var properties = InterceptableMemberHelper.GetInterceptableProperties(Type, Interfaces);
            foreach (var property in properties)
                definitions.Add(new PropertyDefinition(this, property));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
                definitions.AddRange(mxinDefinition.PropertyDefinitions);

            return definitions.ToArray();
        }

        protected override MethodDefinition[] GetMethods()
        {
            var definitions = new List<MethodDefinition>();

            var methods = InterceptableMemberHelper.GetInterceptableMethods(Type, Interfaces);
            foreach (var method in methods)
                definitions.Add(new MethodDefinition(this, method));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
                definitions.AddRange(mxinDefinition.MethodDefinitions);

            return definitions.ToArray();
        }

        protected override EventDefinition[] GetEvents()
        {
            var definitions = new List<EventDefinition>();

            var events = InterceptableMemberHelper.GetInterceptableEvents(Type, Interfaces);
            foreach (var @event in events)
                definitions.Add(new EventDefinition(this, @event));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
                definitions.AddRange(mxinDefinition.EventDefinitions);

            return definitions.ToArray();
        }
    }
}
