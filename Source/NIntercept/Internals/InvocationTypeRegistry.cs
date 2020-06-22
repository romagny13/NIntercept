using System;
using System.Collections.Generic;

namespace NIntercept
{
    internal class InvocationTypeRegistry
    {
        private Dictionary<string, Type> invocationTypes;

        public InvocationTypeRegistry()
        {
            invocationTypes = new Dictionary<string, Type>();
        }

        public IReadOnlyDictionary<string, Type> InvocationTypes
        {
            get { return invocationTypes; }
        }

        protected internal void Add(string name, Type buildType)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (buildType is null)
                throw new ArgumentNullException(nameof(buildType));

            invocationTypes.Add(name, buildType);
        }

        public Type GetBuildType(string name)
        {
            Type buildType = null;
            invocationTypes.TryGetValue(name, out buildType);
            return buildType;
        }
    }
}
