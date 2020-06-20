using System;

namespace NIntercept.Definition
{

    public class ModuleDefinition
    {
        private static IMemberSelector DefaultMemberSelector;
        private static object locker = new object();
        private TypeDefinitionCollection typeDefinitions;
        private IMemberSelector memberSelector;

        static ModuleDefinition()
        {
            DefaultMemberSelector = new MemberSelector();
        }

        public ModuleDefinition()
        {
            typeDefinitions = new TypeDefinitionCollection();
        }

        public IMemberSelector MemberSelector
        {
            get { return memberSelector ?? DefaultMemberSelector; }
            set { memberSelector = value; }
        }

        public TypeDefinitionCollection TypeDefinitions
        {
            get { return typeDefinitions; }
        }

        public virtual ProxyTypeDefinition GetOrAdd(Type type, object target)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            lock (locker)
            {
                ProxyTypeDefinition typeDefinition = typeDefinitions.GetByType(type, target);
                if (typeDefinition != null)
                    return typeDefinition;

                typeDefinition = new ProxyTypeDefinition(this, type, target);

                typeDefinitions.Add(typeDefinition);

                return typeDefinition;
            }
        }
    }
}
