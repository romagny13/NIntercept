using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept.Tests
{

    [TestClass]
    public class DoublonTests
    {
        [TestMethod]
        public void Do_Not_Create_Members_Mutliple_Times_With_Interfaces()
        {
            var generator = new ProxyGenerator();
            var target = new TypeIDoublon1();
            var proxy = generator.CreateInterfaceProxyWithTarget<ICs>(target);

            Assert.AreEqual(1, proxy.GetType().GetProperties().Length);
            Assert.AreEqual(1, proxy.GetType().GetMethods().Where(p => p.Name == "Method").Count());
            Assert.AreEqual(1, proxy.GetType().GetEvents().Length);
        }
    }

    public interface IC1
    {
        string MyProperty { get; set; }

        void Method<T>(string p1);

        event EventHandler MyEvent;
    }

    public interface IC2
    {
        [SetterInterceptor(typeof(IntForIC1))]
        string MyProperty { get; set; }

        void Method<T>(string p1);

        event EventHandler MyEvent;
    }

    public interface ICs : IC1, IC2
    {

    }

    public class TypeIDoublon1 : ICs
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public string MyProperty { get; set; }

        public event EventHandler MyEvent;

        public void Method<T>(string p1)
        {
           
        }
    }

    public class IntForDoublon1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeIDoublon1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeIDoublon1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
