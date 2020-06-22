using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept
{
    public class MethodAccessorCollection : IEnumerable<MethodAccessor>
    {
        private readonly MethodAccessor[] items;

        public MethodAccessorCollection(MethodAccessor[] items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            this.items = items;
        }

        public MethodAccessor this[int index]
        {
            get { return items[index]; }
        }

        public MethodAccessor[] this[string name]
        {
            get { return GetByName(name); }
        }

        public int Count
        {
            get { return items.Length; }
        }

        public MethodAccessor[] GetByName(string name)
        {
            return this.items.Where(p => p.Method.Name == name).ToArray();
        }

        public MethodAccessor GetFirstOrDefault(string name)
        {
            return GetByName(name).FirstOrDefault();
        }

        public bool ContainsByName(string name)
        {
            return GetByName(name).Length > 0;
        }

        public IEnumerator<MethodAccessor> GetEnumerator()
        {
            return items.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
