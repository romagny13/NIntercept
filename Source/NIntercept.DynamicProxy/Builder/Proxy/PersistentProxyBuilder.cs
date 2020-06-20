namespace NIntercept
{
#if NET45 || NET472
    public class PersistentProxyBuilder : ProxyBuilder
    {
        public PersistentProxyBuilder(bool strongName) :
           base(new ProxyModuleBuilder(true, strongName))
        { }

        public PersistentProxyBuilder() :
            base(new ProxyModuleBuilder(true))
        { }

        public PersistentProxyBuilder(string assemblyName, string moduleName, bool strongName) :
          base(new ProxyModuleBuilder(assemblyName, moduleName, true, strongName))
        { }

        public PersistentProxyBuilder(string assemblyName, string moduleName) :
           base(new ProxyModuleBuilder(assemblyName, moduleName, true, false))
        { }
    }
#endif
}
