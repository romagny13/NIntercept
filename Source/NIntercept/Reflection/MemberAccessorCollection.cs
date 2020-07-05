using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept.Reflection
{
    public class MemberAccessorCollection<T> : IEnumerable<T> where T : IMemberAccessor
    {
        private readonly T[] items;

        public MemberAccessorCollection(T[] items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            this.items = items;
        }

        public T this[string name]
        {
            get { return GetByName(name); }
        }

        public T this[int index]
        {
            get { return items.ElementAt(index); }
        }

        public int Count
        {
            get { return items.Length; }
        }

        public T GetByName(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return items.FirstOrDefault(p => p.Name == name);
        }

        public bool ContainsByName(string name)
        {
            return GetByName(name) != null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
