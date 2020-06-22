using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests.Builder
{

    [TestClass]
    public class ProxyMethodBuilderTests
    {
        [TestMethod]
        public void Attributes_Added()
        {
            var builder = new ProxyMethodBuilder();

            var moduleScope = TestHelpers.GetNewModuleScope();
            var typeBuilder = TestHelpers.GetNewTypeBuilder(moduleScope.Module);
            var collector = new ModuleDefinition();
            var typeDef = collector.GetOrAdd(typeof(TypeM1), null, null);
            var typeMet = typeDef.MethodDefinitions.FirstOrDefault(p => p.Name == "Method");
            var fields = TestHelpers.GetFields(typeBuilder, null);

            var methodBuilder = builder.CreateMethod(moduleScope, typeBuilder, typeMet, typeMet.Method, fields);

            Type type = typeBuilder.BuildType();

            var typeAtt = type.GetCustomAttributes();
            var metAtt = type.GetMethod("Method").GetCustomAttributes();
            Assert.AreEqual(0, typeAtt.Count());
            Assert.AreEqual(0, metAtt.Count());
        }

        [TestMethod]
        public void Attributes_Added_With_Parent_Defined()
        {
            var builder = new ProxyMethodBuilder();

            var moduleScope = TestHelpers.GetNewModuleScope();
            var typeBuilder = TestHelpers.GetNewTypeBuilder(moduleScope.Module);
            var collector = new ModuleDefinition();
            var typeDef = collector.GetOrAdd(typeof(TypeM1), null, null);
            var typeMet = typeDef.MethodDefinitions.FirstOrDefault(p => p.Name == "Method");
            var fields = TestHelpers.GetFields(typeBuilder, null);

            typeBuilder.SetParent(typeof(TypeM1));

            var methodBuilder = builder.CreateMethod(moduleScope, typeBuilder, typeMet, typeMet.Method, fields);

            Type type = typeBuilder.BuildType();

            var typeAtt = type.GetCustomAttributes();
            var metAtt = type.GetMethod("Method").GetCustomAttributes();
            Assert.AreEqual(1, typeAtt.Count());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(1, metAtt.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), metAtt.FirstOrDefault().GetType());
        }
    }

    public class TestHelpers
    {
        public static ModuleBuilder GetNewModule()
        {
            ModuleScope moduleScope = GetNewModuleScope();
            return moduleScope.Module;
        }

        public static ModuleScope GetNewModuleScope()
        {
            return new ModuleScope($"DynAssembly{Guid.NewGuid()}", $"DynModule{Guid.NewGuid()}");
        }

        public static TypeBuilder GetNewTypeBuilder(ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType("Test", TypeAttributes.Public);
        }

        public static FieldBuilder[] GetFields(TypeBuilder typeBuilder, object target)
        {
            FieldBuilder interceptorsField = typeBuilder.DefineField("_interceptors", typeof(IInterceptor[]), FieldAttributes.Private);
            if (target != null)
            {
                FieldBuilder targetField = typeBuilder.DefineField("_target", target.GetType(), FieldAttributes.Private);
                return new FieldBuilder[] { interceptorsField, targetField };
            }
            return new FieldBuilder[] { interceptorsField };
        }
    }

    [AllInterceptor(typeof(InterceptorM1))]
    public interface ITypeM2
    {
        [PropertySetInterceptor(typeof(InterceptorM3))]
        string MyProperty { get; set; }

        [MethodInterceptor(typeof(InterceptorM2))]
        void Method();

        [AddEventInterceptor(typeof(InterceptorM4))]
        event EventHandler MyEvent;
    }

    public class TypeM2 : ITypeM2
    {
        public string MyProperty { get ; set ; }

        public event EventHandler MyEvent;

        public virtual void Method()
        {
        }
    }


    [AllInterceptor(typeof(InterceptorM1))]
    public class TypeM1
    {
        public static List<StateTypes> States = new List<StateTypes>();

        [PropertySetInterceptor(typeof(InterceptorM3))]
        public virtual string MyProperty { get; set; }

        [MethodInterceptor(typeof(InterceptorM2))]
        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }

        [AddEventInterceptor(typeof(InterceptorM4))]
        public virtual event EventHandler MyEvent;
    }

    [AllInterceptor(typeof(InterceptorM1))]
    public class TypeM1Full
    {
        private string myProperty;

        [PropertySetInterceptor(typeof(InterceptorM3))]
        public virtual string MyProperty
        {
            get { return myProperty; }
            set { myProperty = value; }
        }

        [MethodInterceptor(typeof(InterceptorM2))]
        public virtual void Method()
        {

        }

        private event EventHandler myEvent;
        [AddEventInterceptor(typeof(InterceptorM4))]
        public virtual event EventHandler MyEvent
        {
            add { myEvent += value; }
            remove { myEvent -= value; }
        }
    }

    public class InterceptorM1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeM1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeM1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class InterceptorM2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeM1.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeM1.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class InterceptorM3 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeM1.States.Add(StateTypes.Interceptor3_IsCalledBefore);

            invocation.Proceed();

            TypeM1.States.Add(StateTypes.Interceptor3_IsCalledAfter);
        }
    }

    public class InterceptorM4 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeM1.States.Add(StateTypes.Interceptor4_IsCalledBefore);

            invocation.Proceed();

            TypeM1.States.Add(StateTypes.Interceptor4_IsCalledAfter);
        }
    }
}
