using NIntercept.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class ClassProxyDefinition : ProxyTypeDefinition
    {
        private static readonly ClassProxyMemberSelector DefaultMemberSelector;
        private static readonly IConstructorSelector DefaultConstructorSelector;
        private ClassProxyMemberSelector memberSelector;
        private IConstructorSelector constructorSelector;
        private ConstructorInfo constructor;

        static ClassProxyDefinition()
        {
            DefaultMemberSelector = new ClassProxyMemberSelector();
            DefaultConstructorSelector = new DefaultConstructorSelector();
        }

        public ClassProxyDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType, ProxyGeneratorOptions options)
            : base(moduleDefinition, type, targetType, options)
        {
        }

        public ClassProxyMemberSelector MemberSelector
        {
            get
            {
                if (memberSelector == null)
                {
                    if (Options != null && Options.ClassProxyMemberSelector != null)
                        memberSelector = Options.ClassProxyMemberSelector;
                    else
                        memberSelector = DefaultMemberSelector;
                }
                return memberSelector;
            }
        }

        public IConstructorSelector ConstructorSelector
        {
            get
            {
                if (constructorSelector == null)
                {
                    if (Options != null && Options.ConstructorSelector != null)
                        constructorSelector = Options.ConstructorSelector;
                    else
                        constructorSelector = DefaultConstructorSelector;
                }
                return constructorSelector;
            }
        }

        public ConstructorInfo Constructor
        {
            get
            {
                if (constructor == null)
                    constructor = ConstructorSelector.Select(Type);
                return constructor;
            }
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.ClassProxy; }
        }

        protected override PropertyDefinition[] GetProperties()
        {
            var definitions = new List<PropertyDefinition>();

            var properties = InterceptableMemberHelper.GetProperties(Type).Where(MemberSelector.IncludeProperty); // virtual
            foreach (var property in properties)
                definitions.Add(new PropertyDefinition(this, property));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
            {
                foreach (var propertyDefinition in mxinDefinition.PropertyDefinitions)
                {
                    if (definitions.FirstOrDefault(p => p.Name == propertyDefinition.Name) == null)
                        definitions.Add(propertyDefinition);
                }
            }

            return definitions.ToArray();
        }

        protected override MethodDefinition[] GetMethods()
        {
            var definitions = new List<MethodDefinition>();

            var methods = InterceptableMemberHelper.GetMethods(Type).Where(m => MemberSelector.IncludeMethod(m)); // virtual
            foreach (var method in methods)
                definitions.Add(new MethodDefinition(this, method));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
            {
                foreach (var methodDefinition in mxinDefinition.MethodDefinitions)
                {
                    if (MethodFinder.FindMethod(methods, methodDefinition.Method) == null)
                        definitions.Add(methodDefinition);
                }
            }

            return definitions.ToArray();
        }

        protected override EventDefinition[] GetEvents()
        {
            var definitions = new List<EventDefinition>();

            var events = InterceptableMemberHelper.GetEvents(Type).Where(MemberSelector.IncludeEvent); // virtual
            foreach (var @event in events)
                definitions.Add(new EventDefinition(this, @event));

            // mixins
            foreach (var mxinDefinition in MixinDefinitions)
            {
                foreach (var eventDefinition in mxinDefinition.EventDefinitions)
                {
                    if (definitions.FirstOrDefault(p => p.Name == eventDefinition.Name) == null)
                        definitions.Add(eventDefinition);
                }
            }

            return definitions.ToArray();
        }
    }
}
