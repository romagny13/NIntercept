using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyGeneratorOptions
    {
        private List<object> mixinInstances;
        private IClassProxyMemberSelector classProxyMemberSelector;
        private List<CustomAttributeBuilder> additionalTypeAttributes;
        private IConstructorSelector constructorSelector;
        private IConstructorInjectionResolver constructorInjectionResolver;

        public ProxyGeneratorOptions()
        {
            mixinInstances = new List<object>();
            additionalTypeAttributes = new List<CustomAttributeBuilder>();
        }

        public IConstructorSelector ConstructorSelector
        {
            get { return constructorSelector; }
            set { constructorSelector = value; }
        }

        public IConstructorInjectionResolver ConstructorInjectionResolver
        {
            get { return constructorInjectionResolver ; }
            set { constructorInjectionResolver = value; }
        }

        protected internal List<object> MixinInstances
        {
            get { return mixinInstances; }
        }

        public IClassProxyMemberSelector ClassProxyMemberSelector
        {
            get { return classProxyMemberSelector; }
            set { classProxyMemberSelector = value; }
        }

        public List<CustomAttributeBuilder> AdditionalTypeAttributes
        {
            get { return additionalTypeAttributes; }
        }

        public void AddMixinInstance(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            this.mixinInstances.Add(instance);
        }
    }
}
