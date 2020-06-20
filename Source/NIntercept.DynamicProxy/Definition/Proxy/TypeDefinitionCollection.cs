using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept.Definition
{
    public class TypeDefinitionCollection : IEnumerable<ProxyTypeDefinition>
    {
        private List<ProxyTypeDefinition> items;

        public TypeDefinitionCollection()
        {
            items = new List<ProxyTypeDefinition>();
        }

        public ProxyTypeDefinition GetByType(Type type, object target)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return this.FirstOrDefault(p => p.Type == type && p.Target == target);
        }

        public bool ContainsByType(Type type, object target)
        {
            return GetByType(type, target) != null;
        }

        protected internal void Add(ProxyTypeDefinition typeDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (ContainsByType(typeDefinition.Type, typeDefinition.Target))
                throw new InvalidOperationException($"A type '{typeDefinition.Type.Name}' with the target '{typeDefinition.Target?.GetType().Name}' is already registered.");

            this.items.Add(typeDefinition);
        }

        protected internal int IndexOf(ProxyTypeDefinition typeDefinition)
        {
            var typeDefinitions = this.Where(p => p.Type == typeDefinition.Type);
            int index = 0;
            foreach (var typeDef in typeDefinitions)
            {
                if (typeDef.Target == typeDefinition.Target)
                    return index;
                index++;
            }
            return -1;
        }

        protected internal void Clear()
        {
            items.Clear();
        }

        public IEnumerator<ProxyTypeDefinition> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
