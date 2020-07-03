using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Unity;

namespace NIntercept.Tests
{


    [TestClass]
    public class ProxyGeneratorTests
    {
        [TestInitialize()]
        public void Startup()
        {
            TypeP1.States.Clear();
            TypeP3.States.Clear();
            TypeP4.States.Clear();
            TypeP5.States.Clear();
            TypeP6.States.Clear();
            TypeP7.States.Clear();
            TypeP8.States.Clear();
            TypeP9.States.Clear();
            TypeP10.States.Clear();
            TypeP11.States.Clear();
            TypeP13.States.Clear();
        }

        [TestMethod]
        public void IsInitialized()
        {
            var generator = new ProxyGenerator();

            Assert.IsNotNull(generator.ProxyBuilder);
            Assert.IsNotNull(generator.ModuleDefinition);
            Assert.IsNotNull(generator.ProxyBuilder.ModuleScope);
            Assert.IsNotNull(generator.ProxyBuilder.ProxyEventBuilder);
            Assert.IsNotNull(generator.ProxyBuilder.ProxyMethodBuilder);
            Assert.IsNotNull(generator.ProxyBuilder.ProxyPropertyBuilder);

            var scope = generator.ProxyBuilder.ModuleScope;
            Assert.AreEqual("NIntercept.DynamicAssembly", scope.AssemblyName);
            Assert.AreEqual("NIntercept.DynamicModule", scope.ModuleName);
        }

        [TestMethod]
        public void Change_Services()
        {
            var generator = new ProxyGenerator();

            var m1 = new TypeDefintionCollectorMock();
            var m2 = new ProxyPropertyBuilderMock();
            var m3 = new ProxyMethodBuilderMock();
            var m4 = new ProxyEventBuilderMock();
            var m5 = new CallbackMethodBuilderMock();
            var m6 = new InvocationTypeBuilderMock();

            var mock = new ServiceLocatorMock();

            ProxyServiceLocator.SetLocatorProvider(() => mock);

            generator.ModuleDefinition = m1;
            mock.ProxyPropertyBuilder = m2;
            mock.ProxyMethodBuilder = m3;
            mock.ProxyEventBuilder = m4;
            mock.CallbackMethodBuilder = m5;
            mock.InvocationTypeBuilder = m6;


            generator.CreateClassProxy<TypeP2>();

            Assert.IsTrue(m1.IsUsed);
            Assert.IsTrue(m2.IsUsed);
            Assert.IsTrue(m3.IsUsed);
            Assert.IsTrue(m4.IsUsed);
            Assert.IsTrue(m5.IsUsed);
            Assert.IsTrue(m6.IsUsed);

            ProxyServiceLocator.SetLocatorProvider(() => new DefaultServiceProvider());
        }

        #region Methods

        [TestMethod]
        public void Call_Empty_Method()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeP1>(new IntForP1());

            Assert.AreEqual(0, TypeP1.States.Count);

            proxy.MethodA();

            Assert.AreEqual(3, TypeP1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP1.States[2]);
        }

        [TestMethod]
        public void Call_Method_With_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<TypeP1>(new IntForP1());

            Assert.AreEqual(0, TypeP1.States.Count);

            var d = DateTime.Now;
            var dt = new DateTimeOffset(d);
            var uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
            var ts = TimeSpan.FromSeconds(100);
            var g = Guid.NewGuid();

            proxy.Parameters(-1, -10, -100, 1, 10, 100, "A", 1.5F, 2.5, 3.5M, 1, 2, true, d, dt, uri, ts, g, 'c', MyEnum.First, "Obj", new MyItem { MyProperty = "A" }, new List<string> { "A", "B" }, new string[] { "A", "B" }, new MyCollection());

            Assert.AreEqual(TypeP1.MyShort, (short)-1);
            Assert.AreEqual(TypeP1.MyInt, (int)-10);
            Assert.AreEqual(TypeP1.MyLong, (long)-100);
            Assert.AreEqual(TypeP1.MyUShort, (ushort)1);
            Assert.AreEqual(TypeP1.MyUInt, (uint)10);
            Assert.AreEqual(TypeP1.MyULong, (ulong)100);
            Assert.AreEqual(TypeP1.MyString, "A");
            Assert.AreEqual(TypeP1.MyFloat, 1.5F);
            Assert.AreEqual(TypeP1.MyDouble, 2.5);
            Assert.AreEqual(TypeP1.MyDecimal, 3.5M);
            Assert.AreEqual(TypeP1.MySbyte, (sbyte)1);
            Assert.AreEqual(TypeP1.MyByte, (byte)2);
            Assert.AreEqual(TypeP1.MyBool, true);
            Assert.AreEqual(TypeP1.MyDateTime, d);
            Assert.AreEqual(TypeP1.MyDateTimeOffset, dt);
            Assert.AreEqual(TypeP1.MyUri, uri);
            Assert.AreEqual(TypeP1.MyTimeSpan, ts);
            Assert.AreEqual(TypeP1.MyGuid, g);
            Assert.AreEqual(TypeP1.MyChar, 'c');
            Assert.AreEqual(TypeP1.MyEnum, MyEnum.First);
            Assert.AreEqual(TypeP1.MyObj, "Obj");
            Assert.AreEqual(TypeP1.MyItem.MyProperty, "A");
            Assert.AreEqual(TypeP1.MyList.Count, 2);
            Assert.AreEqual(TypeP1.MyArray.Length, 2);
            Assert.IsNotNull(TypeP1.MyCollection);

