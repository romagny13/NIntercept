using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NIntercept.Tests
{
    [TestClass]
    public class AsyncInterceptorWithResultClassTests
    {
        [TestInitialize()]
        public void Startup()
        {

        }

        [TestMethod]
        public async Task CancelBefore()
        {
            var generator = new ProxyGenerator();
            var interceptor = new IntForAsync1();
            var target = new TypeAsync1();
            var proxy = generator.CreateClassProxyWithTarget<TypeAsync1>(target, interceptor);

            interceptor.ThrowBefore = true;

            Assert.AreEqual(0, interceptor.States.Count);

            await proxy.Method2();

            Assert.AreEqual(3, interceptor.States.Count);
            Assert.AreEqual(StateTypes.Enter_Interceptor, interceptor.States[0]);
            Assert.AreEqual(StateTypes.Exception_Interceptor, interceptor.States[1]);
            Assert.AreEqual("Before", interceptor.Ex.Message);
            Assert.AreEqual(StateTypes.Exit_Interceptor, interceptor.States[2]);
        }

        [TestMethod]
        public async Task CancelAfter()
        {
            var generator = new ProxyGenerator();
            var interceptor = new IntForAsync1();
            var target = new TypeAsync1();
            var proxy = generator.CreateClassProxyWithTarget<TypeAsync1>(target, interceptor);

            interceptor.ThrowAfter = true;

            Assert.AreEqual(0, interceptor.States.Count);

            await proxy.Method2();

            Assert.AreEqual(3, interceptor.States.Count);
            Assert.AreEqual(StateTypes.Enter_Interceptor, interceptor.States[0]);
            Assert.AreEqual(StateTypes.Exit_Interceptor, interceptor.States[1]);
            Assert.AreEqual(StateTypes.Exception_Interceptor, interceptor.States[2]);
            Assert.AreEqual("After", interceptor.Ex.Message);
        }

        [TestMethod]
        public async Task Throw_Before()
        {
            var generator = new ProxyGenerator();
            var interceptor = new IntForAsync1();
            var target = new TypeAsync1();
            var proxy = generator.CreateClassProxyWithTarget<TypeAsync1>(target, interceptor);

            interceptor.ThrowBefore = true;
            interceptor.Throw = true;

            Assert.AreEqual(0, interceptor.States.Count);

            bool isThrown = false;
            try
            {
                await proxy.Method2();
            }
            catch (Exception)
            {
                isThrown = true;
            }

            Assert.IsTrue(isThrown);
            Assert.IsFalse(target.IsCalled);
            Assert.AreEqual(2, interceptor.States.Count);
            Assert.AreEqual(StateTypes.Enter_Interceptor, interceptor.States[0]);
            Assert.AreEqual(StateTypes.Exception_Interceptor, interceptor.States[1]);
            Assert.AreEqual("Before", interceptor.Ex.Message);
        }

        [TestMethod]
        public async Task Throw_Method()
        {
            var generator = new ProxyGenerator();
            var interceptor = new IntForAsync1();
            var target = new TypeAsync1();
            var proxy = generator.CreateClassProxyWithTarget<TypeAsync1>(target, interceptor);

            target.Throw = true;
            interceptor.Throw = true;

            Assert.AreEqual(0, interceptor.States.Count);

            bool isThrown = false;
            try
            {
                await proxy.Method2();
            }
            catch (Exception)
            {
                isThrown = true;
            }

            Assert.IsTrue(isThrown);
            Assert.IsTrue(target.IsCalled);
            Assert.AreEqual(2, interceptor.States.Count);
            Assert.AreEqual(StateTypes.Enter_Interceptor, interceptor.States[0]);
            Assert.AreEqual(StateTypes.Exception_Interceptor, interceptor.States[1]);
            Assert.AreEqual("Method", interceptor.Ex.Message);
        }

        [TestMethod]
        public async Task Throw_After()
        {
            var generator = new ProxyGenerator();
            var interceptor = new IntForAsync1();
            var target = new TypeAsync1();
            var proxy = generator.CreateClassProxyWithTarget<TypeAsync1>(target, interceptor);

            interceptor.ThrowAfter = true;
            interceptor.Throw = true;

            Assert.AreEqual(0, interceptor.States.Count);

            bool isThrown = false;
            try
            {
                await proxy.Method2();
            }
            catch (Exception)
            {
                isThrown = true;
            }

            Assert.IsTrue(isThrown);
            Assert.IsTrue(target.IsCalled);
            Assert.AreEqual(3, interceptor.States.Count);
            Assert.AreEqual(StateTypes.Enter_Interceptor, interceptor.States[0]);
            Assert.AreEqual(StateTypes.Exit_Interceptor, interceptor.States[1]);
            Assert.AreEqual(StateTypes.Exception_Interceptor, interceptor.States[2]);
            Assert.AreEqual("After", interceptor.Ex.Message);
        }
    }
}
