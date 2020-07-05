using System;

namespace NIntercept.Definition
{

    public sealed class ModuleDefinition
    {
        private static object locker = new object();
        private TypeDefinitionCollection typeDefinitions;

        public ModuleDefinition()
        {
            typeDefinitions = new TypeDefinitionCollection();
        }

        public TypeDefinitionCollection TypeDefinitions
        {
            get { return typeDefinitions; }
        }

        public ProxyTypeDefinition GetTypeDefinition(Type type, Type targetType, ProxyGeneratorOptions options)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            lock (locker)
            {
                ProxyTypeDefinition typeDefinition = typeDefinitions.GetByType(type, targetType, options);
                if (typeDefinition != null)
                    return typeDefinition;

                if (type.IsInterface)
                    typeDefinition = new InterfaceProxyDefinition(this, type, targetType, options);
                else
                    typeDefinition = new ClassProxyDefinition(this, type, targetType, options);

                SetMixinDefinitions(typeDefinition, options);

                typeDefinitions.Add(typeDefinition);

                return typeDefinition;
            }
        }

        private void SetMixinDefinitions(ProxyTypeDefinition typeDefinition, ProxyGeneratorOptions options)
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
