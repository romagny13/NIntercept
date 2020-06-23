using System;

namespace NIntercept
{
    public static class ProxyServiceLocator
    {
        private static readonly IProxyServiceProvider DefaultServiceProvider;
        private static Lazy<IProxyServiceProvider> lazyLocatorProvider;

        private static IProxyServiceProvider current;

        static ProxyServiceLocator()
        {
            DefaultServiceProvider = new DefaultServiceProvider();
        }

        public static IProxyServiceProvider Current
        {
            get
            {
                if (current == null)
                {
                    if (lazyLocatorProvider == null)
                        SetLocatorProvider(() => DefaultServiceProvider);

                    current = lazyLocatorProvider.Value;
                }
                return current;
            }
        }

        public static void SetLocatorProvider(Func<IProxyServiceProvider> locatorProvider)
        {
            if (locatorProvider is null)
                throw new ArgumentNullException(nameof(locatorProvider));

            current = null;
            ProxyServiceLocator.lazyLocatorProvider = new Lazy<IProxyServiceProvider>(locatorProvider);
        }
    }
}
