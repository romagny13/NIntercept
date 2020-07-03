using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace NIntercept.Tests
{
    [TestClass]
    public class ProxyGeneratorBaseCtorTests
    {
        [TestInitialize()]
        public void Startup()
        {
            TypeInj1.Clear();
        }

        [TestMethod]
        public void With_Default_ConstructorInjectionResolver()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInj1>();

            Assert.IsNotNull(proxy);
            Assert.IsNull(TypeInj1.InjService1);
            Assert.IsNull(TypeInj1.InjService2);
            Assert.AreEqual(null, TypeInj1.P1);
            Assert.AreEqual(0, TypeInj1.P2);
        }

        [TestMethod]
        public void With_Custom_ConstructorInjectionResolver()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.ConstructorInjectionResolver = new MyCtorInjectionResolver();

            var proxy = generator.CreateClassProxy<TypeInj1>(options);

            Assert.IsNotNull(proxy);
            Assert.IsNotNull(TypeInj1.InjService1);
            Assert.IsNotNull(TypeInj1.InjService2);
            Assert.AreEqual("Ok", TypeInj1.P1);
            Assert.AreEqual(10, TypeInj1.P2);
        }

        [TestMethod]
        public void With_Default_ConstructorInjectionResolver_With_Target()
        {
            var generator = new ProxyGenerator();

            var target = new TypeInj2(new InjService1(), new InjService2(), "Test", 100);
            var proxy = generator.CreateClassProxyWithTarget<TypeInj2>(target);

            Assert.IsNotNull(proxy);
            Assert.IsNotNull(target.InjService1);
            Assert.IsNotNull(target.InjService2);
            Assert.AreEqual("Test", target.P1);
            Assert.AreEqual(100, target.P2);

            Assert.IsNull(proxy.InjService1);
            Assert.IsNull(proxy.InjService2);
            Assert.AreEqual(null, proxy.P1);
            Assert.AreEqual(0, proxy.P2);
        }

        [TestMethod]
        public void With_Custom_ConstructorInjectionResolver_With_Target()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.ConstructorInjectionResolver = new MyCtorInjectionResolver();

            var target = new TypeInj2(new InjService1(), new InjService2(), "Test", 100);
            var proxy = generator.CreateClassProxyWithTarget<TypeInj2>(target, options);

            Assert.IsNotNull(proxy);
            Assert.IsNotNull(target.InjService1);
            Assert.IsNotNull(target.InjService2);
            Assert.AreEqual("Test", target.P1);
            Assert.AreEqual(100, target.P2);

            Assert.IsNotNull(proxy.InjService1);
            Assert.IsNotNull(proxy.InjService2);
            Assert.AreEqual("Ok", proxy.P1);
            Assert.AreEqual(10, proxy.P2);
        }

        [TestMethod]
        public void With_Container_ConstructorInjectionResolver_And_Values()
        {
            var generator = new ProxyGenerator();

            IUnityContainer container = new UnityContainer();
            container.RegisterSingleton<IInjSservice1, InjService1>();
            container.RegisterSingleton<IInjSservice2, InjService2>();

            var options = new ProxyGeneratorOptions();
            options.ConstructorInjectionResolver = new UnityConstructorInjectionResolver(container);

            var proxy = generator.CreateClassProxy<TypeInj1>(options);

            Assert.IsNotNull(proxy);
            Assert.IsNotNull(TypeInj1.InjService1);
            Assert.IsNotNull(TypeInj1.InjService2);
            Assert.AreEqual("Ok", TypeInj1.P1);
            Assert.AreEqual(10, TypeInj1.P2);
        }
    }

    public class UnityConstructorInjectionResolver : IConstructorInjectionResolver
    {
        private readonly IUnityContainer container;

        public UnityConstructorInjectionResolver(IUnityContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            this.container = container;
        }

        public object Resolve(ParameterInfo parameter)
        {
            if (parameter.Name == "p1")
                return "Ok";

            if (parameter.Name == "p2")
                return 10;

            return container.Resolve(parameter.ParameterType);
        }
    }

    public class MyCtorInjectionResolver : DefaultConstructorInjectionResolver
    {
        public override object Resolve(ParameterInfo parameter)
        {
            if (parameter.Name == "p1")
                return "Ok";

            if (parameter.Name == "p2")
                return 10;

            if (parameter.Name == "injService1")
                return new InjService1();

            if (parameter.Name == "injService2")
                return new InjService2();

            return base.Resolve(parameter);
        }
    }


    public interface IInjSservice1
    {

    }

    public class InjService1 : IInjSservice1
    {

    }

    public interface IInjSservice2
    {

    }

    public class InjService2 : IInjSservice2
    {

    }

    public class TypeInj1
    {
        public TypeInj1(IInjSservice1 injService1, IInjSservice2 injService2, string p1, int p2)
        {
            InjService1 = injService1;
            InjService2 = injService2;
            P1 = p1;
            P2 = p2;
        }

        public static void Clear()
        {
            InjService1 = null;
            InjService2 = null;
            P1 = null;
            P2 = null;
        }

        public static IInjSservice1 InjService1 { get; set; }
        public static IInjSservice2 InjService2 { get; set; }
        public static string P1 { get; set; }
        public static int? P2 { get; set; }
    }


    public class TypeInj2
    {
        public TypeInj2(IInjSservice1 injService1, IInjSservice2 injService2, string p1, int p2)
        {
            InjService1 = injService1;
            InjService2 = injService2;
            P1 = p1;
            P2 = p2;
        }

        public IInjSservice1 InjService1 { get; set; }
        public IInjSservice2 InjService2 { get; set; }
        public string P1 { get; set; }
        public int? P2 { get; set; }
    }


    public class TypeInj3
    {
        public TypeInj3(IInjSservice1 injService1, IInjSservice2 injService2)
        {
            InjService1 = injService1;
            InjService2 = injService2;
        }

        public static void Clear()
        {
            InjService1 = null;
            InjService2 = null;
        }

        public static IInjSservice1 InjService1 { get; set; }
        public static IInjSservice2 InjService2 { get; set; }
    }
}
