using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests
{
    public class ProxyEventBuilderMock : ProxyEventBuilder
    {
        public bool IsUsed { get; set; }

        public override EventBuilder CreateEvent(ModuleScope moduleScope, TypeBuilder typeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateEvent(moduleScope, typeBuilder, eventDefinition, fields);
        }
    }

    public class ProxyPropertyBuilderMock : ProxyPropertyBuilder
    {

        public bool IsUsed { get; set; }

        public override PropertyBuilder CreateProperty(ModuleScope moduleScope, TypeBuilder typeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateProperty(moduleScope, typeBuilder, propertyDefinition, fields);
        }
    }

    public class ProxyMethodBuilderMock : ProxyMethodBuilder
    {
        public bool IsUsed { get; set; }


        public override MethodBuilder CreateMethod(ModuleScope moduleScope, TypeBuilder typeBuilder, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateMethod(moduleScope, typeBuilder, methodDefinition, member, fields);
        }
    }

    public class TypeDefintionCollectorMock : ModuleDefinition
    {
        public bool IsUsed { get; set; }

        public override ProxyTypeDefinition GetOrAdd(Type type, object target, ProxyGeneratorOptions options)
        {
            IsUsed = true;
            return base.GetOrAdd(type, target, options);
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
        public ProxyTypeDefinitionMock(ModuleDefinition moduleDefinition, Type type, object target, ProxyGeneratorOptions options) : base(moduleDefinition, type, target, options)
        {
        }

        public override TypeDefinitionType TypeDefinitionType => TypeDefinitionType.ClassProxy;

        protected override TypeDefinition GetTypeDefinition()
        {
            return this;
        }
    }
}
