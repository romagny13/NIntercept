using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NIntercept.Tests.Internals
{
    [TestClass]
    public class InvocationTypeRegistryTests
    {
        [TestMethod]
        public void Retunrs_Same_Type_For_Same_Name()
        {
            var registry = new InvocationTypeRegistry();

            var type = typeof(TypeITD);
            registry.Add("A", type);

            Assert.AreEqual(type, registry.GetBuildType("A"));
        }

        [TestMethod]
        public void Retunrs_Not_Same_Type_For_No_Same_Name()
        {
            var registry = new InvocationTypeRegistry();

            var type = typeof(TypeITD);
            registry.Add("A", type);

            Assert.AreEqual(null, registry.GetBuildType("B"));
        }
    }
}
