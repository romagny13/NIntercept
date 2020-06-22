using System.Collections.Generic;

namespace NIntercept
{
    internal class ProxyGeneratorOptionsComparer : IEqualityComparer<ProxyGeneratorOptions>
    {
        public bool Equals(ProxyGeneratorOptions x, ProxyGeneratorOptions y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            int xCount = x.MixinInstances.Count;
            int yCount = y.MixinInstances.Count;

            if (xCount == 0 && yCount == 0)
                return true;

            if (xCount != yCount)
                return false;

            for (int i = 0; i < xCount; i++)
            {
                var xMixin = x.MixinInstances[i];
                var yMixin = y.MixinInstances[i];
                if (xMixin.GetType() != yMixin.GetType())
                    return false;
            }

            return true;
        }

        public int GetHashCode(ProxyGeneratorOptions obj)
        {
            return obj.GetHashCode();
        }
    }
}
