using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NIntercept.Tests
{
    [TestClass]
    public class ProxyGeneratorWithInterfaceTests
    {
        [TestInitialize()]
        public void Startup()
        {
            MyService.States.Clear();
            MyClassTarget.States.Clear();
            TypeS0.States.Clear();
            TypeS1.States.Clear();
            TypeS3.States.Clear();
            TypeS4.States.Clear();
            TypeS5.States.Clear();
            TypeS6.States.Clear();
            TypeS7.States.Clear();
            TypeS8.States.Clear();
            TypeS9.States.Clear();
            TypeS10.States.Clear();
        }

        [TestMethod]
        public void CallTarget()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new MyService();

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IMyService>(target, new InterceptorInterfWithTarget1());

            Assert.AreEqual(0, MyService.States.Count);

            proxy.MethodA();

            Assert.AreEqual(3, MyService.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyService.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyService.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyService.States[2]);
            Assert.AreEqual(true, target.IsMethodACalled);
        }

        [TestMethod]
        public void GetResult()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new MyService();

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IMyService>(target, new InterceptorInterfWithTarget1(), new InterceptorInterfWithTarget2());

            Assert.AreEqual(0, MyService.States.Count);

            int r = proxy.MethodB();

            Assert.AreEqual(5, MyService.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyService.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, MyService.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, MyService.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, MyService.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyService.States[4]);
            Assert.AreEqual(true, target.IsMethodBCalled);
            Assert.AreEqual(2, r);
        }

        [TestMethod]
        public void GetResult_Generic()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new MyService();

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IMyService>(target, new InterceptorInterfWithTarget1(), new InterceptorInterfWithTarget2());

            Assert.AreEqual(0, MyService.States.Count);

            MyItem r = proxy.MethodC<MyItem>();

            Assert.AreEqual(5, MyService.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyService.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, MyService.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, MyService.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, MyService.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyService.States[4]);
            Assert.AreEqual(true, target.IsMethodCCalled);
            Assert.IsNotNull(r);
        }

        [TestMethod]
        public void Mixed_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new MyClassTarget();

            Assert.AreEqual(0, MyClassTarget.States.Count);

            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IMyClassTarget>(target, new InterceptorTarget1());
            int d = 0;
            bool r = proxy.TryMixed(20, out double b, "Ok", ref d);

            Assert.AreEqual(3, MyClassTarget.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, MyClassTarget.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, MyClassTarget.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, MyClassTarget.States[2]);

            Assert.AreEqual(20, target.t1);
            Assert.AreEqual(10.5, b);
            Assert.AreEqual("Ok", target.t3);
            Assert.AreEqual(100, d);
            Assert.AreEqual(true, r);
        }

        #region Methods

        [TestMethod]
        public void Call_Sample_Method()
        {
            var generator = new ProxyGenerator();

            var target = new TypeS0();
            var proxy = generator.CreateInterfaceProxyWithTarget<ITypeS0>(target, new IntForS0());

            Assert.AreEqual(0, TypeS0.States.Count);

            proxy.MethodA();

            Assert.AreEqual(3, TypeS0.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS0.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS0.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS0.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Call_Empty_Method()
        {
            var generator = new ProxyGenerator();

            var target = new TypeS1();
            var proxy = generator.CreateInterfaceProxyWithTarget<ITypeS1>(target, new IntForS1());

            Assert.AreEqual(0, TypeS1.States.Count);

            proxy.MethodA();

            Assert.AreEqual(3, TypeS1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS1.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Call_Method_With_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS1();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS1>(target, new IntForS1());

            Assert.AreEqual(0, TypeS1.States.Count);

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

            Assert.AreEqual(3, TypeS1.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS1.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS1.States[2]);
        }


        [TestMethod]
        public void Out_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS5();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS5>(target, new IntForS5());

            Assert.AreEqual(0, TypeS5.States.Count);

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
            Assert.AreEqual(MyDateTime, TypeS5.d);
            Assert.AreEqual(MyDateTimeOffset, TypeS5.dt);
            Assert.AreEqual(MyUri, TypeS5.uri);
            Assert.AreEqual(MyTimeSpan, TypeS5.ts);
            Assert.AreEqual(MyGuid, TypeS5.g);
            Assert.AreEqual(MyChar, 'c');
            Assert.AreEqual(MyEnum, MyEnum.First);
            Assert.AreEqual(MyObj, "Obj");
            Assert.AreEqual(MyItem.MyProperty, "A");
            Assert.AreEqual(MyList.Count, 2);
            Assert.AreEqual(MyArray.Length, 2);
            Assert.IsNotNull(MyCollection);

            Assert.AreEqual(3, TypeS5.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS5.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS5.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS5.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Ref_Parameters()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS6();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS6>(target, new IntForS6());

            Assert.AreEqual(0, TypeS6.States.Count);

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

            Assert.AreEqual(3, TypeS6.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS6.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS6.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS6.States[2]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Find_Attributes_On_Type_And_Method()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS7();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS7>(target);

            Assert.AreEqual(0, TypeS7.States.Count);

            proxy.Method();

            Assert.AreEqual(5, TypeS7.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS7.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeS7.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS7.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeS7.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS7.States[4]);
            Assert.AreEqual(true, target.IsCalled);
        }

        [TestMethod]
        public void Method_Results()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS4();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS4>(target, new IntForS4());

            Assert.AreEqual(0, TypeS4.States.Count);

            Assert.AreEqual(proxy.MyShort(), (short)-1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyInt(), (int)-10);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyLong(), (long)-100);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyUShort(), (ushort)1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyUInt(), (uint)10);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyULong(), (ulong)100);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyString(), "A");
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyFloat(), 1.5F);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDouble(), 2.5);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDecimal(), 3.5M);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MySbyte(), (sbyte)1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyByte(), (byte)2);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyBool(), true);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDateTime(), target.d);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDateTimeOffset(), target.dt);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyUri(), target.uri);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyTimeSpan(), target.ts);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyGuid(), target.g);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyChar(), 'c');
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyEnum(), MyEnum.First);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyObj(), "Obj");
            CheckAndClearTypeS4();

            Assert.IsNotNull(proxy.MyItem().MyProperty, "A");
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyList().Count, 2);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyArray().Length, 2);
            CheckAndClearTypeS4();

            Assert.IsNotNull(proxy.MyCollection());
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyT<MyItem>().GetType(), typeof(MyItem));
            CheckAndClearTypeS4();

            // nullables

            Assert.AreEqual(proxy.MyShortN(), (short)-1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyIntN(), (int)-10);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyLongN(), (long)-100);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyUShortN(), (ushort)1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyUIntN(), (uint)10);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyULongN(), (ulong)100);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyFloatN(), 1.5F);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDoubleN(), 2.5);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDecimalN(), 3.5M);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MySbyteN(), (sbyte)1);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyByteN(), (byte)2);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyBoolN(), true);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDateTimeN(), target.d);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyDateTimeOffsetN(), target.dt);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyTimeSpanN(), target.ts);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyGuidN(), target.g);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyCharN(), 'c');
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyEnumN(), MyEnum.First);
            CheckAndClearTypeS4();

            Assert.AreEqual(proxy.MyIntNull(), null);
            CheckAndClearTypeS4();
        }

        #endregion // Methods

        #region Properties


        [TestMethod]
        public void Call_Properties()
        {
            var proxyGenerator = new ProxyGenerator();
            var target = new TypeS3();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS3>(target, new IntForS3());

            var d = DateTime.Now;
            var dt = new DateTimeOffset(d);
            var uri = new Uri("http://mysite.com", UriKind.RelativeOrAbsolute);
            var ts = TimeSpan.FromSeconds(100);
            var g = Guid.NewGuid();

            Assert.AreEqual(0, TypeS3.States.Count);

            proxy.MyShort = -1;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyInt = -10;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyLong = -100;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyUShort = 1;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyUInt = 10;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyULong = 100;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyString = "A";

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyFloat = 1.5F;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyDouble = 2.5;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyDecimal = 3.5M;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MySbyte = 1;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyByte = 2;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyBool = true;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyDateTime = d;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyDateTimeOffset = dt;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyUri = uri;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyTimeSpan = ts;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyGuid = g;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyChar = 'c';

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyEnum = MyEnum.First;

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyObj = "Obj";

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyItem = new MyItem { MyProperty = "A" };

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyList = new List<string> { "A", "B" };

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyArray = new string[] { "A", "B" };

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();

            proxy.MyCollection = new MyCollection();

            Assert.AreEqual(2, TypeS3.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS3.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS3.States[1]);
            TypeS3.States.Clear();
        }

        [TestMethod]
        public void Event_Are_Called_With_Attributes_With_Interface()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeP10();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeP10>(target);
            bool isCalled = false;

            Assert.AreEqual(0, TypeP10.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeP10.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP10.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeP10.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeP10.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeP10.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP10.States[4]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(true, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeP10.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, TypeP10.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeP10.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeP10.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeP10.States[2]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(true, target.IsCalledRemove);

            TypeP10.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            target.RaiseMyEvent();

            Assert.AreEqual(0, TypeP10.States.Count);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);
        }




        #endregion // Properties

        #region Events

        [TestMethod]
        public void Event_Are_Called()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeS8();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS8>(target, new IntForS8());
            bool isCalled = false;

            Assert.AreEqual(0, TypeS8.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(2, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[1]);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeS8.States.Clear();
            isCalled = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(2, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[1]);
            Assert.AreEqual(false, isCalled);

            TypeS8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 += ev;

            Assert.AreEqual(3, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeS8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[2]);
            Assert.AreEqual(false, isCalled);

            target.RaiseMyEvent2();

            Assert.AreEqual(true, isCalled);

            TypeS8.States.Clear();
            isCalled = false;

            proxy.MyEvent2 -= ev;

            Assert.AreEqual(3, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeS8.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[2]);
            Assert.AreEqual(false, isCalled);

            TypeS8.States.Clear();
            isCalled = false;

            string p = null;
            PropertyChangedEventHandler ev2 = (s, e) =>
            {
                isCalled = true;
                p = e.PropertyName;
            };

            proxy.PropertyChanged += ev2;

            Assert.AreEqual(2, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[1]);
            Assert.AreEqual(false, isCalled);

            target.RaisePropertyChanged("MyProperty");

            Assert.AreEqual(true, isCalled);
            Assert.AreEqual("MyProperty", p);

            TypeS8.States.Clear();
            isCalled = false;

            proxy.PropertyChanged -= ev2;

            Assert.AreEqual(2, TypeS8.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS8.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS8.States[1]);
            Assert.AreEqual(false, isCalled);
        }

        [TestMethod]
        public void Event_Are_Called_With_Attributes_With_Target()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new TypeS9();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<ITypeS9>(target);
            bool isCalled = false;

            Assert.AreEqual(0, TypeS9.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeS9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS9.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeS9.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeS9.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeS9.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS9.States[4]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(true, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);

            target.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            TypeS9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, TypeS9.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS9.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeS9.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS9.States[2]);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(true, target.IsCalledRemove);

            TypeS9.States.Clear();
            isCalled = false;
            target.IsCalledAdd = false;
            target.IsCalledRemove = false;

            target.RaiseMyEvent();

            Assert.AreEqual(0, TypeS9.States.Count);
            Assert.AreEqual(false, isCalled);
            Assert.AreEqual(false, target.IsCalledAdd);
            Assert.AreEqual(false, target.IsCalledRemove);
        }

        private static void CheckAndClearTypeS4()
        {
            Assert.AreEqual(3, TypeS4.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS4.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeS4.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS4.States[2]);
            TypeS4.States.Clear();
        }

        #endregion // Events

        [TestMethod]
        public void DiscoverAndImplementInterfaces()
        {
            var generator = new ProxyGenerator();

            var target = new TypeS11();
            var proxy = generator.CreateInterfaceProxyWithTarget<IContracts3>(target, new IntForS11());

            Assert.AreEqual(0, TypeS11.States.Count);

            proxy.MethodA();

            Assert.AreEqual(2, TypeS11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS11.States[1]);
            Assert.AreEqual(true, target.IsMethodACalled);

            TypeS11.States.Clear();

            proxy.MethodB();

            Assert.AreEqual(2, TypeS11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS11.States[1]);
            Assert.AreEqual(true, target.IsMethodBCalled);

            TypeS11.States.Clear();

            var v = proxy.MyInt;

            Assert.AreEqual(2, TypeS11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS11.States[1]);
            Assert.AreEqual(true, target.IsPropCalled);

            TypeS11.States.Clear();

            EventHandler ev = null;
            ev = (s, e) => { };
            proxy.MyEvent += ev;

            Assert.AreEqual(2, TypeS11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS11.States[1]);
            Assert.AreEqual(true, target.IsAddEventCalled);

            TypeS11.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(2, TypeS11.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS11.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS11.States[1]);
            Assert.AreEqual(true, target.IsRemoveEventCalled);
        }

        [TestMethod]
        public void Set_Properties_With_Same_Name_That_Implement_2_Interfaces()
        {
            var generator = new ProxyGenerator();
            var target = new TypeS13();
            var proxy = generator.CreateInterfaceProxyWithTarget<IContracts>(target);

            ((IContract1)proxy).MyProperty = "A";

            Assert.AreEqual(0, TypeS13.States.Count);

            ((IContract2)proxy).MyProperty = "B";

            Assert.AreEqual(2, TypeS13.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeS13.States[0]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeS13.States[1]);

            Assert.AreEqual("A", ((IContract1)proxy).MyProperty);
            Assert.AreEqual("B", ((IContract2)proxy).MyProperty);
        }

        [TestMethod]
        public void Event_Generic_Test()
        {
            var proxyGenerator = new ProxyGenerator();

            var target = new EventGenericClass();
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget<IEventGenericClass>(target, new IntForEventGenericClass());

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

            proxy.RaiseMyEvent();

            Assert.AreEqual(true, isCalled);

            EventGenericClass.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(3, EventGenericClass.States.Count);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, EventGenericClass.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, EventGenericClass.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, EventGenericClass.States[2]);

            EventGenericClass.States.Clear();
            isCalled = false;

            proxy.RaiseMyEvent();

            Assert.AreEqual(false, isCalled);
        }
    }


    public class InterceptorInterfWithTarget1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            MyService.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            MyService.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class InterceptorInterfWithTarget2 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            MyService.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            MyService.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public interface IMyClassTarget
    {
        void Method(bool a, int b, string c, MyEnum d, object e, MyItem f, List<string> g, int? h);
        void MethodWithGenericsParameters_NoResult<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
        bool TryMixed(int a, out double b, string c, ref int d);
    }

    public class MyClassTarget : IMyClassTarget
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public bool a;
        public int b;
        public string c;
        public MyEnum d;
        public object e;
        public MyItem f;
        public List<string> g;
        public int? h;

        public object p1;
        public object p2;
        public object p3;
        public object p4;
        public object p5;
        public object p6;
        public object p7;
        public object p8;

        public int t1 { get; private set; }
        public string t3 { get; private set; }
        public virtual void Method(bool a, int b, string c, MyEnum d, object e, MyItem f, List<string> g, int? h)
        {
            States.Add(StateTypes.Class_Method);
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
            this.h = h;
        }

        public virtual void MethodWithGenericsParameters_NoResult<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h)
        {
            this.p1 = a;
            this.p2 = b;
            this.p3 = c;
            this.p4 = d;
            this.p5 = e;
            this.p6 = f;
            this.p7 = g;
            this.p8 = h;

            States.Add(StateTypes.Class_Method);
        }

        public virtual bool TryMixed(int a, out double b, string c, ref int d)
        {
            States.Add(StateTypes.Class_Method);

            b = 0;

            t1 = a;
            b = 10.5;
            t3 = c;
            d = 100;
            return true;
        }
    }

    public class InterceptorTarget1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            MyClassTarget.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            MyClassTarget.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public interface IMyService
    {
        void MethodA();
        int MethodB();

        T MethodC<T>() where T : new();
    }

    public class MyService : IMyService
    {
        public bool IsMethodACalled { get; set; }
        public bool IsMethodBCalled { get; set; }
        public bool IsMethodCCalled { get; set; }

        public static List<StateTypes> States = new List<StateTypes>();

        public void MethodA()
        {
            States.Add(StateTypes.Class_Method);
            IsMethodACalled = true;
        }

        public int MethodB()
        {
            States.Add(StateTypes.Class_Method);
            IsMethodBCalled = true;
            return 2;
        }

        public T MethodC<T>() where T : new()
        {
            States.Add(StateTypes.Class_Method);
            IsMethodCCalled = true;
            return new T();
        }
    }

    public interface IContract1
    {
        string MyProperty { get; set; }
    }

    public interface IContract2
    {
        [SetterInterceptor(typeof(IntForS13))]
        string MyProperty { get; set; }
    }

    public interface IContracts : IContract1, IContract2
    {

    }

    public class TypeS13 : IContracts
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public string MyProperty { get; set; }

        string IContract2.MyProperty { get; set; }
    }

    public class IntForS13 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS13.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS13.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public interface ITypeS0
    {
        bool IsCalled { get; }

        void MethodA();
    }

    public class TypeS0 : ITypeS0
    {
        public void MethodA()
        {
            States.Add(StateTypes.Class_Method);
            IsCalled = true;
        }

        public static List<StateTypes> States = new List<StateTypes>();

        public bool IsCalled { get; set; }
    }

    public interface ITypeS1
    {
        bool IsCalled { get; }
        string[] MyArray { get; set; }
        bool MyBool { get; set; }
        byte MyByte { get; set; }
        char MyChar { get; set; }
        MyCollection MyCollection { get; set; }
        DateTime MyDateTime { get; set; }
        DateTimeOffset MyDateTimeOffset { get; set; }
        decimal MyDecimal { get; set; }
        double MyDouble { get; set; }
        MyEnum MyEnum { get; set; }
        float MyFloat { get; set; }
        Guid MyGuid { get; set; }
        int MyInt { get; set; }
        MyItem MyItem { get; set; }
        List<string> MyList { get; set; }
        long MyLong { get; set; }
        object MyObj { get; set; }
        sbyte MySbyte { get; set; }
        short MyShort { get; set; }
        string MyString { get; set; }
        TimeSpan MyTimeSpan { get; set; }
        uint MyUInt { get; set; }
        ulong MyULong { get; set; }
        Uri MyUri { get; set; }
        ushort MyUShort { get; set; }

        void MethodA();
        void Parameters(short MyShort, int MyInt, long MyLong, ushort MyUShort, uint MyUInt, ulong MyULong, string MyString, float MyFloat, double MyDouble, decimal MyDecimal, sbyte MySbyte, byte MyByte, bool MyBool, DateTime MyDateTime, DateTimeOffset MyDateTimeOffset, Uri MyUri, TimeSpan MyTimeSpan, Guid MyGuid, char MyChar, MyEnum MyEnum, object MyObj, MyItem MyItem, List<string> MyList, string[] MyArray, MyCollection MyCollection);
    }

    public class TypeS1 : ITypeS1
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

    public interface ITypeS2
    {
        int MyProperty { get; set; }

        event EventHandler MyEvent;

        void MethodA();
    }

    public class TypeS2 : ITypeS2
    {
        public virtual int MyProperty { get; set; }

        public virtual void MethodA()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public interface ITypeS3
    {
        string[] MyArray { get; set; }
        bool MyBool { get; set; }
        byte MyByte { get; set; }
        char MyChar { get; set; }
        MyCollection MyCollection { get; set; }
        DateTime MyDateTime { get; set; }
        DateTimeOffset MyDateTimeOffset { get; set; }
        decimal MyDecimal { get; set; }
        double MyDouble { get; set; }
        MyEnum MyEnum { get; set; }
        float MyFloat { get; set; }
        Guid MyGuid { get; set; }
        int MyInt { get; set; }
        MyItem MyItem { get; set; }
        List<string> MyList { get; set; }
        long MyLong { get; set; }
        object MyObj { get; set; }
        sbyte MySbyte { get; set; }
        short MyShort { get; set; }
        string MyString { get; set; }
        TimeSpan MyTimeSpan { get; set; }
        uint MyUInt { get; set; }
        ulong MyULong { get; set; }
        Uri MyUri { get; set; }
        ushort MyUShort { get; set; }
    }

    public class TypeS3 : ITypeS3
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

    public interface ITypeS4
    {
        string[] MyArray();
        bool MyBool();
        bool? MyBoolN();
        byte MyByte();
        byte? MyByteN();
        char MyChar();
        char? MyCharN();
        MyCollection MyCollection();
        DateTime MyDateTime();
        DateTime? MyDateTimeN();
        DateTimeOffset MyDateTimeOffset();
        DateTimeOffset? MyDateTimeOffsetN();
        decimal MyDecimal();
        decimal? MyDecimalN();
        double MyDouble();
        double? MyDoubleN();
        MyEnum MyEnum();
        MyEnum? MyEnumN();
        float MyFloat();
        float? MyFloatN();
        Guid MyGuid();
        Guid? MyGuidN();
        int MyInt();
        int? MyIntN();
        int? MyIntNull();
        MyItem MyItem();
        List<string> MyList();
        long MyLong();
        long? MyLongN();
        object MyObj();
        sbyte MySbyte();
        sbyte? MySbyteN();
        short MyShort();
        short? MyShortN();
        string MyString();
        T MyT<T>() where T : new();
        TimeSpan MyTimeSpan();
        TimeSpan? MyTimeSpanN();
        uint MyUInt();
        uint? MyUIntN();
        ulong MyULong();
        ulong? MyULongN();
        Uri MyUri();
        ushort MyUShort();
        ushort? MyUShortN();
    }

    public class TypeS4 : ITypeS4
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

    public interface ITypeS5
    {
        bool IsCalled { get; }

        void Parameters(out short MyShort, out int MyInt, out long MyLong, out ushort MyUShort, out uint MyUInt, out ulong MyULong, out string MyString, out float MyFloat, out double MyDouble, out decimal MyDecimal, out sbyte MySbyte, out byte MyByte, out bool MyBool, out DateTime MyDateTime, out DateTimeOffset MyDateTimeOffset, out Uri MyUri, out TimeSpan MyTimeSpan, out Guid MyGuid, out char MyChar, out MyEnum MyEnum, out object MyObj, out MyItem MyItem, out List<string> MyList, out string[] MyArray, out MyCollection MyCollection);
    }

    public class TypeS5 : ITypeS5
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


            //TypeS5.MyShort = MyShort;
            //TypeS5.MyInt = MyInt;
            //TypeS5.MyLong = MyLong;
            //TypeS5.MyUShort = MyUShort;
            //TypeS5.MyUInt = MyUInt;
            //TypeS5.MyULong = MyULong;
            //TypeS5.MyString = MyString;
            //TypeS5.MyFloat = MyFloat;
            //TypeS5.MyDouble = MyDouble;
            //TypeS5.MyDecimal = MyDecimal;
            //TypeS5.MySbyte = MySbyte;
            //TypeS5.MyByte = MyByte;
            //TypeS5.MyBool = MyBool;
            //TypeS5.MyDateTime = MyDateTime;
            //TypeS5.MyDateTimeOffset = MyDateTimeOffset;
            //TypeS5.MyUri = MyUri;
            //TypeS5.MyTimeSpan = MyTimeSpan;
            //TypeS5.MyGuid = MyGuid;
            //TypeS5.MyChar = MyChar;
            //TypeS5.MyEnum = MyEnum;
            //TypeS5.MyObj = MyObj;
            //TypeS5.MyItem = MyItem;
            //TypeS5.MyList = MyList;
            //TypeS5.MyArray = MyArray;
            //TypeS5.MyCollection = MyCollection;
        }
    }

    public interface ITypeS6
    {
        bool IsCalled { get; }

        void Parameters(ref short MyShort, ref int MyInt, ref long MyLong, ref ushort MyUShort, ref uint MyUInt, ref ulong MyULong, ref string MyString, ref float MyFloat, ref double MyDouble, ref decimal MyDecimal, ref sbyte MySbyte, ref byte MyByte, ref bool MyBool, ref DateTime MyDateTime, ref DateTimeOffset MyDateTimeOffset, ref Uri MyUri, ref TimeSpan MyTimeSpan, ref Guid MyGuid, ref char MyChar, ref MyEnum MyEnum, ref object MyObj, ref MyItem MyItem, ref List<string> MyList, ref string[] MyArray, ref MyCollection MyCollection);
    }

    public class TypeS6 : ITypeS6
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

    [AllInterceptor(typeof(IntForS7))]
    public interface ITypeS7
    {
        bool IsCalled { get; }

        [MethodInterceptor(typeof(IntForS7_b))]
        void Method();
    }

    public class TypeS7 : ITypeS7
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public bool IsCalled { get; private set; }

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
            IsCalled = true;
        }
    }

    public interface ITypeS8 : INotifyPropertyChanged
    {
        event EventHandler MyEvent;
        event EventHandler MyEvent2;

        void RaiseMyEvent();
        void RaiseMyEvent2();
        void RaisePropertyChanged(string p);
    }

    public class TypeS8 : ITypeS8
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

    [AllInterceptor(typeof(IntForS9))]
    public interface ITypeS9
    {
        [AddOnInterceptor(typeof(IntForS9_b))]
        event EventHandler MyEvent;

        void RaiseMyEvent();
    }

    public class TypeS9 : ITypeS9
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


    [AllInterceptor(typeof(IntForS10))]
    public interface ITypeS10
    {
        [AddOnInterceptor(typeof(IntForS10_b))]
        event EventHandler MyEvent;

        void RaiseMyEvent();
    }

    public class TypeS10 : ITypeS10
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

    public interface IContracts1
    {
        int MyInt { get; }

        event EventHandler MyEvent;
    }

    public interface IContracts2 : IContracts1
    {
        void MethodA();
    }

    public interface IContracts3 : IContracts2
    {

        void MethodB();
    }

    public class TypeS11 : IContracts3
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public int MyInt
        {
            get
            {
                IsPropCalled = true;
                return 10;
            }
        }

        public bool IsAddEventCalled { get; set; }
        public bool IsRemoveEventCalled { get; set; }
        public bool IsPropCalled { get; set; }
        public bool IsMethodACalled { get; set; }
        public bool IsMethodBCalled { get; set; }

        event EventHandler myEvent;
        public event EventHandler MyEvent
        {
            add
            {
                IsAddEventCalled = true;
                myEvent += value;
            }
            remove
            {
                IsRemoveEventCalled = true;
                myEvent -= value;
            }
        }

        public void MethodA()
        {
            IsMethodACalled = true;
        }

        public void MethodB()
        {
            IsMethodBCalled = true;
        }
    }

    public class IntForS11 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS11.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS11.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS0 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS0.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS0.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS1 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS1.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS1.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS3 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS3.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS3.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS4 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS4.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS4.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS5 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS5.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS5.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS6 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS6.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS6.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS7 : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS7.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS7.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS7_b : IInterceptor
    {
        public static IInvocation invocationBefore;
        public static IInvocation invocationAfter;

        public void Intercept(IInvocation invocation)
        {
            TypeS7.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocationBefore = invocation;
            invocation.Proceed();
            invocationAfter = invocation;

            TypeS7.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForS9 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS9.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS9.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS9_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS9.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeS9.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForS10 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS10.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS10.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForS10_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS10.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeS10.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    public class IntForS8 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeS8.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeS8.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }
}
