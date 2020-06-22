using System;
using System.Reflection;

namespace NIntercept.Definition
{
    public class InterfaceProxyDefinition : ProxyTypeDefinition
    {
        private Type[] interfaces;
        private InterfaceToImplementDefinition[] interfacesToImplement;

        public InterfaceProxyDefinition(ModuleDefinition moduleDefinition, Type type, object target, ProxyGeneratorOptions options) 
            : base(moduleDefinition, type, target, options)
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
