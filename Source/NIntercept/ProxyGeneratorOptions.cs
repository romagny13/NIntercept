using NIntercept.Definition;
using System;
using System.Collections.Generic;

namespace NIntercept
{
    public class ProxyGeneratorOptions
    {
        private List<object> mixinInstances;
        private IClassProxyMemberSelector classProxyMemberSelector;

        public ProxyGeneratorOptions()
        {
            mixinInstances = new List<object>();
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

        public void AddMixinInstance(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            this.mixinInstances.Add(instance);
        }
    }
}
