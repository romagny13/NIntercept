using NIntercept.Definition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NIntercept.Tests
{
    [TestClass]
    public class NamingHelperTests
    {
        [TestMethod]
        public void GetUniqueName()
        {
            var methods = typeof(MyClassN).GetMethods().Where(p => p.Name == "MethodA").ToArray();

            Assert.AreEqual("MethodA", NamingHelper.GetUniqueName(methods[0]));
            Assert.AreEqual("MethodA_1", NamingHelper.GetUniqueName(methods[1]));
            Assert.AreEqual("MethodA_2", NamingHelper.GetUniqueName(methods[2]));
            Assert.AreEqual("MethodA_3", NamingHelper.GetUniqueName(methods[3]));
        }

        [TestMethod]
        public void The_Invocation_Has_Unique_Name()
        {
            var collector = new ModuleDefinition();
            var typeDefinition = collector.GetOrAdd(typeof(MyClassN), null);

            Assert.AreEqual("MyClassN_Proxy_MethodA_Invocation", typeDefinition.MethodDefinitions[0].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_1_Invocation", typeDefinition.MethodDefinitions[1].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_2_Invocation", typeDefinition.MethodDefinitions[2].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_3_Invocation", typeDefinition.MethodDefinitions[3].InvocationTypeDefinition.Name);

            Assert.AreEqual("Interception.Invocations.MyClassN_Proxy_MethodA_Invocation", typeDefinition.MethodDefinitions[0].InvocationTypeDefinition.FullName);
            Assert.AreEqual("Interception.Invocations.MyClassN_Proxy_MethodA_1_Invocation", typeDefinition.MethodDefinitions[1].InvocationTypeDefinition.FullName);
            Assert.AreEqual("Interception.Invocations.MyClassN_Proxy_MethodA_2_Invocation", typeDefinition.MethodDefinitions[2].InvocationTypeDefinition.FullName);
            Assert.AreEqual("Interception.Invocations.MyClassN_Proxy_MethodA_3_Invocation", typeDefinition.MethodDefinitions[3].InvocationTypeDefinition.FullName);
        }

        [TestMethod]
        public void With_Interface()
        {
            var collector = new ModuleDefinition();
            var typeDefinition = collector.GetOrAdd(typeof(IMyCol1), null);

            Assert.AreEqual("GetEnumerator", typeDefinition.TypesToImplement[0].MethodDefinitions[0].Name);
            Assert.AreEqual("GetEnumerator", typeDefinition.TypesToImplement[1].MethodDefinitions[0].Name);

            Assert.AreEqual("IMyCol1_Proxy_IEnumerable`1_GetEnumerator_Invocation", typeDefinition.TypesToImplement[0].MethodDefinitions[0].InvocationTypeDefinition.Name);
            Assert.AreEqual("IMyCol1_Proxy_IEnumerable_GetEnumerator_Invocation", typeDefinition.TypesToImplement[1].MethodDefinitions[0].InvocationTypeDefinition.Name);
        }
    }

    public interface IMyCol1 : IEnumerable<string>
    {

    }

    public class MyColN : IMyCol1
    {
        public IEnumerator<string> GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }

    public class MyClassN
    {
        public virtual void MethodA()
        {

        }

        public virtual void MethodA(string a)
        {

        }

        public virtual void MethodA(string a, int b)
        {

        }

        public virtual void MethodA<T>()
        {

        }
    }
}
