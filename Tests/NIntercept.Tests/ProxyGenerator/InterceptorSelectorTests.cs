using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NIntercept.Tests
{
    [TestClass]
    public class InterceptorSelectorTests
    {
        [TestInitialize()]
        public void Startup()
        {

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();
        }

        [TestMethod]
        public void Select_Interceptors_For_Methods()
        {
            var generator = new ProxyGenerator();
            var options = new ProxyGeneratorOptions();
            options.InterceptorSelector = new IS1();
            var proxy = generator.CreateClassProxy<TypeIS1>(options, new IntGetForIS1(), new IntSetForIS1(), new IntMethForIS1(), new IntAddForIS1(), new IntRemoveForIS1());

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(0, TypeIS1.States.Count);

            // set
            proxy.MyProperty = "New value";

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(true, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Set_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // get
            var p = proxy.MyProperty;

            Assert.AreEqual(true, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual("New value", p);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Get_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method
            proxy.Method();

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method with parameter

            proxy.Method("My paramete");

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            EventHandler ev = null;
            ev = (s, e) => { };

            proxy.MyEvent += ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(true, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(true, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeIS1.States[0]);
        }

        [TestMethod]
        public void Select_Interceptors_For_Methods_For_ClassProxy_With_Target()
        {
            var generator = new ProxyGenerator();
            var options = new ProxyGeneratorOptions();
            options.InterceptorSelector = new IS1();
            var target = new TypeIS1();
            var proxy = generator.CreateClassProxyWithTarget<TypeIS1>(target, options, new IntGetForIS1(), new IntSetForIS1(), new IntMethForIS1(), new IntAddForIS1(), new IntRemoveForIS1());

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(0, TypeIS1.States.Count);

            // set
            proxy.MyProperty = "New value";

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(true, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Set_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // get
            var p = proxy.MyProperty;

            Assert.AreEqual(true, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual("New value", p);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Get_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method
            proxy.Method();

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method with parameter

            proxy.Method("My paramete");

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            EventHandler ev = null;
            ev = (s, e) => { };

            proxy.MyEvent += ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(true, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(true, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeIS1.States[0]);
        }

        [TestMethod]
        public void Select_Interceptors_For_Methods_For_InterfaceProxy_With_Target()
        {
            var generator = new ProxyGenerator();
            var options = new ProxyGeneratorOptions();
            options.InterceptorSelector = new IS1();
            var target = new TypeIS1();
            var proxy = generator.CreateInterfaceProxyWithTarget<ITypeIS1>(target, options, new IntGetForIS1(), new IntSetForIS1(), new IntMethForIS1(), new IntAddForIS1(), new IntRemoveForIS1());

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(0, TypeIS1.States.Count);

            // set
            proxy.MyProperty = "New value";

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(true, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Set_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // get
            var p = proxy.MyProperty;

            Assert.AreEqual(true, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual("New value", p);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Get_Called, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method
            proxy.Method();

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            // method with parameter

            proxy.Method("My paramete");

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(true, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            EventHandler ev = null;
            ev = (s, e) => { };

            proxy.MyEvent += ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(true, IntAddForIS1.IsCalled);
            Assert.AreEqual(false, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeIS1.States[0]);

            IntSetForIS1.IsCalled = false;
            IntGetForIS1.IsCalled = false;
            IntMethForIS1.IsCalled = false;
            IntAddForIS1.IsCalled = false;
            IntRemoveForIS1.IsCalled = false;
            TypeIS1.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(false, IntGetForIS1.IsCalled);
            Assert.AreEqual(false, IntSetForIS1.IsCalled);
            Assert.AreEqual(false, IntMethForIS1.IsCalled);
            Assert.AreEqual(false, IntAddForIS1.IsCalled);
            Assert.AreEqual(true, IntRemoveForIS1.IsCalled);
            Assert.AreEqual(1, TypeIS1.States.Count);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeIS1.States[0]);
        }
    }

    public class IntGetForIS1 : IInterceptor
    {
        public static bool IsCalled { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalled = true;
            invocation.Proceed();
        }
    }

    public class IntSetForIS1 : IInterceptor
    {
        public static bool IsCalled { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalled = true;
            invocation.Proceed();
        }
    }

    public class IntMethForIS1 : IInterceptor
    {
        public static bool IsCalled { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalled = true;
            invocation.Proceed();
        }
    }

    public class IntAddForIS1 : IInterceptor
    {
        public static bool IsCalled { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalled = true;
            invocation.Proceed();
        }
    }

    public class IntRemoveForIS1 : IInterceptor
    {
        public static bool IsCalled { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalled = true;
            invocation.Proceed();
        }
    }

    public class IS1 : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (method.Name.StartsWith("get_"))
                return new IInterceptor[] { interceptors.First(p => p.GetType() == typeof(IntGetForIS1)) };
            else if (method.Name.StartsWith("set_"))
                return new IInterceptor[] { interceptors.First(p => p.GetType() == typeof(IntSetForIS1)) };
            else if (method.Name.StartsWith("add_"))
                return new IInterceptor[] { interceptors.First(p => p.GetType() == typeof(IntAddForIS1)) };
            else if (method.Name.StartsWith("remove_"))
                return new IInterceptor[] { interceptors.First(p => p.GetType() == typeof(IntRemoveForIS1)) };
            else
                return new IInterceptor[] { interceptors.First(p => p.GetType() == typeof(IntMethForIS1)) };
        }
    }

    public interface ITypeIS1
    {
        string MyProperty { get; set; }

        event EventHandler MyEvent;

        void Method();
        void Method(string p1);
    }

    public class TypeIS1 : ITypeIS1
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }


        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }

        public virtual void Method(string p1)
        {
            States.Add(StateTypes.Class_Method_2);
        }

        private event EventHandler myEvent;

        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }

    }
}
