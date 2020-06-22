namespace NIntercept
{
#if NET45 || NET472
    public class PersistentProxyBuilder : ProxyBuilder
    {
        public PersistentProxyBuilder(bool strongName) :
           base(new ModuleScope(true, strongName))
        { }

        public PersistentProxyBuilder() :
            base(new ModuleScope(true))
        { }

        public PersistentProxyBuilder(string assemblyName, string moduleName, bool strongName) :
          base(new ModuleScope(assemblyName, moduleName, true, strongName))
        { }

        public PersistentProxyBuilder(string assemblyName, string moduleName) :
           base(new ModuleScope(assemblyName, moduleName, true, false))
        { }
    }
#endif
}
