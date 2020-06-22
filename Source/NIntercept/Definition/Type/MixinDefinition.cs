using System;

namespace NIntercept.Definition
{
    public class MixinDefinition : TypeDefinition
    {       
        private ProxyTypeDefinition proxyTypeDefinition;
        private Type[] interfaces;
        private InterfaceToImplementDefinition[] interfacesToImplement;

        public MixinDefinition(ModuleDefinition moduleDefinition, ProxyTypeDefinition proxyTypeDefinition, object mixinInstance) 
            : base(moduleDefinition, mixinInstance.GetType(), mixinInstance)
        {
            if (proxyTypeDefinition is null)
                throw new ArgumentNullException(nameof(proxyTypeDefinition));

            this.proxyTypeDefinition = proxyTypeDefinition;
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

        protected override TypeDefinition GetTypeDefinition()
        {
            return this;
        }

        protected virtual InterfaceToImplementDefinition[] GetInterfacesToImplement()
        {
            Type[] interfaces = Interfaces;
            int length = interfaces.Length;
            var typesToImplement = new InterfaceToImplementDefinition[length];
            for (int i = 0; i < length; i++)
                typesToImplement[i] = new InterfaceToImplementDefinition(ModuleDefinition, interfaces[i], Target, this);
            return typesToImplement;
        }
    }

}
