using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NIntercept.Tests
{
    public class ProxyEventBuilderMock : ProxyEventBuilder
    {
        public bool IsUsed { get; set; }

        public override EventBuilder CreateEvent(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyEventDefinition eventDefinition, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateEvent(moduleBuilder, typeBuilder, eventDefinition, fields);
        }
    }

    public class ProxyPropertyBuilderMock : ProxyPropertyBuilder
    {

        public bool IsUsed { get; set; }

        public override PropertyBuilder CreateProperty(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyPropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateProperty(moduleBuilder, typeBuilder, propertyDefinition, fields);
        }
    }

    public class ProxyMethodBuilderMock : ProxyMethodBuilder
    {
        public bool IsUsed { get; set; }


        public override MethodBuilder CreateMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, ProxyMethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields)
        {
            IsUsed = true;
            return base.CreateMethod(moduleBuilder, typeBuilder, methodDefinition, member, fields);
        }
    }

    public class TypeDefintionCollectorMock : ModuleDefinition
    {
        public bool IsUsed { get; set; }

        public override ProxyTypeDefinition GetOrAdd(Type type, object target)
        {
            IsUsed = true;
            return base.GetOrAdd(type, target);
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
        public ProxyTypeDefinitionMock(ModuleDefinition moduleDefinition, Type type, object target) : base(moduleDefinition, type, target)
        {
        }
    }
}
