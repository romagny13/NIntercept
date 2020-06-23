using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NIntercept.Tests
{

    [TestClass]
    public class ProxyGeneratorWithTargetTests
    {
        [TestInitialize()]
        public void Startup()
        {
            TypeT1.States.Clear();
            TypeT3.States.Clear();
            TypeT4.States.Clear();
            TypeT5.States.Clear();
            TypeT6.States.Clear();
            TypeT7.States.Clear();
            TypeT8.States.Clear();
            TypeT9.States.Clear();
            TypeT10.States.Clear();
            EventGenericClass.States.Clear();
        }

        #region Methods

        [TestMethod]
        public void Call_Empty_Method()
        {
            var generator = new ProxyGenerator();

            var target = new TypeT1();
            var proxy = generator.CreateClassProxyWithTarget<TypeT1>(target, new IntForT1());

            Assert.AreEqual(0, TypeT1.States.Count);

            proxy.MethodA();

            Assert.AreEqual(3, TypeT1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT1.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Call_Method_With_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT1();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT1>(target, new IntForT1());

            Assert.AreEqual(0, TypeT1.States.Count);

            var d = DateTime.Now;
            var dt = new DateTimeOffset(d);
            var uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
            var ts = TimeSpan.FromSeconds(100);
            var g = Guid.NewGuid();

            proxy.Parameters(-1, -10, -100, 1, 10, 100, "A", 1.5F, 2.5, 3.5M, 1, 2, true, d, dt, uri, ts, g, 'c', MyEnum.First, "Obj", new MyItem { MyProperty = "A" }, new List<string> { "A", "B" }, new string[] { "A", "B" }, new MyCollection());

            Assert.AreEqual(target.MyShort, (short)-1);
            Assert.AreEqual(target.MyInt, (int)-10);
            Assert.AreEqual(target.MyLong, (long)-100);
            Assert.AreEqual(target.MyUShort, (ushort)1);
            Assert.AreEqual(target.MyUInt, (uint)10);
            Assert.AreEqual(target.MyULong, (ulong)100);
            Assert.AreEqual(target.MyString, "A");
            Assert.AreEqual(target.MyFloat, 1.5F);
            Assert.AreEqual(target.MyDouble, 2.5);
            Assert.AreEqual(target.MyDecimal, 3.5M);
            Assert.AreEqual(target.MySbyte, (sbyte)1);
            Assert.AreEqual(target.MyByte, (byte)2);
            Assert.AreEqual(target.MyBool, true);
            Assert.AreEqual(target.MyDateTime, d);
            Assert.AreEqual(target.MyDateTimeOffset, dt);
            Assert.AreEqual(target.MyUri, uri);
            Assert.AreEqual(target.MyTimeSpan, ts);
            Assert.AreEqual(target.MyGuid, g);
            Assert.AreEqual(target.MyChar, 'c');
            Assert.AreEqual(target.MyEnum, MyEnum.First);
            Assert.AreEqual(target.MyObj, "Obj");
            Assert.AreEqual(target.MyItem.MyProperty, "A");
            Assert.AreEqual(target.MyList.Count, 2);
            Assert.AreEqual(target.MyArray.Length, 2);
            Assert.IsNotNull(target.MyCollection);

            Assert.AreEqual(3, TypeT1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT1.States[2]);
        }


        [TestMethod]
        public void Out_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT5();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT5>(target, new IntForT5());

            Assert.AreEqual(0, TypeT5.States.Count);

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
            Assert.AreEqual(MyDateTime, TypeT5.d);
            Assert.AreEqual(MyDateTimeOffset, TypeT5.dt);
            Assert.AreEqual(MyUri, TypeT5.uri);
            Assert.AreEqual(MyTimeSpan, TypeT5.ts);
            Assert.AreEqual(MyGuid, TypeT5.g);
            Assert.AreEqual(MyChar, 'c');
            Assert.AreEqual(MyEnum, MyEnum.First);
            Assert.AreEqual(MyObj, "Obj");
            Assert.AreEqual(MyItem.MyProperty, "A");
            Assert.AreEqual(MyList.Count, 2);
            Assert.AreEqual(MyArray.Length, 2);
            Assert.IsNotNull(MyCollection);

            Assert.AreEqual(3, TypeT5.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT5.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT5.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT5.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Ref_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT6();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT6>(target, new IntForT6());

            Assert.AreEqual(0, TypeT6.States.Count);

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
            Assert.AreEqual(MyDateTime, target.d);
            Assert.AreEqual(MyDateTimeOffset, target.dt);
            Assert.AreEqual(MyUri, target.uri);
            Assert.AreEqual(MyTimeSpan, target.ts);
            Assert.AreEqual(MyGuid, target.g);
            Assert.AreEqual(MyChar, 'c');
            Assert.AreEqual(MyEnum, MyEnum.First);
            Assert.AreEqual(MyObj, "Obj");
            Assert.AreEqual(MyItem.MyProperty, "A");
            Assert.AreEqual(MyList.Count, 2);
            Assert.AreEqual(MyArray.Length, 2);
            Assert.IsNotNull(MyCollection);

            Assert.AreEqual(3, TypeT6.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT6.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT6.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT6.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Find_Attributes_On_Type_And_Method()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT7();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT7>(target);

            Assert.AreEqual(0, TypeT7.States.Count);

            proxy.Method();

            Assert.AreEqual(5, TypeT7.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT7.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeT7.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT7.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeT7.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT7.States[4]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Method_Results()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT4();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT4>(target, new IntForT4());

            Assert.AreEqual(0, TypeT4.States.Count);

            Assert.AreEqual(proxy.MyShort(), (short)-1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyInt(), (int)-10);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyLong(), (long)-100);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyUShort(), (ushort)1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyUInt(), (uint)10);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyULong(), (ulong)100);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyString(), "A");
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyFloat(), 1.5F);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDouble(), 2.5);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDecimal(), 3.5M);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MySbyte(), (sbyte)1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyByte(), (byte)2);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyBool(), true);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDateTime(), target.d);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDateTimeOffset(), target.dt);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyUri(), target.uri);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyTimeSpan(), target.ts);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyGuid(), target.g);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyChar(), 'c');
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyEnum(), MyEnum.First);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyObj(), "Obj");
            CheckAndClearTypeT4();

            Assert.IsNotNull(proxy.MyItem().MyProperty, "A");
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyList().Count, 2);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyArray().Length, 2);
            CheckAndClearTypeT4();

            Assert.IsNotNull(proxy.MyCollection());
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyT<MyItem>().GetType(), typeof(MyItem));
            CheckAndClearTypeT4();

            // nullables

            Assert.AreEqual(proxy.MyShortN(), (short)-1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyIntN(), (int)-10);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyLongN(), (long)-100);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyUShortN(), (ushort)1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyUIntN(), (uint)10);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyULongN(), (ulong)100);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyFloatN(), 1.5F);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDoubleN(), 2.5);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDecimalN(), 3.5M);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MySbyteN(), (sbyte)1);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyByteN(), (byte)2);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyBoolN(), true);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDateTimeN(), target.d);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyDateTimeOffsetN(), target.dt);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyTimeSpanN(), target.ts);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyGuidN(), target.g);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyCharN(), 'c');
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyEnumN(), MyEnum.First);
            CheckAndClearTypeT4();

            Assert.AreEqual(proxy.MyIntNull(), null);
            CheckAndClearTypeT4();
        }

        #endregion // Methods

        #region Properties


        [TestMethod]
        public void Call_Properties()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeT3();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT3>(target, new IntForT3());

            var d = DateTime.Now;
            var dt = new DateTimeOffset(d);
            var uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
            var ts = TimeSpan.FromSeconds(100);
            var g = Guid.NewGuid();

            Assert.AreEqual(0, TypeT3.States.Count);

            proxy.MyShort = -1;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyInt = -10;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyLong = -100;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyUShort = 1;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyUInt = 10;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyULong = 100;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyString = "A";

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyFloat = 1.5F;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyDouble = 2.5;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyDecimal = 3.5M;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MySbyte = 1;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyByte = 2;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyBool = true;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyDateTime = d;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyDateTimeOffset = dt;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyUri = uri;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyTimeSpan = ts;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyGuid = g;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyChar = 'c';

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyEnum = MyEnum.First;

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyObj = "Obj";

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyItem = new MyItem { MyProperty = "A" };

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyList = new List<string> { "A", "B" };

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyArray = new string[] { "A", "B" };

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();

            proxy.MyCollection = new MyCollection();

            Assert.AreEqual(2, TypeT3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT3.States[1]);
            TypeT3.States.Clear();
        }


        #endregion // Properties

        #region Events

        [TestMethod]
        public void Event_Are_Called()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeT8();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT8>(target, new IntForT8());
            bool isCalled = false;

            Assert.AreEqual(0, TypeT8.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(2, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[1]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeT8.States.Clear();
            isCalled = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(2, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[1]);
            Assert.AreEqual(false, isCalled);

            TypeT8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 += ev;

            Assert.AreEqual(3, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeT8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[2]);
            Assert.AreEqual(false, isCalled);

            target.RaiseMyEvent2();

            Assert.AreEqual(true, isCalled);

            TypeT8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 -= ev;

            Assert.AreEqual(3, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeT8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[2]);
            Assert.AreEqual(false, isCalled);

            TypeT8.States.Clear();
            isCalled = false;

            string p = null;
            PropertyChangedEventHandler ev2 = (s, e) =>
            {
                isCalled = true;
                p = e.PropertyName;
            };

            proxy.PropertyChanged += ev2;

            Assert.AreEqual(2, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[1]);
            Assert.AreEqual(false, isCalled);

            target.RaisePropertyChanged("MyProperty");

            Assert.AreEqual(true, isCalled);
            Assert.AreEqual("MyProperty", p);

            TypeT8.States.Clear();
            isCalled = false;

            proxy.PropertyChanged -= ev2;

            Assert.AreEqual(2, TypeT8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT8.States[1]);
            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Event_Are_Called_With_Attributes_With_Target()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeT9();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<TypeT9>(target);
            bool isCalled = false;

            Assert.AreEqual(0, TypeT9.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeT9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT9.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeT9.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeT9.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeT9.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT9.States[4]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(true, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeT9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, TypeT9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT9.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeT9.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT9.States[2]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(true, target.IsCalledRemove);

            TypeT9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            target.RaiseMyEvent();

            Assert.AreEqual(0, TypeT9.States.Count);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);
        }

        private static void CheckAndClearTypeT4()
        {
            Assert.AreEqual(3, TypeT4.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeT4.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeT4.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeT4.States[2]);
            TypeT4.States.Clear();
        }

        [TestMethod]
        public void Event_Generic_Test()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new EventGenericClass();
            var proxy = proxyGenerator.CreateClassProxyWithTarget<EventGenericClass>(target, new IntForEventGenericClass());

            Assert.AreEqual(0, EventGenericClass.States.Count);

            bool isCalled = false;

            EventHandler<MyEventArgs> ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(3, EventGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, EventGenericClass.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, EventGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, EventGenericClass.States[2]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            EventGenericClass.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, EventGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, EventGenericClass.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, EventGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, EventGenericClass.States[2]);

            EventGenericClass.States.Clear();
            isCalled = false;

            target.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }

        #endregion // Events


    }
    public class TypeT1
    {
        public virtual void MethodA()
        {
            States.Add(StateTypes.Class_Method);
            IsCalled = true;
        }

        public static List<StateTypes> States = new List<StateTypes>();

        public short MyShort { get; set; }
        public int MyInt { get; set; }
        public long MyLong { get; set; }
        public ushort MyUShort { get; set; }
        public uint MyUInt { get; set; }
        public ulong MyULong { get; set; }
        public string MyString { get; set; }
        public float MyFloat { get; set; }
        public double MyDouble { get; set; }
        public decimal MyDecimal { get; set; }
        public sbyte MySbyte { get; set; }
        public byte MyByte { get; set; }
        public bool MyBool { get; set; }
        public DateTime MyDateTime { get; set; }
        public DateTimeOffset MyDateTimeOffset { get; set; }
        public Uri MyUri { get; set; }
        public TimeSpan MyTimeSpan { get; set; }
        public Guid MyGuid { get; set; }
        public char MyChar { get; set; }
        public MyEnum MyEnum { get; set; }
        public object MyObj { get; set; }
        public MyItem MyItem { get; set; }
        public List<string> MyList { get; set; }
        public string[] MyArray { get; set; }
        public MyCollection MyCollection { get; set; }
        public bool IsCalled { get; private set; }

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
            this.MyShort = MyShort;
            this.MyInt = MyInt;
            this.MyLong = MyLong;
            this.MyUShort = MyUShort;
            this.MyUInt = MyUInt;
            this.MyULong = MyULong;
            this.MyString = MyString;
            this.MyFloat = MyFloat;
            this.MyDouble = MyDouble;
            this.MyDecimal = MyDecimal;
            this.MySbyte = MySbyte;
            this.MyByte = MyByte;
            this.MyBool = MyBool;
            this.MyDateTime = MyDateTime;
            this.MyDateTimeOffset = MyDateTimeOffset;
            this.MyUri = MyUri;
            this.MyTimeSpan = MyTimeSpan;
            this.MyGuid = MyGuid;
            this.MyChar = MyChar;
            this.MyEnum = MyEnum;
            this.MyObj = MyObj;
            this.MyItem = MyItem;
            this.MyList = MyList;
            this.MyArray = MyArray;
            this.MyCollection = MyCollection;
        }
    }

    public class TypeT2
    {
        public virtual int MyProperty { get; set; }

        public virtual void MethodA()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public class TypeT3
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

    public class TypeT4
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public DateTime d = DateTime.Now;
        public DateTimeOffset dt = new DateTimeOffset(DateTime.Now);
        public Uri uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
        public TimeSpan ts = TimeSpan.FromSeconds(100);
        public Guid g = Guid.NewGuid();

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

    public class TypeT5
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
        public bool IsCalled { get; private set; }

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

            IsCalled = true;
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


            //TypeT5.MyShort = MyShort;
            //TypeT5.MyInt = MyInt;
            //TypeT5.MyLong = MyLong;
            //TypeT5.MyUShort = MyUShort;
            //TypeT5.MyUInt = MyUInt;
            //TypeT5.MyULong = MyULong;
            //TypeT5.MyString = MyString;
            //TypeT5.MyFloat = MyFloat;
            //TypeT5.MyDouble = MyDouble;
            //TypeT5.MyDecimal = MyDecimal;
            //TypeT5.MySbyte = MySbyte;
            //TypeT5.MyByte = MyByte;
            //TypeT5.MyBool = MyBool;
            //TypeT5.MyDateTime = MyDateTime;
            //TypeT5.MyDateTimeOffset = MyDateTimeOffset;
            //TypeT5.MyUri = MyUri;
            //TypeT5.MyTimeSpan = MyTimeSpan;
            //TypeT5.MyGuid = MyGuid;
            //TypeT5.MyChar = MyChar;
            //TypeT5.MyEnum = MyEnum;
            //TypeT5.MyObj = MyObj;
            //TypeT5.MyItem = MyItem;
            //TypeT5.MyList = MyList;
            //TypeT5.MyArray = MyArray;
            //TypeT5.MyCollection = MyCollection;
        }
    }

    public class TypeT6
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public DateTime d = DateTime.Now;
        public DateTimeOffset dt = new DateTimeOffset(DateTime.Now);
        public Uri uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
        public TimeSpan ts = TimeSpan.FromSeconds(100);
        public Guid g = Guid.NewGuid();

        public bool IsCalled { get; private set; }

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
            IsCalled = true;
        }
    }

    [AllInterceptor(typeof(IntForT7))]
    public class TypeT7
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public bool IsCalled { get; private set; }

        [MethodInterceptor(typeof(IntForT7_b))]
        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
            IsCalled = true;
        }
    }


    public class TypeT8 : INotifyPropertyChanged
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


    [AllInterceptor(typeof(IntForT9))]
    public class TypeT9
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;

        public bool IsCalledAdd = false;
        public bool IsCalledRemove = false;

        [AddEventInterceptor(typeof(IntForT9_b))]
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


    [AllInterceptor(typeof(IntForT10))]
    public interface ITypeT10
    {
        [AddEventInterceptor(typeof(IntForT10_b))]
        event EventHandler MyEvent;

        void RaiseMyEvent();
    }

    public class TypeT10 : ITypeT10
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


    public class IntForT1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT3 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT3.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT3.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT4 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT4.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT4.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT5 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT5.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT5.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT6 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT6.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT6.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT7 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT7.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT7.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT7_b : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeT7.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeT7.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForT9 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeT9.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeT9.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT9_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeT9.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeT9.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForT10 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeT10.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeT10.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForT10_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeT10.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeT10.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForT8 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeT8.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeT8.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
