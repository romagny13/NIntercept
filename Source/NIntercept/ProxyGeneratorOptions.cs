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
    
        public ProxyGeneratorOptions()
        {
            mixinInstances = new List<object>();
            additionalTypeAttributes = new List<CustomAttributeBuilder>();
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
