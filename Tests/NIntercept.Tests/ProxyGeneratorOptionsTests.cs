using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NIntercept.Tests
{

    // TODO: test generic events
    [TestClass]
    public class ProxyGeneratorOptionsTests
    {
        [TestMethod]
        public void NullEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();
            Assert.AreEqual(true, comparer.Equals(null, null));
        }

        [TestMethod]
        public void NullAndNotNullNotEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o = new ProxyGeneratorOptions();
            Assert.AreEqual(false, comparer.Equals(o, null));
            Assert.AreEqual(false, comparer.Equals(null, o));
        }

        [TestMethod]
        public void EmptyOptionsEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            var o2 = new ProxyGeneratorOptions();
            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void MixinNumberNotEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AddMixinInstance(new TypeITD());
            var o2 = new ProxyGeneratorOptions();
            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void SameMxinsTypeNotEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AddMixinInstance(new TypeITD());
            var o2 = new ProxyGeneratorOptions();
            o2.AddMixinInstance(new TypeITD());
            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }
    }

    public class TypeITD
    {
        public virtual void Method()
        {

        }
    }

    public class TypeITD2
    {
    }

    public class TypeITD3
    {
    }
}
