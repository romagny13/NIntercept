using MvvmSample.ViewModels;
using MvvmSample.Views;
using NIntercept;
using System;
using System.Collections.Generic;
using System.Windows;
using Unity;

namespace MvvmSample
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper(new UnityContainer());
            bootstrapper.Run();
        }
    }

    public abstract class BootstrapperBase
    {
        private Window shell;

        protected Window Shell
        {
            get { return shell; }
        }

        protected virtual void ConfigureContainerLocator()
        {

        }

        protected virtual void BeforeCreatingShell()
        {

        }

        protected virtual void ConfigureContainer()
        {

        }

        protected virtual void ConfigureViewModelLocator()
        {

        }

        protected abstract void RegisterTypes();

        protected virtual void RegisterRequiredTypes()
        {
            
        }

        protected virtual void FinalizeContainerRegistration()
        {
           
        }

        protected abstract Window CreateShell();

        protected virtual void InitializeShell(Window shell)
        {
            if (Application.Current != null)
                Application.Current.MainWindow = shell;
        }

        protected virtual void ShowShell()
        {
            shell.Show();
        }

        public virtual void OnInitialized()
        {

        }

        public virtual void Run()
        {
            ConfigureViewModelLocator();

            ConfigureContainer();
            RegisterTypes();
            RegisterRequiredTypes();
            ConfigureContainerLocator();
            FinalizeContainerRegistration();

            BeforeCreatingShell();
            shell = CreateShell();
            if (shell != null)
            {
                InitializeShell(shell);
                ShowShell();
            }

            OnInitialized();
        }
    }

    public class Bootstrapper : BootstrapperBase
    {
        IUnityContainer container;
        private DefaultLocator locator;

        public Bootstrapper(IUnityContainer container)
        {
            this.container = container;
            this.locator = new DefaultLocator(container);
        }

        public IUnityContainer Container { get => container; }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterInstance<IContainerLocator>(locator);
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetViewModelFactory(type => locator.Resolve(type));
        }

        protected override void ConfigureContainerLocator()
        {

            ContainerLocator.SetLocatorProvider(() => locator);
        }
    }

    public interface IContainerLocator : IServiceProvider
    {
        object Resolve(Type serviceType);
        TService Resolve<TService>();
    }

    public static class ContainerLocator
    {
        private static Lazy<IContainerLocator> lazyContainerLocatorProvider;

        private static IContainerLocator current;

        public static IContainerLocator Current
        {
            get
            {
                if (current == null)
                {
                    if (lazyContainerLocatorProvider == null)
                        throw new InvalidOperationException("No containerLocatorProvider provided.");

                    current = lazyContainerLocatorProvider.Value;
                }
                return current;
            }
        }

        public static void SetLocatorProvider(Func<IContainerLocator> containerLocatorProvider)
        {
            if (containerLocatorProvider is null)
                throw new ArgumentNullException(nameof(containerLocatorProvider));

            ContainerLocator.lazyContainerLocatorProvider = new Lazy<IContainerLocator>(containerLocatorProvider);
        }
    }

    public class DefaultLocator : IContainerLocator
    {
        private IUnityContainer container;
        private Dictionary<Type, object> proxies;
        private ProxyGenerator generator;

        public DefaultLocator(IUnityContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            this.proxies = new Dictionary<Type, object>();
            this.container = container;
            this.generator = new ProxyGenerator();

            Initialize();
        }

        protected virtual void Initialize()
        {
            proxies.Add(typeof(MainWindowViewModel), generator.CreateClassProxy<MainWindowViewModel>());
        }

        public object GetService(Type serviceType)
        {
            return Resolve(serviceType);
        }

        public virtual object Resolve(Type serviceType)
        {
            if (serviceType is null)
                throw new ArgumentNullException(nameof(serviceType));

            if(proxies.TryGetValue(serviceType, out object service))
                return service;

            return container.Resolve(serviceType);
        }

        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }
    }
}
