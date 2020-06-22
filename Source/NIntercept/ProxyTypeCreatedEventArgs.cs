using System;

namespace NIntercept
{
    public class ProxyTypeCreatedEventArgs : EventArgs
    {
        private Type type;
        private object target;
        private Type proxyType;
        private ProxyGeneratorOptions options;

        public ProxyTypeCreatedEventArgs(Type type, object target, Type proxyType, ProxyGeneratorOptions options)
        {
            this.type = type;
            this.target = target;
            this.proxyType = proxyType;
            this.options = options;
        }

        public Type Type
        {
            get { return type; }
        }

        public object Target
        {
            get { return target; }
        }

        public Type ProxyType
        {
            get { return proxyType; }
        }

        public ProxyGeneratorOptions Options
        {
            get { return options; }
        }
    }
}
