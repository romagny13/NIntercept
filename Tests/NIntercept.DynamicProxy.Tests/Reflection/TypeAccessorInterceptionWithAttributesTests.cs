using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace NIntercept.Tests
{
    [TestClass]
    public class TypeAccessorInterceptionWithAttributesTests
    {

        [TestInitialize()]
        public void Startup()
        {
            TypeRA1.States.Clear();
            TypeRA2.States.Clear();
            TypeRA3.States.Clear();
        }

        [TestMethod]
        public void Intercept_Protected_Property()
        {
            var target = new TypeRA1();
            var accessor = new TypeAccessor(target);
            var p1 = accessor.Properties["MyProperty"];

            Assert.AreEqual(0, TypeRA1.States.Count);

            p1.InterceptSet(new object[] { "A" });

            Assert.AreEqual(3, TypeRA1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA1.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeRA1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA1.States[2]);

            TypeRA1.States.Clear();

            var v = p1.InterceptGet<string>(new object[0]);

            Assert.AreEqual(3, TypeRA1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA1.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeRA1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Intercept_Static_Property()
        {
            var accessor = new TypeAccessor(typeof(TypeRA1));
            var p1 = accessor.Properties["MyPropertyStatic"];

            Assert.AreEqual(0, TypeRA1.States.Count);

            p1.InterceptSet(new object[] { "A" });

            Assert.AreEqual(3, TypeRA1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA1.States[0]);
            Assert.AreEqual(StateTypes.Set2_Called, TypeRA1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA1.States[2]);

            TypeRA1.States.Clear();

            var v = p1.InterceptGet<string>(new object[0]);

            Assert.AreEqual(3, TypeRA1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA1.States[0]);
            Assert.AreEqual(StateTypes.Get2_Called, TypeRA1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Intercept_Method()
        {
            var target = new TypeRA2();
            var accessor = new TypeAccessor(target);
            var m1 = accessor.Methods["MyMethod"][0];

            Assert.AreEqual(0, TypeRA2.States.Count);

            m1.Intercept(new object[] { "A" });

            Assert.AreEqual(3, TypeRA2.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeRA2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA2.States[2]);
            Assert.AreEqual("A", TypeRA2.P1);
        }

        [TestMethod]
        public void Intercept_Static_Method()
        {
            var accessor = new TypeAccessor(typeof(TypeRA2));
            var m1 = accessor.Methods["MyMethodStatic"][0];

            Assert.AreEqual(0, TypeRA2.States.Count);

            m1.Intercept(new object[] { "B" });

            Assert.AreEqual(3, TypeRA2.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeRA2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA2.States[2]);
            Assert.AreEqual("B", TypeRA2.P2);
        }

        [TestMethod]
        public void Intercept_Events()
        {
            bool isCalled = false;
            var target = new TypeRA3();
            var accessor = new TypeAccessor(target);
            var m1 = accessor.Events["MyEvent"];

            Assert.AreEqual(0, TypeRA3.States.Count);

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            m1.InterceptAdd(new object[] { ev });

            Assert.AreEqual(3, TypeRA3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA3.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeRA3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);
            isCalled = false;

            TypeRA3.States.Clear();

            m1.InterceptRemove(new object[] { ev });

            Assert.AreEqual(3, TypeRA3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeRA3.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeRA3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeRA3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }
    }

    public class TypeRA1
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;

        [PropertyGetInterceptor(typeof(IntForRA1))]
        [PropertySetInterceptor(typeof(IntForRA1))]
        protected string MyProperty
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

        private static string myVarStatic;

        [PropertyGetInterceptor(typeof(IntForRA1))]
        [PropertySetInterceptor(typeof(IntForRA1))]
        public static string MyPropertyStatic
        {
            get
            {
                States.Add(StateTypes.Get2_Called);
                return myVarStatic;
            }
            set
            {
                States.Add(StateTypes.Set2_Called);
                myVarStatic = value;
            }
        }
    }

    public class TypeRA2
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public static string P1 { get; set; }
        public static string P2 { get; set; }

        [MethodInterceptor(typeof(IntForRA2))]
        private void MyMethod(string p1)
        {
            P1 = p1;

            States.Add(StateTypes.Class_Method);
        }

        [MethodInterceptor(typeof(IntForRA2))]
        public static void MyMethodStatic(string p2)
        {
            P2 = p2;

            States.Add(StateTypes.Class_Method_2);
        }
    }

    public class TypeRA3
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;

        [AddEventInterceptor(typeof(IntForRA3))]
        [RemoveEventInterceptor(typeof(IntForRA3))]
        protected event EventHandler MyEvent
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

        public void RaiseMyEvent()
        {
            myEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class IntForRA1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeRA1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeRA1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForRA2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeRA2.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeRA2.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForRA3 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeRA3.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeRA3.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
