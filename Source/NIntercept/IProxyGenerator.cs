using NIntercept.Definition;
using System;

namespace NIntercept
{
    public interface IProxyGenerator
    {
        ModuleDefinition ModuleDefinition { get; set; }
        IProxyBuilder ProxyBuilder { get; }

        event EventHandler<ProxyTypeCreatedEventArgs> ProxyTypeCreated;

        object CreateClassProxy(Type parentType, IInterceptor[] interceptors);
        object CreateClassProxy(Type parentType, ProxyGeneratorOptions options, IInterceptor[] interceptors);
        TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class;
        TClass CreateClassProxy<TClass>(ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class;
        object CreateClassProxyWithTarget(object target, IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(object target, ProxyGeneratorOptions options, IInterceptor[] interceptors);
        TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class;
        TClass CreateClassProxyWithTarget<TClass>(TClass target, ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TClass : class;
        object CreateInterfaceProxyWithoutTarget(Type interfaceType, IInterceptor[] interceptors);
        object CreateInterfaceProxyWithoutTarget(Type interfaceType, ProxyGeneratorOptions options, IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params IInterceptor[] interceptors) where TInterface : class;
        TInterface CreateInterfaceProxyWithoutTarget<TInterface>(ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TInterface : class;
        object CreateInterfaceProxyWithTarget(Type interfaceType, object target, IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTarget(Type interfaceType, object target, ProxyGeneratorOptions options, IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class;
        TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, ProxyGeneratorOptions options, params IInterceptor[] interceptors) where TInterface : class;
    }
}
