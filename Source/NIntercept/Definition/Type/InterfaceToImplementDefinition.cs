using System;
using System.Reflection;

namespace NIntercept.Definition
{

    public class InterfaceToImplementDefinition : TypeDefinition
    {
        private TypeDefinition parentTypeDefinition;

        public InterfaceToImplementDefinition(ModuleDefinition moduleDefinition, Type type, object target, TypeDefinition parentTypeDefinition)
            : base(moduleDefinition, type, target)
        {
            if (parentTypeDefinition is null)
                throw new ArgumentNullException(nameof(parentTypeDefinition));

            // InterfaceProxyDefinition or MixinDefinition
            this.parentTypeDefinition = parentTypeDefinition;
        }

        public TypeDefinition ParentTypeDefinition
        {
            get { return parentTypeDefinition; }
        }

        public override TypeDefinitionType TypeDefinitionType
        {
            get { return TypeDefinitionType.InterfaceToImplement; }
        }

        protected override TypeDefinition GetTypeDefinition()
        {
            return parentTypeDefinition;
        }

        protected override BindingFlags GetFlags()
        {
            return BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
    }
}
