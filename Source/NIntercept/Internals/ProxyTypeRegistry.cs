using System;
using System.Collections;
using System.Collections.Generic;

namespace NIntercept
{
    internal class ProxyTypeRegistry : IEnumerable<ProxyTypeRegistryEntry>
    {
        private ProxyGeneratorOptionsComparer comparer;
        private List<ProxyTypeRegistryEntry> items;

        public ProxyTypeRegistry()
        {
            items = new List<ProxyTypeRegistryEntry>();
            comparer = new ProxyGeneratorOptionsComparer();
        }

        protected internal void Add(string name, ProxyGeneratorOptions options, Type buidType)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (buidType is null)
                throw new ArgumentNullException(nameof(buidType));
            if (Contains(name, options))
                throw new ArgumentException($"A proxyType with name {name} and the options provided is already registered");

            items.Add(new ProxyTypeRegistryEntry(name, options, buidType));
        }

        public Type GetBuidType(string name, ProxyGeneratorOptions options)
        {
            foreach (var item in items)
            {
                if(item.Name == name)
                {
                    if (comparer.Equals(item.Options, options))
                        return item.BuildType;
                }
            }
            return null;
        }

        public bool Contains(string name, ProxyGeneratorOptions options)
        {
            return GetBuidType(name, options) != null;
        }

        public IEnumerator<ProxyTypeRegistryEntry> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
