using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace NIntercept.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Serialize_Proxy()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxy<MyClassSer>();

            proxy.Title = "The Title";
            proxy.MyItem = new MyItem { MyProperty = "A" };

            string json = JsonConvert.SerializeObject(proxy);

            Assert.AreEqual("{\"Title\":\"The Title\",\"MyItem\":{\"MyProperty\":\"A\"}}", json);
        }

        [TestMethod]
        public void Populate_Proxy()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxy<MyClassSer>();

            string json = "{\"Title\":\"The Title\",\"MyItem\":{\"MyProperty\":\"A\"}}";
            
            JsonConvert.PopulateObject(json, proxy);

            Assert.AreEqual("The Title", proxy.Title);
            Assert.AreEqual("A", proxy.MyItem.MyProperty);
        }
    }

    public class MyClassSer
    {
        public virtual string Title { get; set; }

        public MyItem MyItem { get; set; }

        public MyClassSer()
        {
          
        }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }
}
