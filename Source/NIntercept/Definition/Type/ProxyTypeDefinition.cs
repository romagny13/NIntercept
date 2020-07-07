using System;

namespace NIntercept.Definition
{
    public abstract class ProxyTypeDefinition : TypeDefinition
    {
        private const string ProxiesNamespace = "NIntercept.Proxies";
        private ProxyGeneratorOptions options;
        private MixinDefinition[] mixinDefinitions;

        public ProxyTypeDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType, ProxyGeneratorOptions options)
            : base(moduleDefinition, type, targetType)
        {
            this.options = options;
        }

        public ProxyGeneratorOptions Options
        {
            get { return options; }
        }

        public MixinDefinition[] MixinDefinitions
        {
            get { return mixinDefinitions; }
            protected internal set { mixinDefinitions = value; }
        }

        public string Namespace
        {
            get { return ProxiesNamespace; }
        }

        public override string FullName
        {
            get { return !string.IsNullOrWhiteSpace(Namespace) ? $"{Namespace}.{Name}" : Name; }
        }

        protected override string GetName()
        {
            int index = ModuleDefinition.TypeDefinitions.IndexOf(this);
            if (index == -1)
                throw new ArgumentException("Unable to find this TypeDefinition. It's probably a bug. Please, open an issue to fix it.");

            if (index > 0)
                return $"{Type.Name}_{index}_Proxy";

            return $"{Type.Name}_Proxy";
        }
    }
}
