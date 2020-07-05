using System.Collections.Generic;

namespace NIntercept
{
    public class ProxyGeneratorOptionsComparer : IEqualityComparer<ProxyGeneratorOptions>
    {
        public bool Equals(ProxyGeneratorOptions x, ProxyGeneratorOptions y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            // AdditionalTypeAttributes
            if (x.AdditionalTypeAttributes.Count != y.AdditionalTypeAttributes.Count)
                return false;
            for (int i = 0; i < x.AdditionalTypeAttributes.Count; i++)
            {
                if (x.AdditionalTypeAttributes[i].Constructor != y.AdditionalTypeAttributes[i].Constructor)
                    return false;
            }

            // AdditionalCode
            if (x.AdditionalCode?.GetType() != y.AdditionalCode?.GetType())
                return false;

            // Mixins
            if (x.MixinInstances.Count != y.MixinInstances.Count)
                return false;
            for (int i = 0; i < x.MixinInstances.Count; i++)
            {
                if (x.MixinInstances[i].GetType() != y.MixinInstances[i].GetType())
                    return false;
            }

            // ConstructorSelector
            if (x.ConstructorSelector?.GetType() != y.ConstructorSelector?.GetType())
                return false;

            // ClassProxyMemberSelector
            if (x.ClassProxyMemberSelector?.GetType() != y.ClassProxyMemberSelector?.GetType())
                return false;

            // ServiceProvider
            if (x.ServiceProvider?.GetType() != y.ServiceProvider?.GetType())
                return false;         
          
            return true;
        }

        public int GetHashCode(ProxyGeneratorOptions obj)
        {
            return obj.GetHashCode();
        }
    }
}
