using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIntercept.Tests.Builder;
using System;

namespace NIntercept.Tests.Options
{
    [TestClass]
    public class ProxyGeneratorOptionsTests
    {
        [TestMethod]
        public void Mixin_Without_Interface_Throw()
        {
            var o = new ProxyGeneratorOptions();
            Assert.ThrowsException<ArgumentException>(() => o.AddMixinInstance(new InvalidMixin()));
        }
    }

    public class InvalidMixin
    {

    }
}
