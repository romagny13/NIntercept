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

        public ProxyTypeDefinition GetByType(Type type, object target, ProxyGeneratorOptions options)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsInterface)
                return this.FirstOrDefault(p => p.Type == type && comparer.Equals(p.Options, options));
            else
                // same type, same target type or null, same options or null
                return this.FirstOrDefault(p => p.Type == type && p.Target?.GetType() == target?.GetType() && comparer.Equals(p.Options, options));
        }

        public bool ContainsByType(Type type, object target, ProxyGeneratorOptions options)
        {
            return GetByType(type, target, options) != null;
        }

        protected internal void Add(ProxyTypeDefinition typeDefinition)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (ContainsByType(typeDefinition.Type, typeDefinition.Target, typeDefinition.Options))
                throw new InvalidOperationException($"A type '{typeDefinition.Type.Name}' and options provided is already registered.");

            this.items.Add(typeDefinition);
        }

        protected internal int IndexOf(ProxyTypeDefinition typeDefinition)
        {
            bool isInterface = typeDefinition.IsInterface;
            var candidates = items.Where(p => p.Type == typeDefinition.Type).ToArray();

            for (int i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];

                if (!isInterface && candidate.Target?.GetType() != typeDefinition.Target?.GetType())
                    continue;

                if (comparer.Equals(candidate.Options, typeDefinition.Options))
                    return i;
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

    //public class TypeDefinitionCollection : IEnumerable<ProxyTypeDefinition>
    //{
    //    private List<ProxyTypeDefinition> items;
    //    private ProxyGeneratorOptionsComparer comparer;

    //    public TypeDefinitionCollection()
    //    {
    //        items = new List<ProxyTypeDefinition>();
    //        comparer = new ProxyGeneratorOptionsComparer();
    //    }

    //    public ProxyTypeDefinition this[int index]
    //    {
    //        get { return items[index]; }
    //    }

    //    public int Count
    //    {
    //        get { return items.Count; }
    //    }

    //    public ProxyTypeDefinition GetByType(Type type, object target, ProxyGeneratorOptions options)
    //    {
    //        if (type is null)
    //            throw new ArgumentNullException(nameof(type));

    //        if (type.IsInterface)
    //            return this.FirstOrDefault(p => p.Type == type && p.Target?.GetType() == target?.GetType() && comparer.Equals(p.Options, options));
    //        else
    //        {
    //            return this.FirstOrDefault(p => p.Type == type && comparer.Equals(p.Options, options));
    //        }
    //    }

    //    public bool ContainsByType(Type type, object target, ProxyGeneratorOptions options)
    //    {
    //        return GetByType(type, target, options) != null;
    //    }

    //    protected internal void Add(ProxyTypeDefinition typeDefinition)
    //    {
    //        if (typeDefinition is null)
    //            throw new ArgumentNullException(nameof(typeDefinition));
    //        if (ContainsByType(typeDefinition.Type, typeDefinition.Target, typeDefinition.Options))
    //            throw new InvalidOperationException($"A type '{typeDefinition.Type.Name}' and options provided is already registered.");

    //        this.items.Add(typeDefinition);
    //    }

    //    protected internal int IndexOf(ProxyTypeDefinition typeDefinition)
    //    {
    //        return items.IndexOf(typeDefinition);
    //    }

    //    protected internal void Clear()
    //    {
    //        items.Clear();
    //    }

    //    public IEnumerator<ProxyTypeDefinition> GetEnumerator()
    //    {
    //        return items.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}
}
