using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NIntercept
{

    public class ProxyGenerator : IProxyGenerator
    {
        private readonly IConstructorInjectionResolver defaultConstructorInjectionResolver;
        private readonly ModuleScope moduleScope;
        private readonly ModuleDefinition moduleDefinition;
        private IConstructorInjectionResolver constructorInjectionResolver;

        public ProxyGenerator(ModuleScope moduleScope)
        {
            if (moduleScope is null)
                throw new ArgumentNullException(nameof(moduleScope));

            this.moduleDefinition = new ModuleDefinition();
            this.defaultConstructorInjectionResolver = new DefaultConstructorInjectionResolver();
            this.moduleScope = moduleScope;
        }

        public ProxyGenerator()
            : this(new ModuleScope())
        { }

        public ModuleScope ModuleScope
        {
            get { return moduleScope; }
        }

        public virtual IConstructorInjectionResolver ConstructorInjectionResolver
        {
            get { return constructorInjectionResolver ?? defaultConstructorInjectionResolver; }
            set { constructorInjectionResolver = value; }
        }

        #region CreateClassProxy

        public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), null, null, interceptors);
        }

        public object CreateClassProxy(Type parentType, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(parentType, null, null, interceptors);
        }

        public TClass CreateClassProxy<TClass>(ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), null, options, interceptors);
        }

        public object CreateClassProxy(Type parentType, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(parentType, null, options, interceptors);
        }

        #endregion

        #region CreateClassProxyWithTarget

        public TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), target, null, interceptors);
        }

        public object CreateClassProxyWithTarget(object target, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(target?.GetType(), target, null, interceptors);
        }

        public TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class
        {
            return (TClass)ProtectedCreateClassProxy(typeof(TClass), target, options, interceptors);
        }

        public object CreateClassProxyWithTarget(object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            return ProtectedCreateClassProxy(target?.GetType(), target, options, interceptors);
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

        #endregion


        public ProxyTypeDefinition GetTypeDefinition(Type type, object target, ProxyGeneratorOptions options)
        {
            return moduleDefinition.GetTypeDefinition(type, target?.GetType(), options);
        }

        protected object ProtectedCreateClassProxy(Type type, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type.IsInterface)
                throw new InvalidOperationException($"Unexpected interface for '{type.Name}'");
            if (target != null && !type.IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException($"The target '{target.GetType().Name}' is not assignable from '{type.Name}'");
            ProxyTypeDefinition typeDefinition = GetTypeDefinition(type, target, options);
            return CreateProxy(typeDefinition, interceptors, target);
        }

        protected object ProtectedCreateInterfaceProxy(Type type, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsInterface)
                throw new InvalidOperationException($"Interface expected for '{type.Name}'");
            if (target != null && !type.IsAssignableFrom(target.GetType()))
                throw new InvalidOperationException($"The target '{target.GetType().Name}' is not assignable from '{type.Name}'");

            ProxyTypeDefinition typeDefinition = GetTypeDefinition(type, target, options);
            return CreateProxy(typeDefinition, interceptors, target);
        }

        protected object CreateProxy(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors, object target)
        {
            if (typeDefinition is null)
                throw new ArgumentNullException(nameof(typeDefinition));
            if (interceptors is null)
                throw new ArgumentNullException(nameof(interceptors));

            Type proxyType = CreateProxyType(typeDefinition, target);

            object[] args = GetArguments(typeDefinition, interceptors, target, proxyType);

            //#if NET45 || NET472
            //            moduleScope.Save();
            //#endif

            return CreateProxyInstance(proxyType, args);
        }

        protected object[] GetArguments(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors, object target, Type proxyType)
        {
            List<object> args = new List<object>();

            // interceptors
            args.Add(interceptors);

            // target
            if (target != null)
                args.Add(target);

            // interceptorSelector
            ProxyGeneratorOptions options = typeDefinition.Options;
            IInterceptorSelector interceptorSelector = options?.InterceptorSelector;
            if(interceptorSelector != null)
                args.Add(interceptorSelector);

            // mixins
            if (options != null && options.MixinInstances.Count > 0)
                args.AddRange(options.MixinInstances);

            // base ctor dependencies
            ConstructorInfo constructor = proxyType.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            int length = parameters.Length;
            if (length > args.Count)
            {
                for (int i = args.Count; i < length; i++)
                    args.Add(ConstructorInjectionResolver.Resolve(parameters[i]));
            }

            return args.ToArray();
        }

        public Type CreateProxyType(ProxyTypeDefinition typeDefinition, object target)
        {
            Type proxyType = moduleScope.GetOrCreateProxyType(typeDefinition);
            OnProxyTypeCreated(typeDefinition.Type, target, proxyType);
            return proxyType;
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

        protected void OnProxyTypeCreated(Type type, object target, Type proxyType)
        {
            ProxyTypeCreated?.Invoke(this, new ProxyTypeCreatedEventArgs(type, target, proxyType));
        }

        public event EventHandler<ProxyTypeCreatedEventArgs> ProxyTypeCreated;
    }
}
