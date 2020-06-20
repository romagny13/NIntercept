using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Reflection.Interception;

namespace NIntercept.Tests
{
    [TestClass]
    public class ReflectionExtensionsTests
    {

        [TestInitialize()]
        public void Startup()
        {
            TypeR1.States.Clear();
            TypeR2.States.Clear();
            TypeR3.States.Clear();
        }

        [TestMethod]
        public void Intercept_Protected_Property()
        {
            var target = new TypeR1();
            var p1 = typeof(TypeR1).GetProperty("MyProperty", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(0, TypeR1.States.Count);

            p1.InterceptSet(target, new object[] { "A" }, new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);

            TypeR1.States.Clear();

            var v = p1.InterceptGet<string>(target, new object[0], new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Intercept_Static_Property()
        {
            var p1 = typeof(TypeR1).GetProperty("MyPropertyStatic", BindingFlags.Public | BindingFlags.Static);

            Assert.AreEqual(0, TypeR1.States.Count);

            p1.InterceptSet(null, new object[] { "A" }, new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Set2_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);

            TypeR1.States.Clear();

            var v = p1.InterceptGet<string>(null, new object[0], new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Get2_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Intercept_Method()
        {
            var target = new TypeR2();
            var m1 = typeof(TypeR2).GetMethod("MyMethod", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(0, TypeR2.States.Count);

            m1.Intercept(target, new object[] { "A" }, new IntForR2());

            Assert.AreEqual(3, TypeR2.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeR2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR2.States[2]);
            Assert.AreEqual("A", TypeR2.P1);
        }

        [TestMethod]
        public void Intercept_Static_Method()
        {
            var m1 = typeof(TypeR2).GetMethod("MyMethodStatic", BindingFlags.Public | BindingFlags.Static);

            Assert.AreEqual(0, TypeR2.States.Count);

            m1.Intercept(null, new object[] { "B" }, new IntForR2());

            Assert.AreEqual(3, TypeR2.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeR2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR2.States[2]);
            Assert.AreEqual("B", TypeR2.P2);
        }

        [TestMethod]
        public void Intercept_Events()
        {
            bool isCalled = false;
            var target = new TypeR3();
            var m1 = typeof(TypeR3).GetEvent("MyEvent", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(0, TypeR3.States.Count);

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            m1.InterceptAdd(target, new object[] { ev }, new IntForR3());

            Assert.AreEqual(3, TypeR3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR3.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeR3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);
            isCalled = false;

            TypeR3.States.Clear();

            m1.InterceptRemove(target, new object[] { ev }, new IntForR3());

            Assert.AreEqual(3, TypeR3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR3.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeR3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }
    }

    public class TypeR1
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;

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

    public class TypeR2
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public static string P1 { get; set; }
        public static string P2 { get; set; }

        private void MyMethod(string p1)
        {
            P1 = p1;

            States.Add(StateTypes.Class_Method);
        }

        public static void MyMethodStatic(string p2)
        {
            P2 = p2;

            States.Add(StateTypes.Class_Method_2);
        }
    }

    public class TypeR3
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;

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

    public class IntForR1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeR1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeR1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForR2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeR2.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeR2.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForR3 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeR3.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeR3.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