            Assert.AreEqual(3, TypeP1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP1.States[2]);
        }


        [TestMethod]
        public void Out_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<TypeP5>(new IntForP5());

            Assert.AreEqual(0, TypeP5.States.Count);

            short MyShort;
            int MyInt;
            long MyLong;
            ushort MyUShort;
            uint MyUInt;
            ulong MyULong;
            string MyString;
            float MyFloat;
            double MyDouble;
            decimal MyDecimal;
            sbyte MySbyte;
            byte MyByte;
            bool MyBool;
            DateTime MyDateTime;
            DateTimeOffset MyDateTimeOffset;
            Uri MyUri;
            TimeSpan MyTimeSpan;
            Guid MyGuid;
            char MyChar;
            MyEnum MyEnum;
            object MyObj;
            MyItem MyItem;
            List<string> MyList;
            string[] MyArray;
            MyCollection MyCollection;

            proxy.Parameters(
             out MyShort,
             out MyInt,
             out MyLong,
             out MyUShort,
             out MyUInt,
             out MyULong,
             out MyString,
             out MyFloat,
             out MyDouble,
             out MyDecimal,
             out MySbyte,
             out MyByte,
             out MyBool,
             out MyDateTime,
             out MyDateTimeOffset,
             out MyUri,
             out MyTimeSpan,
             out MyGuid,
             out MyChar,
             out MyEnum,
             out MyObj,
             out MyItem,
             out MyList,
             out MyArray,
             out MyCollection);

            Assert.AreEqual(MyShort, (short)-1);
            Assert.AreEqual(MyInt, (int)-10);
            Assert.AreEqual(MyLong, (long)-100);
            Assert.AreEqual(MyUShort, (ushort)1);
            Assert.AreEqual(MyUInt, (uint)10);
            Assert.AreEqual(MyULong, (ulong)100);
            Assert.AreEqual(MyString, "A");
            Assert.AreEqual(MyFloat, 1.5F);
            Assert.AreEqual(MyDouble, 2.5);
            Assert.AreEqual(MyDecimal, 3.5M);
            Assert.AreEqual(MySbyte, (sbyte)1);
            Assert.AreEqual(MyByte, (byte)2);
            Assert.AreEqual(MyBool, true);
            Assert.AreEqual(MyDateTime, TypeP5.d);
            Assert.AreEqual(MyDateTimeOffset, TypeP5.dt);
            Assert.AreEqual(MyUri, TypeP5.uri);
            Assert.AreEqual(MyTimeSpan, TypeP5.ts);
            Assert.AreEqual(MyGuid, TypeP5.g);
            Assert.AreEqual(MyChar, 'c');
            Assert.AreEqual(MyEnum, MyEnum.First);
            Assert.AreEqual(MyObj, "Obj");
            Assert.AreEqual(MyItem.MyProperty, "A");
            Assert.AreEqual(MyList.Count, 2);
            Assert.AreEqual(MyArray.Length, 2);
            Assert.IsNotNull(MyCollection);

            Assert.AreEqual(3, TypeP5.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP5.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP5.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP5.States[2]);
        }

        [TestMethod]
        public void Ref_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<TypeP6>(new IntForP6());

            Assert.AreEqual(0, TypeP6.States.Count);

            short MyShort = default;
            int MyInt = default;
            long MyLong = default;
            ushort MyUShort = default;
            uint MyUInt = default;
            ulong MyULong = default;
            string MyString = default;
            float MyFloat = default;
            double MyDouble = default;
            decimal MyDecimal = default;
            sbyte MySbyte = default;
            byte MyByte = default;
            bool MyBool = default;
            DateTime MyDateTime = default;
            DateTimeOffset MyDateTimeOffset = default;
            Uri MyUri = default;
            TimeSpan MyTimeSpan = default;
            Guid MyGuid = default;
            char MyChar = default;
            MyEnum MyEnum = default;
            object MyObj = default;
            MyItem MyItem = default;
            List<string> MyList = default;
            string[] MyArray = default;
            MyCollection MyCollection = default;

            proxy.Parameters(
             ref MyShort,
             ref MyInt,
             ref MyLong,
             ref MyUShort,
             ref MyUInt,
             ref MyULong,
             ref MyString,
             ref MyFloat,
             ref MyDouble,
             ref MyDecimal,
             ref MySbyte,
             ref MyByte,
             ref MyBool,
             ref MyDateTime,
             ref MyDateTimeOffset,
             ref MyUri,
             ref MyTimeSpan,
             ref MyGuid,
             ref MyChar,
             ref MyEnum,
             ref MyObj,
             ref MyItem,
             ref MyList,
             ref MyArray,
             ref MyCollection);

            Assert.AreEqual(MyShort, (short)-1);
            Assert.AreEqual(MyInt, (int)-10);
            Assert.AreEqual(MyLong, (long)-100);
            Assert.AreEqual(MyUShort, (ushort)1);
            Assert.AreEqual(MyUInt, (uint)10);
            Assert.AreEqual(MyULong, (ulong)100);
            Assert.AreEqual(MyString, "A");
            Assert.AreEqual(MyFloat, 1.5F);
            Assert.AreEqual(MyDouble, 2.5);
            Assert.AreEqual(MyDecimal, 3.5M);
            Assert.AreEqual(MySbyte, (sbyte)1);
            Assert.AreEqual(MyByte, (byte)2);
            Assert.AreEqual(MyBool, true);
            Assert.AreEqual(MyDateTime, TypeP6.d);
            Assert.AreEqual(MyDateTimeOffset, TypeP6.dt);
            Assert.AreEqual(MyUri, TypeP6.uri);
            Assert.AreEqual(MyTimeSpan, TypeP6.ts);
            Assert.AreEqual(MyGuid, TypeP6.g);
            Assert.AreEqual(MyChar, 'c');
            Assert.AreEqual(MyEnum, MyEnum.First);
            Assert.AreEqual(MyObj, "Obj");
            Assert.AreEqual(MyItem.MyProperty, "A");
            Assert.AreEqual(MyList.Count, 2);
            Assert.AreEqual(MyArray.Length, 2);
            Assert.IsNotNull(MyCollection);

            Assert.AreEqual(3, TypeP6.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP6.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP6.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP6.States[2]);
        }

        [TestMethod]
        public void Find_Attributes_On_Type_And_Method()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateClassProxy<TypeP7>();

            Assert.AreEqual(0, TypeP7.States.Count);

            proxy.Method();

            Assert.AreEqual(5, TypeP7.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP7.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeP7.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP7.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeP7.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP7.States[4]);
        }

        [TestMethod]
        public void Method_Results()
        {
            var proxyGenerator = new ProxyGenerator();

            var proxy = proxyGenerator.CreateClassProxy<TypeP4>(new IntForP4());

            Assert.AreEqual(0, TypeP4.States.Count);

            Assert.AreEqual(proxy.MyShort(), (short)-1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyInt(), (int)-10);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyLong(), (long)-100);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyUShort(), (ushort)1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyUInt(), (uint)10);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyULong(), (ulong)100);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyString(), "A");
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyFloat(), 1.5F);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDouble(), 2.5);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDecimal(), 3.5M);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MySbyte(), (sbyte)1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyByte(), (byte)2);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyBool(), true);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDateTime(), TypeP4.d);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDateTimeOffset(), TypeP4.dt);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyUri(), TypeP4.uri);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyTimeSpan(), TypeP4.ts);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyGuid(), TypeP4.g);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyChar(), 'c');
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyEnum(), MyEnum.First);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyObj(), "Obj");
            CheckAndClearTypeP4();

            Assert.IsNotNull(proxy.MyItem().MyProperty, "A");
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyList().Count, 2);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyArray().Length, 2);
            CheckAndClearTypeP4();

            Assert.IsNotNull(proxy.MyCollection());
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyT<MyItem>().GetType(), typeof(MyItem));
            CheckAndClearTypeP4();

            // nullables

            Assert.AreEqual(proxy.MyShortN(), (short)-1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyIntN(), (int)-10);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyLongN(), (long)-100);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyUShortN(), (ushort)1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyUIntN(), (uint)10);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyULongN(), (ulong)100);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyFloatN(), 1.5F);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDoubleN(), 2.5);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDecimalN(), 3.5M);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MySbyteN(), (sbyte)1);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyByteN(), (byte)2);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyBoolN(), true);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDateTimeN(), TypeP4.d);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyDateTimeOffsetN(), TypeP4.dt);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyTimeSpanN(), TypeP4.ts);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyGuidN(), TypeP4.g);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyCharN(), 'c');
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyEnumN(), MyEnum.First);
            CheckAndClearTypeP4();

            Assert.AreEqual(proxy.MyIntNull(), null);
            CheckAndClearTypeP4();
        }

        #endregion // Methods

        #region Properties


        [TestMethod]
        public void Call_Properties()
        {
            var proxyGenerator = new ProxyGenerator();

            var proxy = proxyGenerator.CreateClassProxy<TypeP3>(new IntForP3());

            var d = DateTime.Now;
            var dt = new DateTimeOffset(d);
            var uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
            var ts = TimeSpan.FromSeconds(100);
            var g = Guid.NewGuid();

            Assert.AreEqual(0, TypeP3.States.Count);

            proxy.MyShort = -1;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyInt = -10;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyLong = -100;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyUShort = 1;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyUInt = 10;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyULong = 100;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyString = "A";

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyFloat = 1.5F;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyDouble = 2.5;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyDecimal = 3.5M;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MySbyte = 1;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyByte = 2;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyBool = true;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyDateTime = d;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyDateTimeOffset = dt;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyUri = uri;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyTimeSpan = ts;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyGuid = g;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyChar = 'c';

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyEnum = MyEnum.First;

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyObj = "Obj";

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyItem = new MyItem { MyProperty = "A" };

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyList = new List<string> { "A", "B" };

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyArray = new string[] { "A", "B" };

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();

            proxy.MyCollection = new MyCollection();

            Assert.AreEqual(2, TypeP3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP3.States[1]);
            TypeP3.States.Clear();
        }


        #endregion // Properties

        #region Events

        [TestMethod]
        public void Event_Are_Called()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeP8();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeP8>(target, new IntForP8());
            bool isCalled = false;

            Assert.AreEqual(0, TypeP8.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(2, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[1]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeP8.States.Clear();
            isCalled = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(2, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[1]);
            Assert.AreEqual(false, isCalled);

            TypeP8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 += ev;

            Assert.AreEqual(3, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeP8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[2]);
            Assert.AreEqual(false, isCalled);

            target.RaiseMyEvent2();

            Assert.AreEqual(true, isCalled);

            TypeP8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 -= ev;

            Assert.AreEqual(3, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeP8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[2]);
            Assert.AreEqual(false, isCalled);

            TypeP8.States.Clear();
            isCalled = false;

            string p = null;
            PropertyChangedEventHandler ev2 = (s, e) =>
            {
                isCalled = true;
                p = e.PropertyName;
            };

            proxy.PropertyChanged += ev2;

            Assert.AreEqual(2, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[1]);
            Assert.AreEqual(false, isCalled);

            target.RaisePropertyChanged("MyProperty");

            Assert.AreEqual(true, isCalled);
            Assert.AreEqual("MyProperty", p);

            TypeP8.States.Clear();
            isCalled = false;

            proxy.PropertyChanged -= ev2;

            Assert.AreEqual(2, TypeP8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP8.States[1]);
            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Event_Are_Called_With_Attributes_With_Target()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeP9();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeP9>(target);
            bool isCalled = false;

            Assert.AreEqual(0, TypeP9.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeP9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP9.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeP9.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeP9.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeP9.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP9.States[4]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(true, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeP9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, TypeP9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP9.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeP9.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP9.States[2]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(true, target.IsCalledRemove);

            TypeP9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            target.RaiseMyEvent();

            Assert.AreEqual(0, TypeP9.States.Count);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);
        }

        [TestMethod]
        public void Event_Are_Called_With_Attributes_Without_Target()
        {
            var proxyGenerator = new ProxyGenerator();

            var proxy = proxyGenerator.CreateClassProxy<TypeP9>();
            bool isCalled = false;

            Assert.AreEqual(0, TypeP9.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeP9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP9.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeP9.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeP9.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeP9.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP9.States[4]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(true, proxy.IsCalledAdd);
            Assert.AreEqual(false, proxy.IsCalledRemove);

            proxy.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeP9.States.Clear();
            isCalled = false;
            proxy.IsCalledAdd = false;
            proxy.IsCalledRemove = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, TypeP9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP9.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeP9.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP9.States[2]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, proxy.IsCalledAdd);
            Assert.AreEqual(true, proxy.IsCalledRemove);

            TypeP9.States.Clear();
            isCalled = false;
            proxy.IsCalledAdd = false;
            proxy.IsCalledRemove = false;

            proxy.RaiseMyEvent();

            Assert.AreEqual(0, TypeP9.States.Count);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, proxy.IsCalledAdd);
            Assert.AreEqual(false, proxy.IsCalledRemove);
        }

        private static void CheckAndClearTypeP4()
        {
            Assert.AreEqual(3, TypeP4.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP4.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP4.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP4.States[2]);
            TypeP4.States.Clear();
        }

        #endregion // Events

        [TestMethod]
        public void FindPrivateMembers()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeP11>();

            Assert.AreEqual(0, TypeP11.States.Count);

            proxy.RaiseProtected();

            Assert.AreEqual(3, TypeP11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP11.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeP11.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP11.States[2]);

            TypeP11.States.Clear();

            proxy.RaiseProtectedInternal();

            Assert.AreEqual(3, TypeP11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeP11.States[0]);
            Assert.AreEqual(StateTypes.Class_Method_2, TypeP11.States[1]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeP11.States[2]);

            // properties

            TypeP11.States.Clear();

            proxy.RaisePropSet();

            Assert.AreEqual(3, TypeP11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor4_IsCalledBefore, TypeP11.States[0]);
            Assert.AreEqual(StateTypes.Property_Set, TypeP11.States[1]);
            Assert.AreEqual(StateTypes.Interceptor4_IsCalledAfter, TypeP11.States[2]);

            TypeP11.States.Clear();

            proxy.RaisePropGet();

            Assert.AreEqual(3, TypeP11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor3_IsCalledBefore, TypeP11.States[0]);
            Assert.AreEqual(StateTypes.Property_Get, TypeP11.States[1]);
            Assert.AreEqual(StateTypes.Interceptor3_IsCalledAfter, TypeP11.States[2]);

            TypeP11.States.Clear();

            proxy.RaiseEvent();

            Assert.AreEqual(2, TypeP11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP11.States[1]);

        }

        //[TestMethod]
        //public void Throws_With_No_Empty_Types_Base_Ctor()
        //{
        //    var generator = new ProxyGenerator();
        //    Assert.ThrowsException<NotSupportedException>(() => generator.CreateClassProxy<TypeP12>());
        //    Assert.ThrowsException<NotSupportedException>(() => generator.CreateClassProxy<TypeP12_B>());
        //    Assert.ThrowsException<ArgumentException>(() => generator.CreateClassProxy<TypeP12_C>());
        //}

        //[TestMethod]
        //public void Find_Protected_Empty_Base_Ctor()
        //{
        //    var generator = new ProxyGenerator();

        //    Assert.AreEqual(0, TypeP13.States.Count);

        //    var proxy = generator.CreateClassProxy<TypeP13>(new IntForP13());

        //    Assert.AreEqual(1, TypeP13.States.Count);
        //    Assert.AreEqual(StateTypes.Ctor_IsCalled, TypeP13.States[0]);

        //    TypeP13.States.Clear();

        //    proxy.Method();

        //    Assert.AreEqual(3, TypeP13.States.Count);
        //    Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP13.States[0]);
        //    Assert.AreEqual(StateTypes.Class_Method, TypeP13.States[1]);
        //    Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP13.States[2]);
        //}

        [TestMethod]
        public void Fail_With_Dependencies()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeP14>();

            Assert.ThrowsException<ArgumentException>(() => proxy.Method());
        }

        [TestMethod]
        public void Resolve_Interceptor_With_Dependencies()
        {
            var generator = new ProxyGenerator();
            IUnityContainer container = new UnityContainer();
            ObjectFactory.SetDefaultFactory(type => container.Resolve(type));

            container.RegisterType(typeof(IService1), typeof(Service1));
            container.RegisterType(typeof(IService2), typeof(Service2));

            var proxy = generator.CreateClassProxy<TypeP14>();

            proxy.Method();

            Assert.IsTrue(TypeP14.IsCalled);
            Assert.AreEqual(true, InterceptorDynWithInjections.IsCalledBefore);
            Assert.AreEqual(true, InterceptorDynWithInjections.IsCalledAfter);
            Assert.IsNotNull(InterceptorDynWithInjections.Service1);
            Assert.IsNotNull(InterceptorDynWithInjections.Service2);
        }

        [TestMethod]
        public void Filter_By_Attribute_On_Type()
        {
            var generator = new ProxyGenerator();
           
            var proxy = generator.CreateClassProxy<TypeP15>();

            Assert.AreEqual(0, TypeP15.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(2, TypeP15.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP15.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP15.States[1]);

            TypeP15.States.Clear();

            var p = proxy.MyProperty;

            Assert.AreEqual(0, TypeP15.States.Count);

            proxy.Method();

            Assert.AreEqual(2, TypeP15.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP15.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP15.States[1]);

            TypeP15.States.Clear();

            EventHandler ev = null;
            ev = (s, e) => { };
            proxy.MyEvent += ev;

            Assert.AreEqual(2, TypeP15.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP15.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP15.States[1]);

            TypeP15.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(0, TypeP15.States.Count);
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
    public class MultiTargetAttribute : InterceptorAttributeBase,
        IPropertySetInterceptorProvider,
        IMethodInterceptorProvider,
        IAddEventInterceptorProvider
    {
        public MultiTargetAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }


    [MultiTarget(typeof(IntForP15))]
    public class TypeP15
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual string MyProperty { get; set; }

        public virtual void Method() { }

        public virtual event EventHandler MyEvent;
    }

    public class IntForP15 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP15.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP15.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public interface IService1 { }
    public class Service1 : IService1 { }

    public interface IService2 { }
    public class Service2 : IService2 { }

    public class TypeP14
    {
        public static bool IsCalled { get; set; }

        [MethodInterceptor(typeof(InterceptorDynWithInjections))]
        public virtual void Method()
        {
            IsCalled = true;
        }
    }

    public class InterceptorDynWithInjections : IInterceptor
    {
        public InterceptorDynWithInjections(IService1 service1, IService2 service2)
        {
            Service1 = service1;
            Service2 = service2;
        }

        public static IService1 Service1 { get; set; }
        public static IService2 Service2 { get; set; }

        public static bool IsCalledBefore { get; set; }
        public static bool IsCalledAfter { get; set; }

        public void Intercept(IInvocation invocation)
        {
            IsCalledBefore = true;

            invocation.Proceed();

            IsCalledAfter = true;
        }
    }

    public class TypeP12_B
    {
        private TypeP12_B()
        {

        }
    }

    public class TypeP12_C
    {
        internal TypeP12_C()
        {

        }
    }

    public class TypeP12
    {
        public TypeP12(IMyService myService, List<string> list)
        {
            MyService = myService;
            List = list;
        }

        public static IMyService MyService { get; set; }
        public static List<string> List { get; set; }

        public virtual void Method()
        {

        }
    }

    public class TypeP13
    {
        public static List<StateTypes> States = new List<StateTypes>();

        protected TypeP13()
        {
            States.Add(StateTypes.Ctor_IsCalled);
        }

        private TypeP13(IMyService myService, List<string> list)
        {
            MyService = myService;
            List = list;
        }

        public static IMyService MyService { get; set; }
        public static List<string> List { get; set; }

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }

    public class IntForP13 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP13.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP13.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class TypeP1
    {
        public virtual void MethodA()
        {
            States.Add(StateTypes.Class_Method);
        }

        public static List<StateTypes> States = new List<StateTypes>();

        public static short MyShort { get; set; }
        public static int MyInt { get; set; }
        public static long MyLong { get; set; }
        public static ushort MyUShort { get; set; }
        public static uint MyUInt { get; set; }
        public static ulong MyULong { get; set; }
        public static string MyString { get; set; }
        public static float MyFloat { get; set; }
        public static double MyDouble { get; set; }
        public static decimal MyDecimal { get; set; }
        public static sbyte MySbyte { get; set; }
        public static byte MyByte { get; set; }
        public static bool MyBool { get; set; }
        public static DateTime MyDateTime { get; set; }
        public static DateTimeOffset MyDateTimeOffset { get; set; }
        public static Uri MyUri { get; set; }
        public static TimeSpan MyTimeSpan { get; set; }
        public static Guid MyGuid { get; set; }
        public static char MyChar { get; set; }
        public static MyEnum MyEnum { get; set; }
        public static object MyObj { get; set; }
        public static MyItem MyItem { get; set; }
        public static List<string> MyList { get; set; }
        public static string[] MyArray { get; set; }
        public static MyCollection MyCollection { get; set; }

        public virtual void Parameters(short MyShort
          , int MyInt
          , long MyLong
          , ushort MyUShort
          , uint MyUInt
          , ulong MyULong
          , string MyString
          , float MyFloat
          , double MyDouble
          , decimal MyDecimal
          , sbyte MySbyte
          , byte MyByte
          , bool MyBool
          , DateTime MyDateTime
          , DateTimeOffset MyDateTimeOffset
          , Uri MyUri
          , TimeSpan MyTimeSpan
          , Guid MyGuid
          , char MyChar
          , MyEnum MyEnum
          , object MyObj
          , MyItem MyItem
          , List<string> MyList
          , string[] MyArray
          , MyCollection MyCollection)
        {
            States.Add(StateTypes.Class_Method);
            TypeP1.MyShort = MyShort;
            TypeP1.MyInt = MyInt;
            TypeP1.MyLong = MyLong;
            TypeP1.MyUShort = MyUShort;
            TypeP1.MyUInt = MyUInt;
            TypeP1.MyULong = MyULong;
            TypeP1.MyString = MyString;
            TypeP1.MyFloat = MyFloat;
            TypeP1.MyDouble = MyDouble;
            TypeP1.MyDecimal = MyDecimal;
            TypeP1.MySbyte = MySbyte;
            TypeP1.MyByte = MyByte;
            TypeP1.MyBool = MyBool;
            TypeP1.MyDateTime = MyDateTime;
            TypeP1.MyDateTimeOffset = MyDateTimeOffset;
            TypeP1.MyUri = MyUri;
            TypeP1.MyTimeSpan = MyTimeSpan;
            TypeP1.MyGuid = MyGuid;
            TypeP1.MyChar = MyChar;
            TypeP1.MyEnum = MyEnum;
            TypeP1.MyObj = MyObj;
            TypeP1.MyItem = MyItem;
            TypeP1.MyList = MyList;
            TypeP1.MyArray = MyArray;
            TypeP1.MyCollection = MyCollection;
        }
    }

    public class TypeP2
    {
        public virtual int MyProperty { get; set; }

        public virtual void MethodA()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public class MyCollection : Collection<MyItem>
    {

    }

    public class TypeP3
    {
        //       case string v:
        //       case int v:
        //       case short v:
        //       case long v:
        //       case ushort v:
        //       case uint v:
        //       case ulong v:
        //       case float v:
        //       case double v:
        //       case decimal v:
        //       case sbyte v:
        //       case byte v:
        //       case bool v:
        //       case DateTime v:
        //       case DateTimeOffset v:
        //       case Uri v:
        //       case TimeSpan v:
        //       case Guid v:
        //       case char v:
        //       case byte[] v:
        //       case Enum v:

        public static List<StateTypes> States = new List<StateTypes>();

        public virtual short MyShort { get; set; }
        public virtual int MyInt { get; set; }
        public virtual long MyLong { get; set; }
        public virtual ushort MyUShort { get; set; }
        public virtual uint MyUInt { get; set; }
        public virtual ulong MyULong { get; set; }
        public virtual string MyString { get; set; }
        public virtual float MyFloat { get; set; }
        public virtual double MyDouble { get; set; }
        public virtual decimal MyDecimal { get; set; }
        public virtual sbyte MySbyte { get; set; }
        public virtual byte MyByte { get; set; }
        public virtual bool MyBool { get; set; }
        public virtual DateTime MyDateTime { get; set; }
        public virtual DateTimeOffset MyDateTimeOffset { get; set; }
        public virtual Uri MyUri { get; set; }
        public virtual TimeSpan MyTimeSpan { get; set; }
        public virtual Guid MyGuid { get; set; }
        public virtual char MyChar { get; set; }
        public virtual MyEnum MyEnum { get; set; }
        public virtual object MyObj { get; set; }
        public virtual MyItem MyItem { get; set; }
        public virtual List<string> MyList { get; set; }
        public virtual string[] MyArray { get; set; }
        public virtual MyCollection MyCollection { get; set; }
    }

    public class TypeP4
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public static DateTime d = DateTime.Now;
        public static DateTimeOffset dt = new DateTimeOffset(d);
        public static Uri uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
        public static TimeSpan ts = TimeSpan.FromSeconds(100);
        public static Guid g = Guid.NewGuid();

        public virtual short MyShort() { States.Add(StateTypes.Class_Method); return -1; }
        public virtual int MyInt() { States.Add(StateTypes.Class_Method); return -10; }
        public virtual long MyLong() { States.Add(StateTypes.Class_Method); return -100; }
        public virtual ushort MyUShort() { States.Add(StateTypes.Class_Method); return 1; }
        public virtual uint MyUInt() { States.Add(StateTypes.Class_Method); return 10; }
        public virtual ulong MyULong() { States.Add(StateTypes.Class_Method); return 100; }
        public virtual string MyString() { States.Add(StateTypes.Class_Method); return "A"; }
        public virtual float MyFloat() { States.Add(StateTypes.Class_Method); return 1.5F; }
        public virtual double MyDouble() { States.Add(StateTypes.Class_Method); return 2.5; }
        public virtual decimal MyDecimal() { States.Add(StateTypes.Class_Method); return 3.5M; }
        public virtual sbyte MySbyte() { States.Add(StateTypes.Class_Method); return 1; }
        public virtual byte MyByte() { States.Add(StateTypes.Class_Method); return 2; }
        public virtual bool MyBool() { States.Add(StateTypes.Class_Method); return true; }
        public virtual DateTime MyDateTime() { States.Add(StateTypes.Class_Method); return d; }
        public virtual DateTimeOffset MyDateTimeOffset() { States.Add(StateTypes.Class_Method); return dt; }
        public virtual Uri MyUri() { States.Add(StateTypes.Class_Method); return uri; }
        public virtual TimeSpan MyTimeSpan() { States.Add(StateTypes.Class_Method); return ts; }
        public virtual Guid MyGuid() { States.Add(StateTypes.Class_Method); return g; }
        public virtual char MyChar() { States.Add(StateTypes.Class_Method); return 'c'; }
        public virtual MyEnum MyEnum() { States.Add(StateTypes.Class_Method); return Tests.MyEnum.First; }
        public virtual object MyObj() { States.Add(StateTypes.Class_Method); return "Obj"; }
        public virtual MyItem MyItem() { States.Add(StateTypes.Class_Method); return new Tests.MyItem { MyProperty = "A" }; }
        public virtual List<string> MyList() { States.Add(StateTypes.Class_Method); return new List<string> { "A", "B" }; }
        public virtual string[] MyArray() { States.Add(StateTypes.Class_Method); return new string[] { "A", "B" }; }
        public virtual MyCollection MyCollection() { States.Add(StateTypes.Class_Method); return new MyCollection(); }

        public virtual T MyT<T>() where T : new() { States.Add(StateTypes.Class_Method); return new T(); }

        public virtual short? MyShortN() { States.Add(StateTypes.Class_Method); return -1; }
        public virtual int? MyIntN() { States.Add(StateTypes.Class_Method); return -10; }
        public virtual long? MyLongN() { States.Add(StateTypes.Class_Method); return -100; }
        public virtual ushort? MyUShortN() { States.Add(StateTypes.Class_Method); return 1; }
        public virtual uint? MyUIntN() { States.Add(StateTypes.Class_Method); return 10; }
        public virtual ulong? MyULongN() { States.Add(StateTypes.Class_Method); return 100; }
        public virtual float? MyFloatN() { States.Add(StateTypes.Class_Method); return 1.5F; }
        public virtual double? MyDoubleN() { States.Add(StateTypes.Class_Method); return 2.5; }
        public virtual decimal? MyDecimalN() { States.Add(StateTypes.Class_Method); return 3.5M; }
        public virtual sbyte? MySbyteN() { States.Add(StateTypes.Class_Method); return 1; }
        public virtual byte? MyByteN() { States.Add(StateTypes.Class_Method); return 2; }
        public virtual bool? MyBoolN() { States.Add(StateTypes.Class_Method); return true; }
        public virtual DateTime? MyDateTimeN() { States.Add(StateTypes.Class_Method); return d; }
        public virtual DateTimeOffset? MyDateTimeOffsetN() { States.Add(StateTypes.Class_Method); return dt; }
        public virtual TimeSpan? MyTimeSpanN() { States.Add(StateTypes.Class_Method); return ts; }
        public virtual Guid? MyGuidN() { States.Add(StateTypes.Class_Method); return g; }
        public virtual char? MyCharN() { States.Add(StateTypes.Class_Method); return 'c'; }
        public virtual MyEnum? MyEnumN() { States.Add(StateTypes.Class_Method); return Tests.MyEnum.First; }

        public virtual int? MyIntNull() { States.Add(StateTypes.Class_Method); return null; }

    }

    public class TypeP5
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public static DateTime d = DateTime.Now;
        public static DateTimeOffset dt = new DateTimeOffset(d);
        public static Uri uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
        public static TimeSpan ts = TimeSpan.FromSeconds(100);
        public static Guid g = Guid.NewGuid();

        public static short MyShort { get; set; }
        public static int MyInt { get; set; }
        public static long MyLong { get; set; }
        public static ushort MyUShort { get; set; }
        public static uint MyUInt { get; set; }
        public static ulong MyULong { get; set; }
        public static string MyString { get; set; }
        public static float MyFloat { get; set; }
        public static double MyDouble { get; set; }
        public static decimal MyDecimal { get; set; }
        public static sbyte MySbyte { get; set; }
        public static byte MyByte { get; set; }
        public static bool MyBool { get; set; }
        public static DateTime MyDateTime { get; set; }
        public static DateTimeOffset MyDateTimeOffset { get; set; }
        public static Uri MyUri { get; set; }
        public static TimeSpan MyTimeSpan { get; set; }
        public static Guid MyGuid { get; set; }
        public static char MyChar { get; set; }
        public static MyEnum MyEnum { get; set; }
        public static object MyObj { get; set; }
        public static MyItem MyItem { get; set; }
        public static List<string> MyList { get; set; }
        public static string[] MyArray { get; set; }
        public static MyCollection MyCollection { get; set; }

        public virtual void Parameters(out short MyShort
          , out int MyInt
          , out long MyLong
          , out ushort MyUShort
          , out uint MyUInt
          , out ulong MyULong
          , out string MyString
          , out float MyFloat
          , out double MyDouble
          , out decimal MyDecimal
          , out sbyte MySbyte
          , out byte MyByte
          , out bool MyBool
          , out DateTime MyDateTime
          , out DateTimeOffset MyDateTimeOffset
          , out Uri MyUri
          , out TimeSpan MyTimeSpan
          , out Guid MyGuid
          , out char MyChar
          , out MyEnum MyEnum
          , out object MyObj
          , out MyItem MyItem
          , out List<string> MyList
          , out string[] MyArray
          , out MyCollection MyCollection)
        {
            States.Add(StateTypes.Class_Method);


            MyShort = -1;
            MyInt = -10;
            MyLong = -100;
            MyUShort = 1;
            MyUInt = 10;
            MyULong = 100;
            MyString = "A";
            MyFloat = 1.5F;
            MyDouble = 2.5;
            MyDecimal = 3.5M;
            MySbyte = 1;
            MyByte = 2;
            MyBool = true;
            MyDateTime = d;
            MyDateTimeOffset = dt;
            MyUri = uri;
            MyTimeSpan = ts;
            MyGuid = g;
            MyChar = 'c';
            MyEnum = MyEnum.First;
            MyObj = "Obj";
            MyItem = new MyItem { MyProperty = "A" };
            MyList = new List<string> { "A", "B" };
            MyArray = new string[] { "A", "B" };
            MyCollection = new MyCollection();


            //TypeP5.MyShort = MyShort;
            //TypeP5.MyInt = MyInt;
            //TypeP5.MyLong = MyLong;
            //TypeP5.MyUShort = MyUShort;
            //TypeP5.MyUInt = MyUInt;
            //TypeP5.MyULong = MyULong;
            //TypeP5.MyString = MyString;
            //TypeP5.MyFloat = MyFloat;
            //TypeP5.MyDouble = MyDouble;
            //TypeP5.MyDecimal = MyDecimal;
            //TypeP5.MySbyte = MySbyte;
            //TypeP5.MyByte = MyByte;
            //TypeP5.MyBool = MyBool;
            //TypeP5.MyDateTime = MyDateTime;
            //TypeP5.MyDateTimeOffset = MyDateTimeOffset;
            //TypeP5.MyUri = MyUri;
            //TypeP5.MyTimeSpan = MyTimeSpan;
            //TypeP5.MyGuid = MyGuid;
            //TypeP5.MyChar = MyChar;
            //TypeP5.MyEnum = MyEnum;
            //TypeP5.MyObj = MyObj;
            //TypeP5.MyItem = MyItem;
            //TypeP5.MyList = MyList;
            //TypeP5.MyArray = MyArray;
            //TypeP5.MyCollection = MyCollection;
        }
    }

    public class TypeP6
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public static DateTime d = DateTime.Now;
        public static DateTimeOffset dt = new DateTimeOffset(d);
        public static Uri uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
        public static TimeSpan ts = TimeSpan.FromSeconds(100);
        public static Guid g = Guid.NewGuid();

        public static short MyShort { get; set; }
        public static int MyInt { get; set; }
        public static long MyLong { get; set; }
        public static ushort MyUShort { get; set; }
        public static uint MyUInt { get; set; }
        public static ulong MyULong { get; set; }
        public static string MyString { get; set; }
        public static float MyFloat { get; set; }
        public static double MyDouble { get; set; }
        public static decimal MyDecimal { get; set; }
        public static sbyte MySbyte { get; set; }
        public static byte MyByte { get; set; }
        public static bool MyBool { get; set; }
        public static DateTime MyDateTime { get; set; }
        public static DateTimeOffset MyDateTimeOffset { get; set; }
        public static Uri MyUri { get; set; }
        public static TimeSpan MyTimeSpan { get; set; }
        public static Guid MyGuid { get; set; }
        public static char MyChar { get; set; }
        public static MyEnum MyEnum { get; set; }
        public static object MyObj { get; set; }
        public static MyItem MyItem { get; set; }
        public static List<string> MyList { get; set; }
        public static string[] MyArray { get; set; }
        public static MyCollection MyCollection { get; set; }

        public virtual void Parameters(ref short MyShort
          , ref int MyInt
          , ref long MyLong
          , ref ushort MyUShort
          , ref uint MyUInt
          , ref ulong MyULong
          , ref string MyString
          , ref float MyFloat
          , ref double MyDouble
          , ref decimal MyDecimal
          , ref sbyte MySbyte
          , ref byte MyByte
          , ref bool MyBool
          , ref DateTime MyDateTime
          , ref DateTimeOffset MyDateTimeOffset
          , ref Uri MyUri
          , ref TimeSpan MyTimeSpan
          , ref Guid MyGuid
          , ref char MyChar
          , ref MyEnum MyEnum
          , ref object MyObj
          , ref MyItem MyItem
          , ref List<string> MyList
          , ref string[] MyArray
          , ref MyCollection MyCollection)
        {
            States.Add(StateTypes.Class_Method);

            MyShort = -1;
            MyInt = -10;
            MyLong = -100;
            MyUShort = 1;
            MyUInt = 10;
            MyULong = 100;
            MyString = "A";
            MyFloat = 1.5F;
            MyDouble = 2.5;
            MyDecimal = 3.5M;
            MySbyte = 1;
            MyByte = 2;
            MyBool = true;
            MyDateTime = d;
            MyDateTimeOffset = dt;
            MyUri = uri;
            MyTimeSpan = ts;
            MyGuid = g;
            MyChar = 'c';
            MyEnum = MyEnum.First;
            MyObj = "Obj";
            MyItem = new MyItem { MyProperty = "A" };
            MyList = new List<string> { "A", "B" };
            MyArray = new string[] { "A", "B" };
            MyCollection = new MyCollection();
        }
    }

    [AllInterceptor(typeof(IntForP7))]
    public class TypeP7
    {
        public static List<StateTypes> States = new List<StateTypes>();


        [MethodInterceptor(typeof(IntForP7_b))]
        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }


    public class TypeP8 : INotifyPropertyChanged
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual event EventHandler MyEvent;

        public virtual event PropertyChangedEventHandler PropertyChanged;

        private event EventHandler myEvent2;

        public virtual event EventHandler MyEvent2
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent2 += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent2 -= value;
            }
        }

        public void RaiseMyEvent()
        {
            MyEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseMyEvent2()
        {
            myEvent2?.Invoke(this, EventArgs.Empty);
        }

        public void RaisePropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }


    [AllInterceptor(typeof(IntForP9))]
    public class TypeP9
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;

        public bool IsCalledAdd = false;
        public bool IsCalledRemove = false;

        [AddEventInterceptor(typeof(IntForP9_b))]
        public virtual event EventHandler MyEvent
        {
            add
            {
                IsCalledAdd = true;
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                IsCalledRemove = true;
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }

        public void RaiseMyEvent()
        {
            myEvent?.Invoke(this, EventArgs.Empty);
        }
    }


    [AllInterceptor(typeof(IntForP10))]
    public interface ITypeP10
    {
        [AddEventInterceptor(typeof(IntPFor10_b))]
        event EventHandler MyEvent;

        void RaiseMyEvent();
    }

    public class TypeP10 : ITypeP10
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;

        public bool IsCalledAdd = false;
        public bool IsCalledRemove = false;

        public virtual event EventHandler MyEvent
        {
            add
            {
                IsCalledAdd = true;
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                IsCalledRemove = true;
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }

        public void RaiseMyEvent()
        {
            myEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public class TypeP11
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;

        [PropertyGetInterceptor(typeof(IntForP11_c))]
        [PropertySetInterceptor(typeof(IntForP11_d))]
        protected virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Property_Get);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Property_Set);
                myVar = value;
            }
        }


        [MethodInterceptor(typeof(IntForP11))]
        protected virtual void ProtectedMethod()
        {
            States.Add(StateTypes.Class_Method);
        }

        [MethodInterceptor(typeof(IntForP11_b))]
        protected internal virtual void ProtectedInternalMethod()
        {
            States.Add(StateTypes.Class_Method_2);
        }

        public virtual void RaiseProtected()
        {
            ProtectedMethod();
        }

        public virtual void RaiseProtectedInternal()
        {
            ProtectedInternalMethod();
        }

        public virtual void RaisePropSet()
        {
            MyProperty = "New value";
        }

        public virtual string RaisePropGet()
        {
            return MyProperty;
        }

        public void RaiseEvent()
        {
            MyEvent += TypeP11_MyEvent;
            MyEvent?.Invoke(this, EventArgs.Empty);
        }

        private void TypeP11_MyEvent(object sender, EventArgs e)
        {

        }

        [AddEventInterceptor(typeof(IntForP11))]
        protected virtual event EventHandler MyEvent;
    }

    public class Sub : TypeP11
    {
        public Sub()
        {
            base.MyEvent += Sub_MyEvent;
        }

        private void Sub_MyEvent(object sender, EventArgs e)
        {

        }
    }

    public class IntForP11 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP11.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP11.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP11_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP11.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeP11.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForP11_c : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP11.States.Add(StateTypes.Interceptor3_IsCalledBefore);

            invocation.Proceed();

            TypeP11.States.Add(StateTypes.Interceptor3_IsCalledAfter);
        }
    }

    public class IntForP11_d : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP11.States.Add(StateTypes.Interceptor4_IsCalledBefore);

            invocation.Proceed();

            TypeP11.States.Add(StateTypes.Interceptor4_IsCalledAfter);
        }
    }

    public class IntForP11_e : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP11.States.Add(StateTypes.Interceptor5_IsCalledBefore);

            invocation.Proceed();

            TypeP11.States.Add(StateTypes.Interceptor5_IsCalledAfter);
        }
    }
    public enum MyEnum
    {
        Default,
        First
    }

    [Serializable]
    public class MyItem
    {
        public string MyProperty { get; set; }
    }

    public enum StateTypes
    {
        Interceptor1_IsCalledBefore,
        Interceptor1_IsCalledAfter,
        Interceptor2_IsCalledBefore,
        Interceptor2_IsCalledAfter,
        Class_Method,
        Class_Method_2,
        Class_Method_3,
        Class_Method_4,
        Interceptor3_IsCalledBefore,
        Interceptor3_IsCalledAfter,
        Interceptor4_IsCalledBefore,
        Interceptor4_IsCalledAfter,
        AddEvent_IsCalled,
        RemoveEvent_IsCalled,
        Property_Set,
        Property_Get,
        Interceptor5_IsCalledBefore,
        Interceptor5_IsCalledAfter,
        Ctor_IsCalled,
        Get_Called,
        Set_Called,
        Get2_Called,
        Set2_Called,
        Enter_Interceptor,
        Exit_Interceptor,
        Exception_Interceptor,
        Notify
    }

    public class IntForP1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP3 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP3.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP3.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP4 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP4.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP4.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP5 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP5.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP5.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP6 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP6.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP6.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP7 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP7.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP7.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP7_b : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeP7.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeP7.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForP9 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP9.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP9.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForP9_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP9.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeP9.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForP10 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP10.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP10.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntPFor10_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP10.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeP10.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForP8 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeP8.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeP8.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

}
