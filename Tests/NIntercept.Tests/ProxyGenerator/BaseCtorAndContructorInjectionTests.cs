using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIntercept.Definition;
using Unity;

namespace NIntercept.Tests
{
    // Only for class proxy
    // - base ctor
    // - no base ctor?
    // - ctor inaccessible
    // - constructor selector
    // - injection with custom constructor injection resolver
    // - injection with container

    [TestClass]
    public class BaseCtorAndContructorInjectionTests
    {
        [TestInitialize()]
        public void Startup()
        {
            TypeInj1.Clear();
        }

        [TestMethod]
        public void Parameterless_Ctor_Is_Called()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInj_With_ParameterLess_Ctor>();

            Assert.IsNotNull(proxy);
            Assert.AreEqual(true, TypeInj_With_ParameterLess_Ctor.IsCalled);
        }

        [TestMethod]
        public void BaseCtor_Inaccessible_Is_Ignored()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInj_With_Private_Ctor>();

            Assert.IsNotNull(proxy);
            Assert.AreEqual(false, TypeInj_With_Private_Ctor.IsCalled);
        }

        [TestMethod]
        public void Select_Constructor()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.ConstructorSelector = new MostParameterConstructorSelector();
            var proxy = generator.CreateClassProxy<TypeInj_Multi_Ctor>(options);

            Assert.IsNotNull(proxy);
            Assert.AreEqual(true, TypeInj_Multi_Ctor.IsMostCalled);
            Assert.IsNull(TypeInj_Multi_Ctor.InjService1);
            Assert.IsNull(TypeInj_Multi_Ctor.InjService2);
        }

        [TestMethod]
        public void Select_Constructor_With_Custom_ConstructorResolver_Resolve_Injections()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.ConstructorSelector = new MostParameterConstructorSelector();
            generator.ConstructorInjectionResolver = new MyCtorInjectionResolver();
            var proxy = generator.CreateClassProxy<TypeInj_Multi_Ctor>(options);

            Assert.IsNotNull(proxy);
            Assert.AreEqual(true, TypeInj_Multi_Ctor.IsMostCalled);
            Assert.IsNotNull(TypeInj_Multi_Ctor.InjService1);
            Assert.IsNotNull(TypeInj_Multi_Ctor.InjService2);
        }

        [TestMethod]
        public void Default_ConstructorInjectionResolver_Return_Default_Values()
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
        public void Custom_ConstructorInjectionResolver_Resolve_Injections()
        {
            var generator = new ProxyGenerator();

            generator.ConstructorInjectionResolver = new MyCtorInjectionResolver();
            var proxy = generator.CreateClassProxy<TypeInj1>();

            Assert.IsNotNull(proxy);
            Assert.IsNotNull(TypeInj1.InjService1);
            Assert.IsNotNull(TypeInj1.InjService2);
            Assert.AreEqual("Ok", TypeInj1.P1);
            Assert.AreEqual(10, TypeInj1.P2);
        }

        [TestMethod]
        public void Default_ConstructorInjectionResolver_For_Target_Return_Default_Values()
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
        public void Custom_ConstructorInjectionResolver_For_Target_Resolve_Injections()
        {
            var generator = new ProxyGenerator();

            generator.ConstructorInjectionResolver = new MyCtorInjectionResolver();

            var target = new TypeInj2(new InjService1(), new InjService2(), "Test", 100);
            var proxy = generator.CreateClassProxyWithTarget<TypeInj2>(target);

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
        public void Custom_ConstructorInjectionResolver_With_UnityContainer_Resolve_Injections()
        {
            var generator = new ProxyGenerator();

            IUnityContainer container = new UnityContainer();
            container.RegisterSingleton<IInjSservice1, InjService1>();
            container.RegisterSingleton<IInjSservice2, InjService2>();

            generator.ConstructorInjectionResolver = new UnityConstructorInjectionResolver(container);
            var proxy = generator.CreateClassProxy<TypeInj1>();

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

    public interface IInjSservice1 { }
    public class InjService1 : IInjSservice1 { }
    public interface IInjSservice2 { }
    public class InjService2 : IInjSservice2 { }

    public class TypeInj_With_ParameterLess_Ctor
    {
        public TypeInj_With_ParameterLess_Ctor()
        {
            IsCalled = true;
        }

        public static bool IsCalled { get; set; }
    }

    public class TypeInj_With_Private_Ctor
    {
        private TypeInj_With_Private_Ctor()
        {
            IsCalled = true;
        }

        public static bool IsCalled { get; set; }
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

    public class TypeInj_Multi_Ctor
    {
        public TypeInj_Multi_Ctor(IInjSservice1 injService1)
        {
            InjService1 = injService1;
        }

        public TypeInj_Multi_Ctor(IInjSservice1 injService1, IInjSservice2 injService2)
        {
            IsMostCalled = true;
            InjService1 = injService1;
            InjService2 = injService2;
        }

        public static void Clear()
        {
            InjService1 = null;
            InjService2 = null;
        }

        public static bool IsMostCalled { get; set; }
        public static IInjSservice1 InjService1 { get; set; }
        public static IInjSservice2 InjService2 { get; set; }
    }

    public class MostParameterConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Select(Type type)
        {
            var candidates = type.GetConstructors();
            ConstructorInfo constructor = null;
            int parameterLength = 0;
            foreach (var candidate in candidates)
            {
                var length = candidate.GetParameters().Length;
                if (constructor == null || length > parameterLength)
                {
                    constructor = candidate;
                    parameterLength = length;
                }
            }
            return constructor;
        }
    }
}
