using NIntercept.Builder;
using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests
{
    public class ProxyMethodBuilderMock : InterceptableMethodBuilder
    {
        public bool IsUsed { get; set; }

        public override MethodBuilder CreateMethod(ProxyScope proxyScope, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder memberField, FieldBuilder callerMethodField)
        {
            IsUsed = true;
            return base.CreateMethod(proxyScope, methodDefinition, member, memberField, callerMethodField);
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
}
