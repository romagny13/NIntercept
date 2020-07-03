using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept
{

    public class ProxyGenerator : IProxyGenerator
    {
        private readonly ModuleDefinition defaultModuleDefinition;        
        private readonly IConstructorInjectionResolver defaultConstructorInjectionResolver;
        private IProxyBuilder proxyBuilder;
        private ModuleDefinition moduleDefinition;

        public ProxyGenerator(IProxyBuilder proxyBuilder)
        {
            if (proxyBuilder is null)
                throw new ArgumentNullException(nameof(proxyBuilder));

            this.defaultModuleDefinition = new ModuleDefinition();
            this.defaultConstructorInjectionResolver = new DefaultConstructorInjectionResolver();
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
            get { return moduleDefinition ?? defaultModuleDefinition; }
            set { moduleDefinition = value; }
        }


        #region CreateClassProxy

        public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), null, interceptors);
        }

        public object CreateClassProxy(Type parentType, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(parentType, null, interceptors);
        }

        public TClass CreateClassProxy<TClass>(ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), options, interceptors);
        }

        public object CreateClassProxy(Type parentType, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(parentType, options, interceptors);
        }

        protected virtual object ProtectedCreateClassProxy(Type parentType, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (parentType is null)
                throw new ArgumentNullException(nameof(parentType));

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(parentType, null, options);
            return CreateProxy(typeDefinition, options, interceptors);
        }

        #endregion

        #region CreateClassProxyWithTarget

        public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxyWithTarget(typeof(TClass), target, null, interceptors);
        }

        public object CreateClassProxyWithTarget(object target, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxyWithTarget(target?.GetType(), target, null, interceptors);
        }

        public TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxyWithTarget(typeof(TClass), target, options, interceptors);
        }

        public object CreateClassProxyWithTarget(object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxyWithTarget(target?.GetType(), target, options, interceptors);
        }

        protected virtual object ProtectedCreateClassProxyWithTarget(Type targetType, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (targetType is null)
                throw new ArgumentNullException(nameof(targetType));
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (targetType.IsInterface)
                throw new InvalidOperationException($"Interface unexpected for the method CreateClassProxyWithTarget. Type '{targetType.Name}'");

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(target.GetType(), target, options);
            return CreateProxy(typeDefinition, options, interceptors);
        }

        #endregion

        #region CreateInterfaceWithTarget

        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)ProtectedCreateInterfaceProxy(typeof(TInterface), target, null, interceptors);
        }

        public object CreateInterfaceProxyWithTarget(Type interfaceType, object target, IInterceptor[] interceptors)
        {
            return ProtectedCreateInterfaceProxy(interfaceType, target, null, interceptors);
        }

        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)ProtectedCreateInterfaceProxy(typeof(TInterface), target, options, interceptors);
        }

        public object CreateInterfaceProxyWithTarget(Type interfaceType, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateInterfaceProxy(interfaceType, target, options, interceptors);
        }

        #endregion

        #region CreateInterfaceWithoutTarget

        public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)ProtectedCreateInterfaceProxy(typeof(TInterface), null, null, interceptors);
        }

        public object CreateInterfaceProxyWithoutTarget(Type interfaceType, IInterceptor[] interceptors)
        {
            return ProtectedCreateInterfaceProxy(interfaceType, null, null, interceptors);
        }

        public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)ProtectedCreateInterfaceProxy(typeof(TInterface), null, options, interceptors);
        }

        public object CreateInterfaceProxyWithoutTarget(Type interfaceType, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateInterfaceProxy(interfaceType, null, options, interceptors);
        }

        protected virtual object ProtectedCreateInterfaceProxy(Type interfaceType, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (interfaceType is null)
                throw new ArgumentNullException(nameof(interfaceType));

            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"Interface expected for '{interfaceType.Name}'");

            if (target != null && !interfaceType.IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException($"The target '{target.GetType().Name}' doesn't implement the interface '{interfaceType.Name}'");

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(interfaceType, target, options);
            return CreateProxy(typeDefinition, options, interceptors);
        }

        #endregion

        protected virtual object CreateProxy(ProxyTypeDefinition typeDefinition, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));

            Type proxyType = CreateProxyType(typeDefinition, options, interceptors);

            OnProxyTypeCreated(typeDefinition.Type, typeDefinition.Target, proxyType, options);

            object target = typeDefinition.Target;

            List<object> args = new List<object>();
            args.Add(interceptors);
            if (target != null)
                args.Add(target);

            if (options != null && options.MixinInstances.Count > 0)
                args.AddRange(options.MixinInstances);

            // base ctor dependencies
            ConstructorInfo constructor = proxyType.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            int length = parameters.Length;
            if (length > args.Count)
            {
                IConstructorInjectionResolver constructorInjectionResolver;
                if (options != null && options.ConstructorInjectionResolver != null)
                    constructorInjectionResolver = options.ConstructorInjectionResolver;
                else
                    constructorInjectionResolver = defaultConstructorInjectionResolver;

                for (int i = args.Count; i < length; i++)
                    args.Add(constructorInjectionResolver.Resolve(parameters[i]));
            }

//#if NET45 || NET472
//            ProxyBuilder.ModuleScope.Save();
//#endif

            return CreateProxyInstance(proxyType, args.ToArray());
        }

        protected virtual Type CreateProxyType(ProxyTypeDefinition typeDefinition, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));

            Type buildType = proxyBuilder.ModuleScope.ProxyTypeRegistry.GetBuidType(typeDefinition.FullName, options);
            if (buildType == null)
            {
                TypeBuilder typeBuilder = ProxyBuilder.CreateType(typeDefinition, interceptors);
                buildType = typeBuilder.BuildType();
                proxyBuilder.ModuleScope.ProxyTypeRegistry.Add(typeDefinition.FullName, options, buildType);
            }
            return buildType;
        }

        protected virtual ProxyTypeDefinition GetTypeDefinition(Type type, object target, ProxyGeneratorOptions options)
        {
            return ModuleDefinition.GetOrAdd(type, target, options);
        }

        protected virtual object CreateProxyInstance(Type proxyType, object[] args)
        {
            if (proxyType is null)
                throw new ArgumentNullException(nameof(proxyType));

            try
            {
                return Activator.CreateInstance(proxyType, args);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to create an instance for '{proxyType.Name}'", ex);
            }
        }

        protected void OnProxyTypeCreated(Type type, object target, Type proxyType, ProxyGeneratorOptions options)
        {
            ProxyTypeCreated?.Invoke(this, new ProxyTypeCreatedEventArgs(type, target, proxyType, options));
        }

        public event EventHandler<ProxyTypeCreatedEventArgs> ProxyTypeCreated;
    }
}
