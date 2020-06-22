using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NIntercept.Tests
{
    [TestClass]
    public class AsyncInterceptorClassTests
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

            await proxy.Method1();

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

            await proxy.Method1();

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
                await proxy.Method1();
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
                await proxy.Method1();
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
                await proxy.Method1();
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

    public class TypeAsync1
    {
        public bool IsCalled { get; set; }
        public bool Throw { get; set; }

        public virtual async Task Method1()
        {
            await Task.Delay(1000);

            IsCalled = true;
            if (Throw)
                throw new Exception("Method");
        }

        public virtual async Task<string> Method2()
        {
            await Task.Delay(1000);

            IsCalled = true;
            if (Throw)
                throw new Exception("Method");

            return "Ok";
        }
    }

    public class IntForAsync1 : AsyncInterceptor
    {
        public List<StateTypes> States = new List<StateTypes>();

        public bool ThrowBefore = false;
        public bool ThrowAfter = false;
        public Exception Ex { get; set; }

        public void Reset()
        {
            States.Clear();
            ThrowBefore = false;
            ThrowAfter = false;
            Ex = null;
        }

        protected override void OnEnter(IInvocation invocation)
        {
            States.Add(StateTypes.Enter_Interceptor);
            if (ThrowBefore)
                throw new Exception("Before");
        }

        protected override void OnExit(IInvocation invocation)
        {
            States.Add(StateTypes.Exit_Interceptor);
            if (ThrowAfter)
                throw new Exception("After");
        }

        protected override void OnException(IInvocation invocation, Exception exception)
        {
            States.Add(StateTypes.Exception_Interceptor);
            this.Ex = exception;
        }
    }
}
