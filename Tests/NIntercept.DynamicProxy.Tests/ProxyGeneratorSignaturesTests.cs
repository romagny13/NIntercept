using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIntercept.Tests
{
    [TestClass]
    public class ProxyGeneratorMultiSignatureMethodsTests
    {
        [TestInitialize()]
        public void Startup()
        {
            MyClassMultiSignatures.States.Clear();
        }

        [TestMethod]
        public void Methods_With_Mutliple_Signatures()
        {
            var proxyGenerator = new ProxyGenerator();

            var proxy = proxyGenerator.CreateClassProxy<MyClassMultiSignatures>(new InterceptorPrivate1());

            Assert.AreEqual(0, MyClassMultiSignatures.States.Count);

            proxy.Method();

            Assert.AreEqual(3, MyClassMultiSignatures.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyClassMultiSignatures.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyClassMultiSignatures.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyClassMultiSignatures.States[2]);

            MyClassMultiSignatures.States.Clear();

            proxy.Method("A");

            Assert.AreEqual(3, MyClassMultiSignatures.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyClassMultiSignatures.States[0]);
            Assert.AreEqual(StateTypes.Class_Method_2, MyClassMultiSignatures.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyClassMultiSignatures.States[2]);
        }

        [TestMethod]
        public void GenericMethod_IsNot_Ignored_With_SameNames()
        {
            var proxyGenerator = new ProxyGenerator();

            var proxy = proxyGenerator.CreateClassProxy<MyClassMultiSignatures>(new InterceptorPrivate1());

            Assert.AreEqual(0, MyClassMultiSignatures.States.Count);

            proxy.Method<string, string, int, bool>("a", "b", 10, true);

            Assert.AreEqual(3, MyClassMultiSignatures.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyClassMultiSignatures.States[0]);
            Assert.AreEqual(StateTypes.Class_Method_4, MyClassMultiSignatures.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyClassMultiSignatures.States[2]);
        }
    }

    public class MyClassMultiSignatures
    {
        public static List<StateTypes> States = new List<StateTypes>();
        public static string a;

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }

        public virtual void Method(string a)
        {
            MyClassMultiSignatures.a = a;
            States.Add(StateTypes.Class_Method_2);
        }

        public virtual void Method(string a, int b, bool c)
        {
            States.Add(StateTypes.Class_Method_3);
        }

        public virtual void Method<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            States.Add(StateTypes.Class_Method_4);
        }
    }

    public class InterceptorPrivate1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            MyClassMultiSignatures.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            MyClassMultiSignatures.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
