using NIntercept.Builder;
using NIntercept.Definition;
using System;
using System.Collections.Generic;

namespace NIntercept
{
    public class ProxyGeneratorOptions
    {
        private List<object> mixinInstances;
        private ClassProxyMemberSelector classProxyMemberSelector;
        private IConstructorSelector constructorSelector;
        private List<CustomAttributeDefinition> additionalTypeAttributes;
        private AdditionalCode additionalCode;
        private InterceptableMethodBuilder interceptableMethodBuilder;

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

        public ClassProxyMemberSelector ClassProxyMemberSelector
        {
            get { return classProxyMemberSelector; }
            set { classProxyMemberSelector = value; }
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

        public List<CustomAttributeDefinition> AdditionalTypeAttributes
        {
            get { return additionalTypeAttributes; }
        }


        public InterceptableMethodBuilder InterceptableMethodBuilder
        {
            get { return interceptableMethodBuilder; }
            set { interceptableMethodBuilder = value; }
        }

        public void AddMixinInstance(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            if (instance.GetType().GetInterfaces().Length == 0)
                throw new ArgumentException($"Invalid mixin. The mixin '{instance.GetType().Name}' doesn't implement any interface. An interface is required.");

                this.mixinInstances.Add(instance);
        }
    }
}
