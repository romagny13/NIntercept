using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NIntercept.Tests
{
    [TestClass]
    public class TypeAccessorInterceptionTests
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
            var accessor = new TypeAccessor(target);
            var p1 = accessor.Properties["MyProperty"];

            Assert.AreEqual(0, TypeR1.States.Count);

            p1.InterceptSet(new object[] { "A" }, new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);

            TypeR1.States.Clear();

            var v = p1.InterceptGet<string>(new object[0], new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Intercept_Static_Property()
        {
            var accessor = new TypeAccessor(typeof(TypeR1));
            var p1 = accessor.Properties["MyPropertyStatic"];

            Assert.AreEqual(0, TypeR1.States.Count);

            p1.InterceptSet(new object[] { "A" }, new IntForR1());

            Assert.AreEqual(3, TypeR1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR1.States[0]);
            Assert.AreEqual(StateTypes.Set2_Called, TypeR1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR1.States[2]);

            TypeR1.States.Clear();

            var v = p1.InterceptGet<string>(new object[0], new IntForR1());

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
            var accessor = new TypeAccessor(target);
            var m1 = accessor.Methods["MyMethod"][0];

            Assert.AreEqual(0, TypeR2.States.Count);

            m1.Intercept(new object[] { "A" }, new IntForR2());

            Assert.AreEqual(3, TypeR2.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeR2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR2.States[2]);
            Assert.AreEqual("A", TypeR2.P1);
        }

        [TestMethod]
        public void Intercept_Static_Method()
        {
            var accessor = new TypeAccessor(typeof(TypeR2));
            var m1 = accessor.Methods["MyMethodStatic"][0];

            Assert.AreEqual(0, TypeR2.States.Count);

            m1.Intercept(new object[] { "B" }, new IntForR2());

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
            var accessor = new TypeAccessor(target);
            var m1 = accessor.Events["MyEvent"];

            Assert.AreEqual(0, TypeR3.States.Count);

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            m1.InterceptAdd(new object[] { ev }, new IntForR3());

            Assert.AreEqual(3, TypeR3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR3.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeR3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);
            isCalled = false;

            TypeR3.States.Clear();

            m1.InterceptRemove(new object[] { ev }, new IntForR3());

            Assert.AreEqual(3, TypeR3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeR3.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeR3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeR3.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }
    }

}
