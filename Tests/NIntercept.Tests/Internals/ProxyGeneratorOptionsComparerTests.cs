using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIntercept.Definition;

namespace NIntercept.Tests.Internals
{
    [TestClass]
    public class ProxyGeneratorOptionsComparerTests
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

        // Additional Attributes

        [TestMethod]
        public void AdditonalTypeAttributes_Count_NotEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute).GetConstructors()[0], new object[0]));
            var o2 = new ProxyGeneratorOptions();

            Assert.AreEqual(false, comparer.Equals(o1, o2));

            var o3 = new ProxyGeneratorOptions();
            var o4 = new ProxyGeneratorOptions();
            o4.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute).GetConstructors()[0], new object[0]));
            Assert.AreEqual(false, comparer.Equals(o3, o4));
        }

        [TestMethod]
        public void AdditonalTypeAttributes_Same_Count_Not_Same_Attributes_NotEquals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute).GetConstructors()[0], new object[0]));
            var o2 = new ProxyGeneratorOptions();
            o2.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute2).GetConstructors()[0], new object[0]));

            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void AdditonalTypeAttributes_Same_Count_Same_Attributes_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute).GetConstructors()[0], new object[0]));
            var o2 = new ProxyGeneratorOptions();
            o2.AdditionalTypeAttributes.Add(new CustomAttributeDefinition(typeof(MyAttribute).GetConstructors()[0], new object[0]));

            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }

        // Additional Code

        [TestMethod]
        public void AdditonalCode_Null_And_Not_Null_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalCode = new AdditionalCodeMock1();
            var o2 = new ProxyGeneratorOptions();

            Assert.AreEqual(false, comparer.Equals(o1, o2));

            var o3 = new ProxyGeneratorOptions();
            var o4 = new ProxyGeneratorOptions();
            o4.AdditionalCode = new AdditionalCodeMock1();
            Assert.AreEqual(false, comparer.Equals(o3, o4));
        }

        [TestMethod]
        public void AdditonalCode_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalCode = new AdditionalCodeMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.AdditionalCode = new AdditionalCodeMock2();

            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void AdditonalCode_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.AdditionalCode = new AdditionalCodeMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.AdditionalCode = new AdditionalCodeMock1();

            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }

        // Mixins

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

        // constructor selector

        [TestMethod]
        public void Constructor_Null_And_Not_Null_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ConstructorSelector = new ConstructorSelectorMock1();
            var o2 = new ProxyGeneratorOptions();

            Assert.AreEqual(false, comparer.Equals(o1, o2));

            var o3 = new ProxyGeneratorOptions();
            var o4 = new ProxyGeneratorOptions();
            o4.ConstructorSelector = new ConstructorSelectorMock1();
            Assert.AreEqual(false, comparer.Equals(o3, o4));
        }

        [TestMethod]
        public void Constructor_Not_Same__Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ConstructorSelector = new ConstructorSelectorMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ConstructorSelector = new ConstructorSelectorMock2();

            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void Constructor_Same__Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ConstructorSelector = new ConstructorSelectorMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ConstructorSelector = new ConstructorSelectorMock1();

            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }

        // ClassProxyMemberSelector

        [TestMethod]
        public void MemberSelector_Null_And_Not_Null_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock1();
            var o2 = new ProxyGeneratorOptions();

            Assert.AreEqual(false, comparer.Equals(o1, o2));

            var o3 = new ProxyGeneratorOptions();
            var o4 = new ProxyGeneratorOptions();
            o4.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock1();
            Assert.AreEqual(false, comparer.Equals(o3, o4));
        }

        [TestMethod]
        public void MemberSelector_Not_Same_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock2();

            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void MemberSelector_Same_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ClassProxyMemberSelector = new ClassProxyMemberSelectorMock1();

            Assert.AreEqual(true, comparer.Equals(o1, o2));
        }

        // Service provider

        [TestMethod]
        public void ServiceProvider_Null_And_Not_Null_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ServiceProvider = new ServiceProviderMock1();
            var o2 = new ProxyGeneratorOptions();

            Assert.AreEqual(false, comparer.Equals(o1, o2));

            var o3 = new ProxyGeneratorOptions();
            var o4 = new ProxyGeneratorOptions();
            o4.ServiceProvider = new ServiceProviderMock1();
            Assert.AreEqual(false, comparer.Equals(o3, o4));
        }

        [TestMethod]
        public void ServiceProvider_Not_Same_Not_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ServiceProvider = new ServiceProviderMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ServiceProvider = new ServiceProviderMock2();

            Assert.AreEqual(false, comparer.Equals(o1, o2));
        }

        [TestMethod]
        public void ServiceProvider_Same_Equals()
        {
            var comparer = new ProxyGeneratorOptionsComparer();

            var o1 = new ProxyGeneratorOptions();
            o1.ServiceProvider = new ServiceProviderMock1();
            var o2 = new ProxyGeneratorOptions();
            o2.ServiceProvider = new ServiceProviderMock1();

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
