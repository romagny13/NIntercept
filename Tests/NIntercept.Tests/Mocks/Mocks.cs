using NIntercept.Builder;
using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests
{
    public class ServiceLocatorMock : IProxyServiceProvider
    {
        public ICallbackMethodBuilder CallbackMethodBuilder { get; set; }

        public IInvocationTypeBuilder InvocationTypeBuilder { get; set; }

        public IProxyEventBuilder ProxyEventBuilder { get; set; }

        public IProxyMethodBuilder ProxyMethodBuilder { get; set; }

        public IProxyPropertyBuilder ProxyPropertyBuilder { get; set; }
    }

    public class CallbackMethodBuilderMock : CallbackMethodBuilder
    {
        public bool IsUsed { get; set; }

        public override MethodBuilder CreateMethod(ProxyScope proxyScope, CallbackMethodDefinition callbackMethodDefinition)
        {
            IsUsed = true;
            return base.CreateMethod(proxyScope, callbackMethodDefinition);
        }

    }

    public class ProxyEventBuilderMock : ProxyEventBuilder
    {
        public bool IsUsed { get; set; }

        public override EventBuilder CreateEvent(ProxyScope proxyScope, EventDefinition eventDefinition)
        {
            IsUsed = true;
            return base.CreateEvent(proxyScope, eventDefinition);
        }
    }

    public class ProxyPropertyBuilderMock : ProxyPropertyBuilder
    {

        public bool IsUsed { get; set; }

        public override PropertyBuilder CreateProperty(ProxyScope proxyScope, PropertyDefinition propertyDefinition)
        {
            IsUsed = true;
            return base.CreateProperty(proxyScope, propertyDefinition);
        }
    }

    public class ProxyMethodBuilderMock : ProxyMethodBuilder
    {
        public bool IsUsed { get; set; }

        public override MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member)
        {
            IsUsed = true;
            return base.CreateMethod(proxyScope, methodDefinition, member);
        }
    }

    public class InvocationMock : Invocation
    {
        public InvocationMock(object target, IInterceptor[] interceptors, MethodInfo method, MethodInfo proxyMethod, object proxy, object[] parameters) : base(target, interceptors, method, proxyMethod, proxy, parameters)
        {
        }

        public override Type InterceptorProviderType => typeof(IInterceptorProvider);

        protected override void InvokeMethodOnTarget()
        {

        }
    }

    public class ProxyTypeDefinitionMock : ProxyTypeDefinition
    {
        public ProxyTypeDefinitionMock(ModuleDefinition moduleDefinition, Type type, Type targetType, ProxyGeneratorOptions options) : base(moduleDefinition, type, targetType, options)
        {
        }

        public override TypeDefinitionType TypeDefinitionType => TypeDefinitionType.ClassProxy;

        protected override TypeDefinition GetTypeDefinition()
        {
            return this;
        }
    }
}
