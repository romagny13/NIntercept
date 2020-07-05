using NIntercept.Helpers;
using System;

namespace NIntercept.Definition
{
    public sealed class MixinDefinition : TypeDefinition
    {
        private ProxyTypeDefinition proxyTypeDefinition;
        private object mixinInstance;
        private Type[] interfaces;
        private InterfaceToImplementDefinition[] interfacesToImplement;
        private string targetFieldName;

        public MixinDefinition(ModuleDefinition moduleDefinition, ProxyTypeDefinition proxyTypeDefinition, object mixinInstance)
            : base(moduleDefinition, mixinInstance.GetType(), mixinInstance.GetType())
        {
            if (proxyTypeDefinition is null)
                throw new ArgumentNullException(nameof(proxyTypeDefinition));

            this.proxyTypeDefinition = proxyTypeDefinition;
            this.mixinInstance = mixinInstance;
        }

        public object MixinInstance
        {
            get { return mixinInstance; }
        }

        public ProxyTypeDefinition ProxyTypeDefinition
        {
            get { return proxyTypeDefinition; }
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.Mixin; }
        }

        public override string FullName
        {
            get { return Name; }
        }

        public Type[] Interfaces
        {
            get
            {
                if (interfaces == null)
                    interfaces = Type.GetInterfaces();
                return interfaces;
            }
        }

        public InterfaceToImplementDefinition[] InterfacesToImplement
        {
            get
            {
                if (interfacesToImplement == null)
                    interfacesToImplement = GetInterfacesToImplement();
                return interfacesToImplement;
            }
        }

        public override string TargetFieldName
        {
            get
            {
                if (targetFieldName == null)
                    targetFieldName = $"_{NamingHelper.ToCamelCase(Name)}";
                return targetFieldName;
            }
        }

        protected override TypeDefinition GetTypeDefinition()
        {
            return this;
        }

        private InterfaceToImplementDefinition[] GetInterfacesToImplement()
        {
            Type[] interfaces = Interfaces;
            int length = interfaces.Length;
            var typesToImplement = new InterfaceToImplementDefinition[length];
            for (int i = 0; i < length; i++)
                typesToImplement[i] = new InterfaceToImplementDefinition(ModuleDefinition, interfaces[i], TargetType, this);
            return typesToImplement;
        }

    }

}
