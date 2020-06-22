using System;

namespace NIntercept
{
    internal class ProxyTypeRegistryEntry
    {
        private readonly string name;
        private readonly ProxyGeneratorOptions options;
        private readonly Type buildType;

        public ProxyTypeRegistryEntry(string name, ProxyGeneratorOptions options, Type buidType)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (buidType is null)
                throw new ArgumentNullException(nameof(buidType));

            this.name = name;
            this.options = options;
            this.buildType = buidType;
        }

        public string Name
        {
            get { return name; }
        }

        public ProxyGeneratorOptions Options
        {
            get { return options; }
        }

        public Type BuildType
        {
            get { return buildType; }
        }
    }
}
