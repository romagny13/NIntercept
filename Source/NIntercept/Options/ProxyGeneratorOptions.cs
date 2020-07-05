using NIntercept.Definition;
using System;
using System.Collections.Generic;

namespace NIntercept
{
    public class ProxyGeneratorOptions
    {
        private List<object> mixinInstances;
        private IClassProxyMemberSelector classProxyMemberSelector;
        private List<CustomAttributeDefinition> additionalTypeAttributes;
        private IConstructorSelector constructorSelector;
        private AdditionalCode additionalCode;
        private IProxyServiceProvider serviceProvider;

        public ProxyGeneratorOptions()
        {
            mixinInstances = new List<object>();
            additionalTypeAttributes = new List<CustomAttributeDefinition>();
        }

        public IConstructorSelector ConstructorSelector
        {
            get { return constructorSelector; }
            set { constructorSelector = value; }
        }

        public AdditionalCode AdditionalCode
        {
            get { return additionalCode; }
            set { additionalCode = value; }
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

        public List<CustomAttributeDefinition> AdditionalTypeAttributes
        {
            get { return additionalTypeAttributes; }
        }

        public IProxyServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set { serviceProvider = value; }
        }

        public void AddMixinInstance(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            this.mixinInstances.Add(instance);
        }
    }
}
