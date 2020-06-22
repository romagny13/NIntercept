using NIntercept.Definition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NIntercept.Helpers;

namespace NIntercept.Tests.Helpers
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
        public void Invocation_Has_Unique_Name()
        {
            var collector = new ModuleDefinition();
            var typeDefinition = collector.GetOrAdd(typeof(MyClassN), null, null);

            Assert.AreEqual("MyClassN_Proxy_MethodA_Invocation", typeDefinition.MethodDefinitions[0].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_1_Invocation", typeDefinition.MethodDefinitions[1].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_2_Invocation", typeDefinition.MethodDefinitions[2].InvocationTypeDefinition.Name);
            Assert.AreEqual("MyClassN_Proxy_MethodA_3_Invocation", typeDefinition.MethodDefinitions[3].InvocationTypeDefinition.Name);

            Assert.AreEqual("NIntercept.Invocations.MyClassN_Proxy_MethodA_Invocation", typeDefinition.MethodDefinitions[0].InvocationTypeDefinition.FullName);
            Assert.AreEqual("NIntercept.Invocations.MyClassN_Proxy_MethodA_1_Invocation", typeDefinition.MethodDefinitions[1].InvocationTypeDefinition.FullName);
            Assert.AreEqual("NIntercept.Invocations.MyClassN_Proxy_MethodA_2_Invocation", typeDefinition.MethodDefinitions[2].InvocationTypeDefinition.FullName);
            Assert.AreEqual("NIntercept.Invocations.MyClassN_Proxy_MethodA_3_Invocation", typeDefinition.MethodDefinitions[3].InvocationTypeDefinition.FullName);
        }

        [TestMethod]
        public void et_InvocationType_Name_With_Interface()
        {
            var collector = new ModuleDefinition();
            var typeDefinition = collector.GetOrAdd(typeof(IMyCol1), null, null) as InterfaceProxyDefinition;

            Assert.AreEqual("GetEnumerator", typeDefinition.InterfacesToImplement[0].MethodDefinitions[0].Name);
            Assert.AreEqual("GetEnumerator", typeDefinition.InterfacesToImplement[1].MethodDefinitions[0].Name);

            Assert.AreEqual("IMyCol1_Proxy_IEnumerable`1_GetEnumerator_Invocation", typeDefinition.InterfacesToImplement[0].MethodDefinitions[0].InvocationTypeDefinition.Name);
            Assert.AreEqual("IMyCol1_Proxy_IEnumerable_GetEnumerator_Invocation", typeDefinition.InterfacesToImplement[1].MethodDefinitions[0].InvocationTypeDefinition.Name);
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
