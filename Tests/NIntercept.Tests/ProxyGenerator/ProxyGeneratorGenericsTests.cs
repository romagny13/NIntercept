using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace NIntercept.Tests
{
    [TestClass]
    public class ProxyGeneratorGenericsTests
    {
        [TestInitialize()]
        public void Startup()
        {
            MyGenericClass.States.Clear();
            EventGenericClass.States.Clear();
            InterceptorForGeneric1.invocationBefore = null;
            InterceptorForGeneric1.invocationAfter = null;
            MyGenericClass.a = null;
            MyGenericClass.b = null;
            MyGenericClass.c = null;
            MyGenericClass.d = null;
            MyGenericClass.e = null;
            MyGenericClass.f = null;
            MyGenericClass.g = null;
            MyGenericClass.h = null;
        }

        [TestMethod]
        public void Method_Virtual_IsCalled_WithInterceptor()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            proxy.Method<string, int, object, MyItem>();

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);
        }

        [TestMethod]
        public void Method_NotVirtual_IsNotCalled_WithInterceptor()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            proxy.MethodNotVirtual<string, int, object, MyItem>();

            Assert.AreEqual(1, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[0]);
        }

        [TestMethod]
        public void Method_WithGenericsParameters_NoResult()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            proxy.MethodWithGenericsParameters_NoResult(true, 10, "ok", MyEnum.First, "obj", new MyItem { MyProperty = "My String" }, new List<string> { "A", "B" }, 100);

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);

            Assert.AreEqual(true, MyGenericClass.a);
            Assert.AreEqual(10, MyGenericClass.b);
            Assert.AreEqual("ok", MyGenericClass.c);
            Assert.AreEqual(MyEnum.First, MyGenericClass.d);
            Assert.AreEqual("obj", MyGenericClass.e);
            Assert.AreEqual("My String", ((MyItem)MyGenericClass.f).MyProperty);
            Assert.AreEqual("A", ((List<string>)MyGenericClass.g)[0]);
            Assert.AreEqual("B", ((List<string>)MyGenericClass.g)[1]);
            Assert.AreEqual(100, MyGenericClass.h);
        }

        [TestMethod]
        public void Method_WithGenericsAndOtherParameters_NoResult()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            proxy.MethodWithGenericsAndOtherParameters_NoResult(true, 100, "parameter", 2);

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);

            Assert.AreEqual(true, MyGenericClass.a);
            Assert.AreEqual(100, MyGenericClass.b);
            Assert.AreEqual("parameter", MyGenericClass.c);
            Assert.AreEqual(2, MyGenericClass.d);
        }

        [TestMethod]
        public void Method_WithParameters_ValueTypeResult()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            var r = proxy.MethodWithParameters_ValueTypeResult(true, 100, "parameter", 2);

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);

            Assert.AreEqual(true, MyGenericClass.a);
            Assert.AreEqual(100, MyGenericClass.b);
            Assert.AreEqual("parameter", MyGenericClass.c);
            Assert.AreEqual(2, MyGenericClass.d);
            Assert.AreEqual(1000, r);
        }

        [TestMethod]
        public void Method_WithParameters_GenericResult()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            var r = proxy.MethodWithParameters_GenericResult(true, 100, "parameter", 2);

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);

            Assert.AreEqual(true, MyGenericClass.a);
            Assert.AreEqual(100, MyGenericClass.b);
            Assert.AreEqual("parameter", MyGenericClass.c);
            Assert.AreEqual(2, MyGenericClass.d);
            Assert.AreEqual(100, r);
        }

        [TestMethod]
        public void Method_WithParameters_GenericResult_Constraints()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<MyGenericClass>(new InterceptorForGeneric1());

            Assert.AreEqual(0, MyGenericClass.States.Count);

            var r = proxy.MethodWithParameters_GenericResult_Constraint<string, MyItem>("A");

            Assert.AreEqual(3, MyGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyGenericClass.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyGenericClass.States[2]);

            Assert.AreEqual("A", MyGenericClass.a);
            Assert.AreEqual(typeof(MyItem), r.GetType());
        }

        [TestMethod]
        public void Event_Generic_Test()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<EventGenericClass>(new IntForEventGenericClass());

            Assert.AreEqual(0, EventGenericClass.States.Count);

            bool isCalled = false;

            EventHandler<MyEventArgs> ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };

            proxy.MyEvent += ev;

            Assert.AreEqual(3, EventGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, EventGenericClass.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, EventGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, EventGenericClass.States[2]);

            proxy.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            EventGenericClass.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, EventGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, EventGenericClass.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, EventGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, EventGenericClass.States[2]);

            EventGenericClass.States.Clear();
            isCalled = false;

            proxy.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }
    }


    public class MyGenericClass
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual void Method<T1, T2, T3, T4>()
        {
            States.Add(StateTypes.Class_Method);
        }

        public void MethodNotVirtual<T1, T2, T3, T4>()
        {
            States.Add(StateTypes.Class_Method);
        }

        public static object a;
        public static object b;
        public static object c;
        public static object d;
        public static object e;
        public static object f;
        public static object g;
        public static object h;

        public virtual void MethodWithGenericsParameters_NoResult<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h)
        {
            MyGenericClass.a = a;
            MyGenericClass.b = b;
            MyGenericClass.c = c;
            MyGenericClass.d = d;
            MyGenericClass.e = e;
            MyGenericClass.f = f;
            MyGenericClass.g = g;
            MyGenericClass.h = h;

            States.Add(StateTypes.Class_Method);
        }

        public virtual void MethodWithGenericsAndOtherParameters_NoResult<T1, T2>(T1 a, T2 b, string c, int d)
        {
            MyGenericClass.a = a;
            MyGenericClass.b = b;
            MyGenericClass.c = c;
            MyGenericClass.d = d;

            States.Add(StateTypes.Class_Method);
        }

        public virtual int MethodWithParameters_ValueTypeResult<T1, T2>(T1 a, T2 b, string c, int d)
        {
            MyGenericClass.a = a;
            MyGenericClass.b = b;
            MyGenericClass.c = c;
            MyGenericClass.d = d;

            States.Add(StateTypes.Class_Method);

            return 1000;
        }

        public virtual T2 MethodWithParameters_GenericResult<T1, T2>(T1 a, T2 b, string c, int d)
        {
            MyGenericClass.a = a;
            MyGenericClass.b = b;
            MyGenericClass.c = c;
            MyGenericClass.d = d;

            States.Add(StateTypes.Class_Method);

            return b;
        }

        public virtual T2 MethodWithParameters_GenericResult_Constraint<T1, T2>(T1 a) where T2 : class, new()
        {
            MyGenericClass.a = a;

            States.Add(StateTypes.Class_Method);

            return new T2();
        }
    }

    public interface IEventGenericClass
    {
        event EventHandler<MyEventArgs> MyEvent;

        void RaiseMyEvent();
    }

    public class EventGenericClass : IEventGenericClass
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler<MyEventArgs> myEvent;

        public virtual event EventHandler<MyEventArgs> MyEvent
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
            myEvent?.Invoke(this, new MyEventArgs("P1"));
        }
    }


    public class IntForEventGenericClass : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            EventGenericClass.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            EventGenericClass.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }


    public class InterceptorForGeneric1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            MyGenericClass.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            MyGenericClass.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }


    public class BuilderA
    {
        public static List<StateTypes> States = new List<StateTypes>();
        internal static string a;

        public virtual T Create<T>(string parameter) where T : class, new()
        {
            a = parameter;
            BuilderA.States.Add(StateTypes.Class_Method);
            var instance = new T();
            return (T)instance;
        }
    }

    public class InterceptorBuilderA : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            BuilderA.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            BuilderA.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}