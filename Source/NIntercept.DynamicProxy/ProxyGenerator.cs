using NIntercept.Definition;
using System;
using System.Reflection.Emit;

namespace NIntercept
{
    public class ProxyGenerator : IProxyGenerator
    {
        private static readonly ModuleDefinition DefaultModuleDefinition;
        private IProxyBuilder proxyBuilder;
        private ModuleDefinition moduleDefinition;

        static ProxyGenerator()
        {
            DefaultModuleDefinition = new ModuleDefinition();
        }

        public ProxyGenerator(IProxyBuilder proxyBuilder)
        {
            if (proxyBuilder is null)
                throw new ArgumentNullException(nameof(proxyBuilder));
            this.proxyBuilder = proxyBuilder;
        }

        public ProxyGenerator()
            : this(new ProxyBuilder())
        { }

        public virtual IProxyBuilder ProxyBuilder
        {
            get { return proxyBuilder; }
        }

        public virtual ModuleDefinition ModuleDefinition
        {
            get { return moduleDefinition ?? DefaultModuleDefinition; }
            set { moduleDefinition = value; }
        }

        public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), interceptors);
        }

        public object CreateClassProxy(Type parentType, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(parentType, interceptors);
        }

        public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxyWithTarget(typeof(TClass), target, interceptors);
        }

        public object CreateClassProxyWithTarget(object target, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxyWithTarget(target?.GetType(), target, interceptors);
        }

        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)ProtectedCreateInterfaceProxyWithTarget(typeof(TInterface), target, interceptors);
        }

        public object CreateInterfaceProxyWithTarget(Type interfaceType, object target, IInterceptor[] interceptors)
        {
            return ProtectedCreateInterfaceProxyWithTarget(interfaceType, target, interceptors);
        }

        protected virtual object ProtectedCreateClassProxy(Type parentType, IInterceptor[] interceptors)
        {
            if (parentType is null)
                throw new ArgumentNullException(nameof(parentType));

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(parentType, null);
            return CreateProxy(typeDefinition, interceptors);
        }

        public virtual ProxyTypeDefinition GetTypeDefinition(Type type, object target)
        {
            return ModuleDefinition.GetOrAdd(type, target);
        }

        public virtual object CreateProxy(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));

            Type proxyType = CreateProxyType(typeDefinition, interceptors);

            object target = typeDefinition.Target;
            object[] parameters = target != null ? new object[] { interceptors, target } : new object[] { interceptors };
            return CreateProxyInstance(proxyType, parameters);
        }

        public virtual Type CreateProxyType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));

            TypeBuilder typeBuilder = ProxyBuilder.CreateType(typeDefinition, interceptors);
            return TypeBuilderHelper.BuildType(typeBuilder);
        }

        protected virtual object ProtectedCreateClassProxyWithTarget(Type targetType, object target, IInterceptor[] interceptors)
        {
            if (targetType is null)
                throw new ArgumentNullException(nameof(targetType));
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (targetType.IsInterface)
                throw new InvalidOperationException($"Interface unexpected for the method CreateClassProxyWithTarget. Type '{targetType.Name}'");

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(target.GetType(), target);
            return CreateProxy(typeDefinition, interceptors);
        }

        protected virtual object ProtectedCreateInterfaceProxyWithTarget(Type interfaceType, object target, IInterceptor[] interceptors)
        {
            if (interfaceType is null)
                throw new ArgumentNullException(nameof(interfaceType));
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"Interface expected for '{interfaceType.Name}'");

            if (!interfaceType.IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException($"The target '{target.GetType().Name}' doesn't implement the interface '{interfaceType.Name}'");

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(interfaceType, target);
            return CreateProxy(typeDefinition, interceptors);
        }

        protected virtual object CreateProxyInstance(Type proxyType, object[] parameters)
        {
            if (proxyType is null)
                throw new ArgumentNullException(nameof(proxyType));

            try
            {
                return Activator.CreateInstance(proxyType, parameters);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to create an instance for '{proxyType.Name}'", ex);
            }
        }
    }
}
