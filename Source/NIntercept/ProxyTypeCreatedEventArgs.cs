using System;

namespace NIntercept
{
    public class ProxyTypeCreatedEventArgs : EventArgs
    {
        private Type type;
        private object target;
        private Type proxyType;

        public ProxyTypeCreatedEventArgs(Type type, object target, Type proxyType)
        {
            this.type = type;
            this.target = target;
            this.proxyType = proxyType;
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
    }
}
