using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace NIntercept.Tests.Builder
{
    // verify attributes :
    // added for type => with interface
    // NOT added for type (inherit)
    // added on get and set method for properties (interface / type)
    // added on add and remove method for events (interface / type)
    // added on method for methods (interface)
    // NOT added on method for methods (type / inherit)

    [TestClass]
    public class AttributePresenceTests
    {
        [TestMethod]
        public void Attributes_Not_Added_For_Type()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var typeAtt = proxyType.GetCustomAttributes();
            Assert.AreEqual(3, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(XmlIncludeAttribute), typeAtt.ElementAt(1).GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(2).GetType());
            Assert.AreEqual(typeof(IntForTypeA1), ((IInterceptorProvider)typeAtt.ElementAt(2)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Interface()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var typeAtt = proxyType.GetCustomAttributes();
            Assert.AreEqual(3, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(XmlIncludeAttribute), typeAtt.ElementAt(1).GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(2).GetType());
            Assert.AreEqual(typeof(IntForTypeA2), ((IInterceptorProvider)typeAtt.ElementAt(2)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Interface_With_Ancestors()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2_b), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var typeAtt = proxyType.GetCustomAttributes();
            Assert.AreEqual(3, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(XmlIncludeAttribute), typeAtt.ElementAt(1).GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(2).GetType());
            Assert.AreEqual(typeof(IntForTypeA2), ((IInterceptorProvider)typeAtt.ElementAt(2)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Set_And_Get_Properties()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var getAtt = proxyType.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            Assert.AreEqual(2, getAtt.Count());
            Assert.AreEqual(typeof(GetterInterceptor), getAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Get), ((IInterceptorProvider)getAtt.ElementAt(0)).InterceptorType);
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), getAtt.ElementAt(1).GetType());

            var setAtt = proxyType.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            Assert.AreEqual(2, setAtt.Count());
            Assert.AreEqual(typeof(SetterInterceptor), setAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Set), ((IInterceptorProvider)setAtt.ElementAt(0)).InterceptorType);
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), setAtt.ElementAt(1).GetType());
        }

        [TestMethod]
        public void Attributes_Added_For_Set_And_Get_Properties_Full()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1_Full), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var getAtt = proxyType.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            Assert.AreEqual(1, getAtt.Count());
            Assert.AreEqual(typeof(GetterInterceptor), getAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Get), ((IInterceptorProvider)getAtt.ElementAt(0)).InterceptorType);

            var setAtt = proxyType.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            Assert.AreEqual(1, setAtt.Count());
            Assert.AreEqual(typeof(SetterInterceptor), setAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Set), ((IInterceptorProvider)setAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Set_And_Get_Properties_For_Interface()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var getAtt = proxyType.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            Assert.AreEqual(1, getAtt.Count());
            Assert.AreEqual(typeof(GetterInterceptor), getAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Get), ((IInterceptorProvider)getAtt.ElementAt(0)).InterceptorType);

            var setAtt = proxyType.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            Assert.AreEqual(1, setAtt.Count());
            Assert.AreEqual(typeof(SetterInterceptor), setAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Set), ((IInterceptorProvider)setAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Set_And_Get_Properties_For_Interface_With_Ancestor()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2_b), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var getAtt = proxyType.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            Assert.AreEqual(1, getAtt.Count());
            Assert.AreEqual(typeof(GetterInterceptor), getAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Get), ((IInterceptorProvider)getAtt.ElementAt(0)).InterceptorType);

            var setAtt = proxyType.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            Assert.AreEqual(1, setAtt.Count());
            Assert.AreEqual(typeof(SetterInterceptor), setAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Set), ((IInterceptorProvider)setAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Add_And_Remove_Events()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var addAtt = proxyType.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            Assert.AreEqual(2, addAtt.Count());
            Assert.AreEqual(typeof(AddOnInterceptorAttribute), addAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Add), ((IInterceptorProvider)addAtt.ElementAt(0)).InterceptorType);
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), addAtt.ElementAt(1).GetType());

            var removeAtt = proxyType.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();
            Assert.AreEqual(2, removeAtt.Count());
            Assert.AreEqual(typeof(RemoveOnInterceptorAttribute), removeAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Remove), ((IInterceptorProvider)removeAtt.ElementAt(0)).InterceptorType);
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), removeAtt.ElementAt(1).GetType());
        }

        [TestMethod]
        public void Attributes_Added_For_Add_And_Remove_Events_Full()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1_Full), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var addAtt = proxyType.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            Assert.AreEqual(1, addAtt.Count());
            Assert.AreEqual(typeof(AddOnInterceptorAttribute), addAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Add), ((IInterceptorProvider)addAtt.ElementAt(0)).InterceptorType);

            var removeAtt = proxyType.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();
            Assert.AreEqual(1, removeAtt.Count());
            Assert.AreEqual(typeof(RemoveOnInterceptorAttribute), removeAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Remove), ((IInterceptorProvider)removeAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Add_And_Remove_Events_For_Interface()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var addAtt = proxyType.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            Assert.AreEqual(1, addAtt.Count());
            Assert.AreEqual(typeof(AddOnInterceptorAttribute), addAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Add), ((IInterceptorProvider)addAtt.ElementAt(0)).InterceptorType);

            var removeAtt = proxyType.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();
            Assert.AreEqual(1, removeAtt.Count());
            Assert.AreEqual(typeof(RemoveOnInterceptorAttribute), removeAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Remove), ((IInterceptorProvider)removeAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Added_For_Add_And_Remove_Events_For_Interface_With_Ancestor()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2_b), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var addAtt = proxyType.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            Assert.AreEqual(1, addAtt.Count());
            Assert.AreEqual(typeof(AddOnInterceptorAttribute), addAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Add), ((IInterceptorProvider)addAtt.ElementAt(0)).InterceptorType);

            var removeAtt = proxyType.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();
            Assert.AreEqual(1, removeAtt.Count());
            Assert.AreEqual(typeof(RemoveOnInterceptorAttribute), removeAtt.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Remove), ((IInterceptorProvider)removeAtt.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Not_Added_For_Method()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(TypeA1), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var att = proxyType.GetMethod("MyMethod").GetCustomAttributes();
            Assert.AreEqual(1, att.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), att.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Method), ((IInterceptorProvider)att.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Not_Added_For_Method_For_Interface()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var att = proxyType.GetMethod("MyMethod").GetCustomAttributes();
            Assert.AreEqual(1, att.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), att.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Method), ((IInterceptorProvider)att.ElementAt(0)).InterceptorType);
        }

        [TestMethod]
        public void Attributes_Not_Added_For_Method_For_Interface_With_Ancestor()
        {
            var generator = new ProxyGenerator();
            var typeDefiniton = generator.ModuleDefinition.GetTypeDefinition(typeof(ITypeA2_b), null, null);
            Type proxyType = generator.CreateProxyType(typeDefiniton, null);

            var att = proxyType.GetMethod("MyMethod").GetCustomAttributes();
            Assert.AreEqual(1, att.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), att.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForTypeA_Method), ((IInterceptorProvider)att.ElementAt(0)).InterceptorType);
        }
    }

    [AllInterceptor(typeof(IntForTypeA1))]
    public class TypeA1
    {
        [GetterInterceptor(typeof(IntForTypeA_Get))]
        [SetterInterceptor(typeof(IntForTypeA_Set))]
        public virtual string MyProperty { get; set; }


        [AddOnInterceptor(typeof(IntForTypeA_Add))]
        [RemoveOnInterceptor(typeof(IntForTypeA_Remove))]
        public virtual event EventHandler MyEvent;

        [MethodInterceptor(typeof(IntForTypeA_Method))]
        public virtual void MyMethod()
        {

        }
    }

    [AllInterceptor(typeof(IntForTypeA1))]
    public class TypeA1_Full
    {
        private string myVar;

        [GetterInterceptor(typeof(IntForTypeA_Get))]
        [SetterInterceptor(typeof(IntForTypeA_Set))]
        public virtual string MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        private event EventHandler myEvent;
        [AddOnInterceptor(typeof(IntForTypeA_Add))]
        [RemoveOnInterceptor(typeof(IntForTypeA_Remove))]
        public virtual event EventHandler MyEvent
        {
            add { myEvent += value; }
            remove { myEvent -= value; }
        }

    }

    [AllInterceptor(typeof(IntForTypeA2))]
    public interface ITypeA2
    {
        [GetterInterceptor(typeof(IntForTypeA_Get))]
        [SetterInterceptor(typeof(IntForTypeA_Set))]
        string MyProperty { get; set; }


        [AddOnInterceptor(typeof(IntForTypeA_Add))]
        [RemoveOnInterceptor(typeof(IntForTypeA_Remove))]
        event EventHandler MyEvent;

        [MethodInterceptor(typeof(IntForTypeA_Method))]
        void MyMethod();
    }

    public interface ITypeA2_b : ITypeA2
    { }

    public class IntForTypeA1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA_Get : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA_Set : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA_Add : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA_Remove : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA_Method : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForTypeA2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}
