using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public sealed class InterfaceProxyDefinition : ProxyTypeDefinition
    {
        private Type[] interfaces;
        private InterfaceToImplementDefinition[] interfacesToImplement;

        public InterfaceProxyDefinition(ModuleDefinition moduleDefinition, Type type, Type targetType, ProxyGeneratorOptions options) 
            : base(moduleDefinition, type, targetType, options)
        {
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.InterfaceProxy; }
        }

        public MethodAttributes MethodAttributes
        {
            get { return MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual; }
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

        protected override BindingFlags GetFlags()
        {
            return BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
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
