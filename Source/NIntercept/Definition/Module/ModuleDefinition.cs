using System;

namespace NIntercept.Definition
{

    public class ModuleDefinition
    {
        private static object locker = new object();
        private static IClassProxyMemberSelector DefaultMemberSelector;
        private TypeDefinitionCollection typeDefinitions;
        private IClassProxyMemberSelector memberSelector;

        static ModuleDefinition()
        {
            DefaultMemberSelector = new ClassProxyMemberSelector();
        }

        public ModuleDefinition()
        {
            typeDefinitions = new TypeDefinitionCollection();
        }

        public IClassProxyMemberSelector MemberSelector
        {
            get { return memberSelector ?? DefaultMemberSelector; }
            set { memberSelector = value; }
        }

        public TypeDefinitionCollection TypeDefinitions
        {
            get { return typeDefinitions; }
        }

        public virtual ProxyTypeDefinition GetOrAdd(Type type, object target, ProxyGeneratorOptions options)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            lock (locker)
            {
                ProxyTypeDefinition typeDefinition = typeDefinitions.GetByType(type, target, options);
                if (typeDefinition != null)
                {
                    if (target != null)
                        typeDefinition.Target = target;
                    if (options != null)
                        typeDefinition.Options = options;
                    return typeDefinition;
                }

                if (type.IsInterface)
                    typeDefinition = new InterfaceProxyDefinition(this, type, target, options);
                else
                    typeDefinition = new ClassProxyDefinition(this, type, target, options);

                SetMixinDefinitions(typeDefinition, options);

                typeDefinitions.Add(typeDefinition);

                return typeDefinition;
            }
        }

        protected virtual void SetMixinDefinitions(ProxyTypeDefinition typeDefinition, ProxyGeneratorOptions options)
        {
            if (options != null)
            {
                int length = options.MixinInstances.Count;
                MixinDefinition[] mixinDefinitions = new MixinDefinition[length];
                for (int i = 0; i < length; i++)
                {
                    var mixinInstance = options.MixinInstances[i];
                    mixinDefinitions[i] = new MixinDefinition(this, typeDefinition, mixinInstance);
                }
                typeDefinition.MixinDefinitions = mixinDefinitions;
            }
            else
                typeDefinition.MixinDefinitions = new MixinDefinition[0];
        }
    }
}
