using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;

namespace NIntercept.Tests
{

    [TestClass]
    public class TypeAccessorTests
    {
        [TestInitialize()]
        public void Startup()
        {

        }

        [TestMethod]
        public void GetFields()
        {
            var accessor = new TypeAccessor(typeof(TypeA0));

            Assert.AreEqual(4, accessor.Fields.Count);
        }

        [TestMethod]
        public void GetField()
        {
            var target = new TypeA0();
            var accessor = new TypeAccessor(target);

            var member = accessor.Fields["myField"];
            member.SetValue("A");
            var v = member.GetValue();

            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Get_Private_Field()
        {
            var target = new TypeA0();
            var accessor = new TypeAccessor(target);

            var member = accessor.Fields["myPrivateField"];
            member.SetValue("B");
            var v = member.GetValue();

            Assert.AreEqual("B", v);
        }

        [TestMethod]
        public void Get_Static_Field()
        {
            var accessor = new TypeAccessor(typeof(TypeA0));

            var member = accessor.Fields["myStaticField"];
            member.SetValue("C");
            var v = member.GetValue();

            Assert.AreEqual("C", v);
        }

        [TestMethod]
        public void Get_Private_Static_Field()
        {
            var accessor = new TypeAccessor(typeof(TypeA0));

            var member = accessor.Fields["myPrivateStaticField"];
            member.SetValue("D");
            var v = member.GetValue();

            Assert.AreEqual("D", v);
        }

        [TestMethod]
        public void GetProperties()
        {
            var accessor = new TypeAccessor(typeof(TypeA1));

            Assert.AreEqual(4, accessor.Properties.Count);
        }

        [TestMethod]
        public void Get_And_Set_Property()
        {
            var target = new TypeA1();
            var accessor = new TypeAccessor(target);

            var member = accessor.Properties["MyProperty"];
            member.SetValue("A");
            Assert.AreEqual("A", member.GetValue<string>());
        }

        [TestMethod]
        public void Get_And_Set_Protected_Property()
        {
            var target = new TypeA1();
            var accessor = new TypeAccessor(target);

            var member = accessor.Properties["MyProtectedProperty"];
            member.SetValue("B");
            Assert.AreEqual("B", member.GetValue<string>());
        }

        [TestMethod]
        public void Get_And_Set_Static_Property()
        {
            var accessor = new TypeAccessor(typeof(TypeA1));

            var member = accessor.Properties["MyStaticProperty"];
            member.SetValue("C");
            Assert.AreEqual("C", member.GetValue<string>());
        }

        [TestMethod]
        public void Get_And_Set_Static_Private_Property()
        {
            var accessor = new TypeAccessor(typeof(TypeA1));

            var member = accessor.Properties["MyStaticPrivateProperty"];
            member.SetValue("D");
            Assert.AreEqual("D", member.GetValue<string>());
        }


        [TestMethod]
        public void ReadOnlyProperty()
        {
            var target = new TypeA3();
            var accessor = new TypeAccessor(target);

            var member = accessor.Properties["MyReadOnlyProperty"];

            Assert.ThrowsException<InvalidOperationException>(() => member.SetValue("A"));
        }

        [TestMethod]
        public void GetMethods()
        {
            var accessor = new TypeAccessor(typeof(TypeA2));

            Assert.AreEqual(3, accessor.Methods.Count);
        }

        [TestMethod]
        public void GetMethod()
        {
            var target = new TypeA2();
            var accessor = new TypeAccessor(target);

            var member = accessor.Methods[1];
            member.Invoke(new object[] { "A" });

            Assert.AreEqual(true, target.MethodBCalled);
            Assert.AreEqual("A", target.P1);
        }

        [TestMethod]
        public void Invoke_Generic_Method_With_Accessor()
        {
            var target = new TypeA2();
            var accessor = new TypeAccessor(target);

            var member = accessor.Methods["Method"][2];

            Assert.IsTrue(member.Method.IsGenericMethod);

            member
                .MakeGenericMethod(new Type[] { typeof(string) })
                .Invoke(new object[] { "B" });

            Assert.AreEqual(false, target.MethodACalled);
            Assert.AreEqual(false, target.MethodBCalled);
            Assert.AreEqual(true, target.MethodCCalled);
            Assert.AreEqual("B", target.P2);
        }

        [TestMethod]
        public void Indexer()
        {
            var target = new MyCollectionI();
            var accessor = new TypeAccessor(target);

            var member = accessor.Properties["Item"];

            accessor.Methods.GetFirstOrDefault("Add").Invoke(new object[] { "A" });

            Assert.AreEqual(1, accessor.Properties["Count"].GetValue<int>());
            var v = member.GetValue<string>(new object[] { 0 });
            Assert.AreEqual("A", v);

            member.SetValue("B", new object[] { 0 });
            v = member.GetValue<string>(new object[] { 0 });

            Assert.AreEqual("B", v);
        }

        [TestMethod]
        public void Get_Parent_Members()
        {
            var target = new B2();
            var accessor = new TypeAccessor(target);

            Assert.AreEqual(1, accessor.Properties.Count);
            Assert.AreEqual("MyProperty", accessor.Properties[0].Name);

            Assert.AreEqual(1, accessor.Methods.Count);
            Assert.AreEqual("Method", accessor.Methods[0].Name);
        }

        [TestMethod]
        public void Add_Remove_Event()
        {
            var target = new TypeA4();
            var accessor = new TypeAccessor(target);
            bool isCalled = false;

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            accessor.Events["MyEvent"].AddEventHandler(ev);

            accessor.Methods["RaiseMyEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(true, isCalled);

            isCalled = false;

            accessor.Events["MyEvent"].RemoveEventHandler(ev);

            accessor.Methods["RaiseMyEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Add_Remove_Private_Event()
        {
            var target = new TypeA4();
            var accessor = new TypeAccessor(target);
            bool isCalled = false;

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            accessor.Events["MyPrivateEvent"].AddEventHandler(ev);

            accessor.Methods["RaiseMyPrivateEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(true, isCalled);

            isCalled = false;

            accessor.Events["MyPrivateEvent"].RemoveEventHandler(ev);

            accessor.Methods["RaiseMyPrivateEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Add_Remove_Static_Event()
        {
            var target = new TypeA4();
            var accessor = new TypeAccessor(target);
            bool isCalled = false;

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            accessor.Events["MyStaticEvent"].AddEventHandler(ev);

            accessor.Methods["RaiseMyStaticEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(true, isCalled);

            isCalled = false;

            accessor.Events["MyStaticEvent"].RemoveEventHandler(ev);

            accessor.Methods["RaiseMyStaticEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Add_Remove_Private_Static_Event()
        {
            var target = new TypeA4();
            var accessor = new TypeAccessor(target);
            bool isCalled = false;

            EventHandler ev = (s, e) =>
            {
                isCalled = true;
            };

            accessor.Events["MyPrivateStaticEvent"].AddEventHandler(ev);

            accessor.Methods["RaiseMyPrivateStaticEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(true, isCalled);

            isCalled = false;

            accessor.Events["MyPrivateStaticEvent"].RemoveEventHandler(ev);

            accessor.Methods["RaiseMyPrivateStaticEvent"][0].Invoke(new object[0]);

            Assert.AreEqual(false, isCalled);
        }
    }

    public class TypeA0
    {
        public string myField;
        private string myPrivateField;
        public static string myStaticField;
        private static string myPrivateStaticField;
    }

    public class TypeA1
    {
        private string myVar;

        public string MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        private string myProtectedVar;

        protected string MyProtectedProperty
        {
            get { return myProtectedVar; }
            set { myProtectedVar = value; }
        }

        private static string myStaticVar;

        public static string MyStaticProperty
        {
            get { return myStaticVar; }
            set { myStaticVar = value; }
        }

        private static string myStaticPrivateVar;

        private static string MyStaticPrivateProperty
        {
            get { return myStaticPrivateVar; }
            set { myStaticPrivateVar = value; }
        }
    }

    public class TypeA2
    {
        public bool MethodACalled { get; set; }
        public bool MethodBCalled { get; set; }
        public bool MethodCCalled { get; set; }

        public string P1 { get; set; }
        public object P2 { get; set; }

        public void Method()
        {
            MethodACalled = true;
        }

        public void Method(string p1)
        {
            P1 = p1;
            MethodBCalled = true;
        }

        public void Method<T>(T p2)
        {
            P2 = p2;
            MethodCCalled = true;
        }
    }

    public class TypeA3
    {
        private string myVar;

        public string MyReadOnlyProperty
        {
            get { return myVar; }
        }
    }

    public class TypeA4
    {
        event EventHandler myEvent;
        public event EventHandler MyEvent
        {
            add { myEvent += value; }
            remove { myEvent -= value; }
        }

        event EventHandler myPrivateEvent;
        private event EventHandler MyPrivateEvent
        {
            add { myPrivateEvent += value; }
            remove { myPrivateEvent -= value; }
        }

        static event EventHandler myStaticEvent;
        public static event EventHandler MyStaticEvent
        {
            add { myStaticEvent += value; }
            remove { myStaticEvent -= value; }
        }

        static event EventHandler myPrivateStaticEvent;
        public static event EventHandler MyPrivateStaticEvent
        {
            add { myPrivateStaticEvent += value; }
            remove { myPrivateStaticEvent -= value; }
        }

        public void RaiseMyEvent()
        {
            myEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseMyPrivateEvent()
        {
            myPrivateEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseMyStaticEvent()
        {
            myStaticEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseMyPrivateStaticEvent()
        {
            myPrivateStaticEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MyCollectionI : Collection<string>
    {

    }

    public class B1
    {
        public string MyProperty { get; set; }

        public void Method()
        {

        }
    }

    public class B2 : B1
    {

    }
}
