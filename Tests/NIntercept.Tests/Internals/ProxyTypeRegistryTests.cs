using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NIntercept.Tests.Internals
{
    [TestClass]
    public class ProxyTypeRegistryTests
    {
        [TestMethod]
        public void Returns_Same_Type_For_Same_Name_And_Options_Null()
        {
            var registry = new ProxyTypeRegistry();

            var type = typeof(TypeITD);
            registry.Add("A", null, type);

            Assert.AreEqual(type, registry.GetBuidType("A", null));
        }

        [TestMethod]
        public void Returns_Same_Type_For_Same_Name_And_Same_Options_Type()
        {
            var registry = new ProxyTypeRegistry();

            var type = typeof(TypeITD);
            var o = new ProxyGeneratorOptions();
            o.AddMixinInstance(new TypeITD2());

            registry.Add("A", o, type);

            var o2 = new ProxyGeneratorOptions();
            o2.AddMixinInstance(new TypeITD2());


            Assert.AreEqual(type, registry.GetBuidType("A", o2));
        }

        [TestMethod]
        public void Returns_Not_Same_Type_For_Same_Name_And_Not_Same_Options_Type()
        {
            var registry = new ProxyTypeRegistry();

            var type = typeof(TypeITD);
            var o = new ProxyGeneratorOptions();
            o.AddMixinInstance(new TypeITD2());

            registry.Add("A", o, type);

            var o2 = new ProxyGeneratorOptions();
            o2.AddMixinInstance(new TypeITD3());

            Assert.AreEqual(null, registry.GetBuidType("A", o2));
        }
    }
}
