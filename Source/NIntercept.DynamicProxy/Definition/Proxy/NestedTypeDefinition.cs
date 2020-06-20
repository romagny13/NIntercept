using System;

namespace NIntercept.Definition
{
    public class NestedTypeDefinition : ProxyTypeDefinition
    {
        private ProxyTypeDefinition typeDefinition;

        public NestedTypeDefinition(ModuleDefinition moduleDefinition, Type type, object target, ProxyTypeDefinition typeDefinition)
            : base(moduleDefinition, type, target)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));

            this.typeDefinition = typeDefinition;
        }

        public ProxyTypeDefinition TypeDefinition
        {
            get { return typeDefinition; }
        }

        public override NestedTypeDefinition[] TypesToImplement
        {
            get { return null; }
        }

        protected override ProxyTypeDefinition GetTypeDefinition()
        {
            return typeDefinition;
        }
    }
}
