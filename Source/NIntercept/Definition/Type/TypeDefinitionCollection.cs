using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept.Definition
{
    public class TypeDefinitionCollection : IEnumerable<ProxyTypeDefinition>
    {
        private List<ProxyTypeDefinition> items;
        private ProxyGeneratorOptionsComparer comparer;

        public TypeDefinitionCollection()
        {
            items = new List<ProxyTypeDefinition>();
            comparer = new ProxyGeneratorOptionsComparer();
        }

        public ProxyTypeDefinition this[int index]
        {
            get { return items[index]; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        private bool Equals(ProxyTypeDefinition typeDefinition, Type type, Type targetType, ProxyGeneratorOptions options)
        {
            if (typeDefinition.Type != type)
                return false;

            if (typeDefinition.TargetType != targetType)
                return false;

            return comparer.Equals(typeDefinition.Options, options);
        }

        public ProxyTypeDefinition GetByType(Type type, Type targetType, ProxyGeneratorOptions options)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            foreach (var item in items)
            {
                if (Equals(item, type, targetType, options))
                    return item;
            }
            return null;
        }

        public bool ContainsByType(Type type, Type targetType, ProxyGeneratorOptions options)
        {
            return GetByType(type, targetType, options) != null;
        }

        internal void Add(ProxyTypeDefinition typeDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (ContainsByType(typeDefinition.Type, typeDefinition.TargetType, typeDefinition.Options))
                throw new InvalidOperationException($"A type '{typeDefinition.Type.Name}' and options provided is already registered.");

            this.items.Add(typeDefinition);
        }

        internal int IndexOf(ProxyTypeDefinition typeDefinition)
        {
            bool isInterface = typeDefinition.IsInterface;
            // filter by type
            var candidates = items.Where(p => p.Type == typeDefinition.Type).ToArray();

            for (int i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (candidate.TargetType != typeDefinition.TargetType)
                    continue;

                if (comparer.Equals(candidate.Options, typeDefinition.Options))
                    return i;
            }
            return -1;
        }

        internal void Clear()
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
