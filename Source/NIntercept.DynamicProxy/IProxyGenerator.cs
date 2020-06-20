using System;
using NIntercept.Definition;

namespace NIntercept
{
    public interface IProxyGenerator
    {
        ModuleDefinition ModuleDefinition { get; set; }
        IProxyBuilder ProxyBuilder { get; }

        object CreateClassProxy(Type parentType, IInterceptor[] interceptors);
        TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class;
        object CreateClassProxyWithTarget(object target, IInterceptor[] interceptors);
        TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors) where TClass : class;
        object CreateInterfaceProxyWithTarget(Type interfaceType, object target, IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class;
        object CreateProxy(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors);
        Type CreateProxyType(ProxyTypeDefinition typeDefinition, IInterceptor[] interceptors);
        ProxyTypeDefinition GetTypeDefinition(Type type, object target);
    }
}