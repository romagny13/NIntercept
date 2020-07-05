using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NIntercept.Tests.Builder
{
    // Get and set (idem Add and remove)
    // - interceptors get and set are invoked
    // - interceptors global in code invoked
    // - all interceptor on type invoked
    // - all interceptor + get and set + global invoked
    // - custom all interceptor invoked 
    // - custom all interceptor not invoked 
    // - custom all interceptor + get and set + global invoked
    // - multiple get set interceptors invoked
    // - multiple global invoked
    // - multiple all interceptor invoked
    // - multiple custom all interceptor invoked
    // - multiple all interceptor + get set multiple + multi global invoked

    // Method
    // - method interceptor invoked
    // - global invoked
    // - all interceptor invoked
    // - custom all invoked
    // - custom all not invoked
    // - all + method + global invoked
    // - custom + method + global invoked
    // - multiple all invoked
    // - multiple global invoked
    // - multiple method invoked
    // - multi all + method + global invoked

    // with type
    // with interface
    // with ancestor


    [TestClass]
    public class InterceptorsInvokedTests
    {

        [TestInitialize()]
        public void Startup()
        {
            TypeInv1.States.Clear();
            TypeInv1_b.States.Clear();
            TypeInv1_c.States.Clear();
            TypeInv1_d.States.Clear();
            TypeInv1_e.States.Clear();
            TypeInv1_f.States.Clear();
            TypeInv2.States.Clear();
            TypeInv2_b.States.Clear();
            TypeInv2_c.States.Clear();
            TypeInv2_d.States.Clear();
            TypeInv2_e.States.Clear();
            TypeInv3.States.Clear();
        }

        // Get Set

        [TestMethod]
        public void Get_And_Set_Attribute_Interceptors_Invoked()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1>();

            Assert.AreEqual(0, TypeInv1.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(3, TypeInv1.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1.States[2]);

            TypeInv1.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Mutliple_Get_And_Set_Attribute_Interceptors_Invoked()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_f>();

            Assert.AreEqual(0, TypeInv1_f.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(5, TypeInv1_f.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1_f.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_Set_Called_Before, TypeInv1_f.States[1]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1_f.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_Set_Called_After, TypeInv1_f.States[3]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1_f.States[4]);

            TypeInv1_f.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1_f.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_Get_Called_Before, TypeInv1_f.States[1]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1_f.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_Get_Called_After, TypeInv1_f.States[3]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1_f.States[4]);

            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Get_And_Set_Invoked_With_Global()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_b>(new IntForTypeInv1_b());

            Assert.AreEqual(0, TypeInv1_b.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(3, TypeInv1_b.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1_b.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1_b.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1_b.States[2]);

            TypeInv1_b.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1_b.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1_b.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1_b.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void Get_And_Set_Invoked_With_Custom_Attribute()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_c>();

            Assert.AreEqual(0, TypeInv1_c.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(3, TypeInv1_c.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1_c.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1_c.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1_c.States[2]);

            TypeInv1_c.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1_c.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1_c.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1_c.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void AllInterceptor_Called_For_Get_And_Set()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_d>();

            Assert.AreEqual(0, TypeInv1_d.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(3, TypeInv1_d.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1_d.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1_d.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1_d.States[2]);

            TypeInv1_d.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1_d.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1_d.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1_d.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void CustomAttribute_On_Type_Called_For_Get_And_Set()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_e>();

            Assert.AreEqual(0, TypeInv1_e.States.Count);

            proxy.MyProperty = "A";

            Assert.AreEqual(3, TypeInv1_e.States.Count);

            Assert.AreEqual(StateTypes.Interceptor_Set_Called_Before, TypeInv1_e.States[0]);
            Assert.AreEqual(StateTypes.Set_Called, TypeInv1_e.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Set_Called_After, TypeInv1_e.States[2]);

            TypeInv1_e.States.Clear();

            var v = proxy.MyProperty;

            Assert.AreEqual(StateTypes.Interceptor_Get_Called_Before, TypeInv1_e.States[0]);
            Assert.AreEqual(StateTypes.Get_Called, TypeInv1_e.States[1]);
            Assert.AreEqual(StateTypes.Interceptor_Get_Called_After, TypeInv1_e.States[2]);
            Assert.AreEqual("A", v);
        }

        [TestMethod]
        public void CustomAttribute_On_Type_Is_Called_Only_For_InterceptorProviders()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv1_e>();

            Assert.AreEqual(0, TypeInv1_e.States.Count);

            proxy.Method();

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;
            proxy.MyEvent -= ev;
        }

        // Method

        [TestMethod]
        public void Method_Attribute_Interceptors_Invoked()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2>();

            Assert.AreEqual(0, TypeInv2.States.Count);

            proxy.Method();

            Assert.AreEqual(3, TypeInv2.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeInv2.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeInv2.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeInv2.States[2]);
        }

        [TestMethod]
        public void Mutliple_Method_Attribute_Interceptors_Invoked()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2_b>();

            Assert.AreEqual(0, TypeInv2_b.States.Count);

            proxy.Method();

            Assert.AreEqual(5, TypeInv2_b.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeInv2_b.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledBefore, TypeInv2_b.States[1]);
            Assert.AreEqual(StateTypes.Class_Method, TypeInv2_b.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_IsCalledAfter, TypeInv2_b.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeInv2_b.States[4]);
        }

        [TestMethod]
        public void AllInterceptor_Invoked_For_Method()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2_c>();

            Assert.AreEqual(0, TypeInv2_c.States.Count);

            proxy.Method();

            Assert.AreEqual(3, TypeInv2_c.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeInv2_c.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeInv2_c.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeInv2_c.States[2]);
        }

        [TestMethod]
        public void Custom_Attribute_On_Type_Invoked_For_Method()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2_d>();

            Assert.AreEqual(0, TypeInv2_d.States.Count);

            proxy.Method();

            Assert.AreEqual(3, TypeInv2_d.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeInv2_d.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeInv2_d.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeInv2_d.States[2]);
        }

        [TestMethod]
        public void Custom_Attribute_On_Type_Is_Called_Only_For_Method()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2_d>();

            Assert.AreEqual(0, TypeInv2_d.States.Count);

            proxy.MyProperty = "A";

            var v = proxy.MyProperty;

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;
            proxy.MyEvent -= ev;
        }

        [TestMethod]
        public void Global_Interceptor_Invoked_For_Method()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv2_e>(new IntForTypeInv2_e());

            Assert.AreEqual(0, TypeInv2_e.States.Count);

            proxy.Method();

            Assert.AreEqual(3, TypeInv2_e.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_IsCalledBefore, TypeInv2_e.States[0]);
            Assert.AreEqual(StateTypes.Class_Method, TypeInv2_e.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_IsCalledAfter, TypeInv2_e.States[2]);
        }

        // event

        [TestMethod]
        public void Attribute_On_Add_remove_Event_Called()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3>();

            Assert.AreEqual(0, TypeInv3.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;

            Assert.AreEqual(3, TypeInv3.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_Before, TypeInv3.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeInv3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_After, TypeInv3.States[2]);

            TypeInv3.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_Before, TypeInv3.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeInv3.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_After, TypeInv3.States[2]);
        }


        [TestMethod]
        public void Global_Interceptor_Called()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3_b>(new IntForTypeInv3_b());

            Assert.AreEqual(0, TypeInv3_b.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;

            Assert.AreEqual(3, TypeInv3_b.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_Before, TypeInv3_b.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeInv3_b.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_After, TypeInv3_b.States[2]);

            TypeInv3_b.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_Before, TypeInv3_b.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeInv3_b.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_After, TypeInv3_b.States[2]);
        }

        [TestMethod]
        public void Multiple_Add_Remove_Interceptor_Called()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3_c>();

            Assert.AreEqual(0, TypeInv3_c.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;

            Assert.AreEqual(5, TypeInv3_c.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_Before, TypeInv3_c.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_Add_Called_Before, TypeInv3_c.States[1]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeInv3_c.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_Add_Called_After, TypeInv3_c.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_After, TypeInv3_c.States[4]);

            TypeInv3_c.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(5, TypeInv3_c.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_Before, TypeInv3_c.States[0]);
            Assert.AreEqual(StateTypes.Interceptor2_Remove_Called_Before, TypeInv3_c.States[1]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeInv3_c.States[2]);
            Assert.AreEqual(StateTypes.Interceptor2_Remove_Called_After, TypeInv3_c.States[3]);
            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_After, TypeInv3_c.States[4]);
        }

        [TestMethod]
        public void All_Interceptor_Invoked_For_Event()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3_d>();

            Assert.AreEqual(0, TypeInv3_d.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;

            Assert.AreEqual(3, TypeInv3_d.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_Before, TypeInv3_d.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeInv3_d.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_After, TypeInv3_d.States[2]);

            TypeInv3_d.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_Before, TypeInv3_d.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeInv3_d.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_After, TypeInv3_d.States[2]);
        }


        [TestMethod]
        public void Custom_Attribute_On_Type_Invoked_For_Event()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3_e>();

            Assert.AreEqual(0, TypeInv3_e.States.Count);

            EventHandler ev = null;
            ev = (s, e) =>
            {

            };
            proxy.MyEvent += ev;

            Assert.AreEqual(3, TypeInv3_e.States.Count);

            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_Before, TypeInv3_e.States[0]);
            Assert.AreEqual(StateTypes.AddEvent_IsCalled, TypeInv3_e.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Add_Called_After, TypeInv3_e.States[2]);

            TypeInv3_e.States.Clear();

            proxy.MyEvent -= ev;

            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_Before, TypeInv3_e.States[0]);
            Assert.AreEqual(StateTypes.RemoveEvent_IsCalled, TypeInv3_e.States[1]);
            Assert.AreEqual(StateTypes.Interceptor1_Remove_Called_After, TypeInv3_e.States[2]);
        }

        [TestMethod]
        public void Custom_Attribute_On_Type_Call_Only_For_InterceptorProviders()
        {
            var generator = new ProxyGenerator();

            var proxy = generator.CreateClassProxy<TypeInv3_e>();

            Assert.AreEqual(0, TypeInv3_e.States.Count);


            proxy.MyProperty = "A";

            var v = proxy.MyProperty;

            proxy.Method();
        }

    }



    #region Get Set

    public class TypeInv1
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        [GetterInterceptor(typeof(IntForTypeInv1_Get))]
        [SetterInterceptor(typeof(IntForTypeInv1_Set))]
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }
    }

    public class IntForTypeInv1_Get : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1.States.Add(StateTypes.Interceptor_Get_Called_Before);

            invocation.Proceed();

            TypeInv1.States.Add(StateTypes.Interceptor_Get_Called_After);
        }
    }

    public class IntForTypeInv1_Set : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1.States.Add(StateTypes.Interceptor_Set_Called_Before);

            invocation.Proceed();

            TypeInv1.States.Add(StateTypes.Interceptor_Set_Called_After);
        }
    }

    // global

    public class TypeInv1_b
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }
    }

    public class IntForTypeInv1_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IGetterInterceptorProvider))
            {
                TypeInv1_b.States.Add(StateTypes.Interceptor_Get_Called_Before);

                invocation.Proceed();

                TypeInv1_b.States.Add(StateTypes.Interceptor_Get_Called_After);
            }
            if (invocation.InterceptorProviderType == typeof(ISetterInterceptorProvider))
            {
                TypeInv1_b.States.Add(StateTypes.Interceptor_Set_Called_Before);

                invocation.Proceed();

                TypeInv1_b.States.Add(StateTypes.Interceptor_Set_Called_After);
            }
        }
    }

    // custom attribute

    public class TypeInv1_c
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        [GetSetInterceptor]
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }
    }

    public class GetSetInterceptorAttribute : Attribute, IGetterInterceptorProvider, ISetterInterceptorProvider
    {
        public Type InterceptorType => typeof(GetSetInterceptor);
    }

    public class GetSetInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IGetterInterceptorProvider))
            {
                TypeInv1_c.States.Add(StateTypes.Interceptor_Get_Called_Before);

                invocation.Proceed();

                TypeInv1_c.States.Add(StateTypes.Interceptor_Get_Called_After);
            }
            if (invocation.InterceptorProviderType == typeof(ISetterInterceptorProvider))
            {
                TypeInv1_c.States.Add(StateTypes.Interceptor_Set_Called_Before);

                invocation.Proceed();

                TypeInv1_c.States.Add(StateTypes.Interceptor_Set_Called_After);
            }
        }
    }

    // all interceptor

    [AllInterceptor(typeof(GetSetInterceptor_ForTypeInv1_d))]
    public class TypeInv1_d
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }
    }

    public class GetSetInterceptor_ForTypeInv1_d : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IGetterInterceptorProvider))
            {
                TypeInv1_d.States.Add(StateTypes.Interceptor_Get_Called_Before);

                invocation.Proceed();

                TypeInv1_d.States.Add(StateTypes.Interceptor_Get_Called_After);
            }
            if (invocation.InterceptorProviderType == typeof(ISetterInterceptorProvider))
            {
                TypeInv1_d.States.Add(StateTypes.Interceptor_Set_Called_Before);

                invocation.Proceed();

                TypeInv1_d.States.Add(StateTypes.Interceptor_Set_Called_After);
            }
        }
    }

    // custom attribute on type

    [GetSetInterceptorAttribute_e]
    public class TypeInv1_e
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public class GetSetInterceptorAttribute_e : Attribute, IGetterInterceptorProvider, ISetterInterceptorProvider
    {
        public Type InterceptorType => typeof(GetSetInterceptor_ForTypeInv1_e);
    }

    public class GetSetInterceptor_ForTypeInv1_e : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IGetterInterceptorProvider))
            {
                TypeInv1_e.States.Add(StateTypes.Interceptor_Get_Called_Before);

                invocation.Proceed();

                TypeInv1_e.States.Add(StateTypes.Interceptor_Get_Called_After);
            }
            else if (invocation.InterceptorProviderType == typeof(ISetterInterceptorProvider))
            {
                TypeInv1_e.States.Add(StateTypes.Interceptor_Set_Called_Before);

                invocation.Proceed();

                TypeInv1_e.States.Add(StateTypes.Interceptor_Set_Called_After);
            }
            else
            {
                throw new Exception("Invalid");
            }
        }
    }

    // multiple

    public class TypeInv1_f
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private string myVar;
        [GetterInterceptor(typeof(IntForTypeInv1_f_Get))]
        [GetterInterceptor(typeof(IntForTypeInv1_f2_Get))]
        [SetterInterceptor(typeof(IntForTypeInv1_f_Set))]
        [SetterInterceptor(typeof(IntForTypeInv1_f2_Set))]
        public virtual string MyProperty
        {
            get
            {
                States.Add(StateTypes.Get_Called);
                return myVar;
            }
            set
            {
                States.Add(StateTypes.Set_Called);
                myVar = value;
            }
        }
    }

    public class IntForTypeInv1_f_Get : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1_f.States.Add(StateTypes.Interceptor_Get_Called_Before);

            invocation.Proceed();

            TypeInv1_f.States.Add(StateTypes.Interceptor_Get_Called_After);
        }
    }

    public class IntForTypeInv1_f2_Get : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1_f.States.Add(StateTypes.Interceptor2_Get_Called_Before);

            invocation.Proceed();

            TypeInv1_f.States.Add(StateTypes.Interceptor2_Get_Called_After);
        }
    }

    public class IntForTypeInv1_f_Set : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1_f.States.Add(StateTypes.Interceptor_Set_Called_Before);

            invocation.Proceed();

            TypeInv1_f.States.Add(StateTypes.Interceptor_Set_Called_After);
        }
    }

    public class IntForTypeInv1_f2_Set : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv1_f.States.Add(StateTypes.Interceptor2_Set_Called_Before);

            invocation.Proceed();

            TypeInv1_f.States.Add(StateTypes.Interceptor2_Set_Called_After);
        }
    }

    #endregion

    #region Method

    public class TypeInv2
    {
        public static List<StateTypes> States = new List<StateTypes>();

        [MethodInterceptor(typeof(IntForTypeInv2))]
        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }

    public class IntForTypeInv2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv2.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeInv2.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    // mutliple

    public class TypeInv2_b
    {
        public static List<StateTypes> States = new List<StateTypes>();

        [MethodInterceptor(typeof(IntForTypeInv2b_1))]
        [MethodInterceptor(typeof(IntForTypeInv2b_2))]
        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }

    public class IntForTypeInv2b_1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv2_b.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeInv2_b.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    public class IntForTypeInv2b_2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv2_b.States.Add(StateTypes.Interceptor2_IsCalledBefore);

            invocation.Proceed();

            TypeInv2_b.States.Add(StateTypes.Interceptor2_IsCalledAfter);
        }
    }

    // all interceptor

    [AllInterceptor(typeof(IntForTypeInv2_ç))]
    public class TypeInv2_c
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }

    public class IntForTypeInv2_ç : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv2_c.States.Add(StateTypes.Interceptor1_IsCalledBefore);

            invocation.Proceed();

            TypeInv2_c.States.Add(StateTypes.Interceptor1_IsCalledAfter);
        }
    }

    // custom

    [TestMethodInterceptor]
    public class TypeInv2_d
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }

        public virtual string MyProperty { get; set; }

        public virtual event EventHandler MyEvent;
    }

    public class TestMethodInterceptorAttribute : Attribute, IMethodInterceptorProvider
    {
        public Type InterceptorType => typeof(MethodInterceptor);
    }

    public class MethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IMethodInterceptorProvider))
            {
                TypeInv2_d.States.Add(StateTypes.Interceptor1_IsCalledBefore);

                invocation.Proceed();

                TypeInv2_d.States.Add(StateTypes.Interceptor1_IsCalledAfter);
            }
            else
                throw new Exception("Invalid");
        }
    }

    // global

    public class TypeInv2_e
    {
        public static List<StateTypes> States = new List<StateTypes>();

        public virtual void Method()
        {
            States.Add(StateTypes.Class_Method);
        }
    }

    public class IntForTypeInv2_e : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IMethodInterceptorProvider))
            {
                TypeInv2_e.States.Add(StateTypes.Interceptor1_IsCalledBefore);

                invocation.Proceed();

                TypeInv2_e.States.Add(StateTypes.Interceptor1_IsCalledAfter);
            }
        }
    }

    #endregion

    #region Event

    public class TypeInv3
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;
        [AddOnInterceptor(typeof(IntForTypeInv3_Add))]
        [RemoveOnInterceptor(typeof(IntForTypeInv3_Remove))]
        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }
    }

    public class IntForTypeInv3_Add : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3.States.Add(StateTypes.Interceptor1_Add_Called_Before);

            invocation.Proceed();

            TypeInv3.States.Add(StateTypes.Interceptor1_Add_Called_After);
        }
    }

    public class IntForTypeInv3_Remove : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3.States.Add(StateTypes.Interceptor1_Remove_Called_Before);

            invocation.Proceed();

            TypeInv3.States.Add(StateTypes.Interceptor1_Remove_Called_After);
        }
    }

    // global


    public class TypeInv3_b
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;
        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }
    }

    public class IntForTypeInv3_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IAddOnInterceptorProvider))
            {
                TypeInv3_b.States.Add(StateTypes.Interceptor1_Add_Called_Before);

                invocation.Proceed();

                TypeInv3_b.States.Add(StateTypes.Interceptor1_Add_Called_After);
            }
            else if (invocation.InterceptorProviderType == typeof(IRemoveOnInterceptorProvider))
            {
                TypeInv3_b.States.Add(StateTypes.Interceptor1_Remove_Called_Before);

                invocation.Proceed();

                TypeInv3_b.States.Add(StateTypes.Interceptor1_Remove_Called_After);
            }
        }
    }

    // mutliple

    public class TypeInv3_c
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;
        [AddOnInterceptor(typeof(IntForTypeInv3_c1_Add))]
        [AddOnInterceptor(typeof(IntForTypeInv3_c2_Add))]
        [RemoveOnInterceptor(typeof(IntForTypeInv3_c1_Remove))]
        [RemoveOnInterceptor(typeof(IntForTypeInv3_c2_Remove))]
        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }
    }

    public class IntForTypeInv3_c1_Add : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3_c.States.Add(StateTypes.Interceptor1_Add_Called_Before);

            invocation.Proceed();

            TypeInv3_c.States.Add(StateTypes.Interceptor1_Add_Called_After);
        }
    }

    public class IntForTypeInv3_c2_Add : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3_c.States.Add(StateTypes.Interceptor2_Add_Called_Before);

            invocation.Proceed();

            TypeInv3_c.States.Add(StateTypes.Interceptor2_Add_Called_After);
        }
    }

    public class IntForTypeInv3_c1_Remove : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3_c.States.Add(StateTypes.Interceptor1_Remove_Called_Before);

            invocation.Proceed();

            TypeInv3_c.States.Add(StateTypes.Interceptor1_Remove_Called_After);
        }
    }

    public class IntForTypeInv3_c2_Remove : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            TypeInv3_c.States.Add(StateTypes.Interceptor2_Remove_Called_Before);

            invocation.Proceed();

            TypeInv3_c.States.Add(StateTypes.Interceptor2_Remove_Called_After);
        }
    }

    // all interceptor

    [AllInterceptor(typeof(IntForTypeInv3_d))]
    public class TypeInv3_d
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;
        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }
    }

    public class IntForTypeInv3_d : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IAddOnInterceptorProvider))
            {
                TypeInv3_d.States.Add(StateTypes.Interceptor1_Add_Called_Before);

                invocation.Proceed();

                TypeInv3_d.States.Add(StateTypes.Interceptor1_Add_Called_After);
            }
            else if (invocation.InterceptorProviderType == typeof(IRemoveOnInterceptorProvider))
            {
                TypeInv3_d.States.Add(StateTypes.Interceptor1_Remove_Called_Before);

                invocation.Proceed();

                TypeInv3_d.States.Add(StateTypes.Interceptor1_Remove_Called_After);
            }
        }
    }

    // custom on type


    [TestAddRemoveInterceptor]
    public class TypeInv3_e
    {
        public static List<StateTypes> States = new List<StateTypes>();

        private event EventHandler myEvent;
        public virtual event EventHandler MyEvent
        {
            add
            {
                States.Add(StateTypes.AddEvent_IsCalled);
                myEvent += value;
            }
            remove
            {
                States.Add(StateTypes.RemoveEvent_IsCalled);
                myEvent -= value;
            }
        }

        public virtual string MyProperty { get; set; }

        public virtual void Method()
        {

        }
    }

    public class TestAddRemoveInterceptorAttribute : Attribute, IAddOnInterceptorProvider, IRemoveOnInterceptorProvider
    {
        public Type InterceptorType => typeof(AddRemoveInterceptorInterceptor);
    }

    public class AddRemoveInterceptorInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.InterceptorProviderType == typeof(IAddOnInterceptorProvider))
            {
                TypeInv3_e.States.Add(StateTypes.Interceptor1_Add_Called_Before);

                invocation.Proceed();

                TypeInv3_e.States.Add(StateTypes.Interceptor1_Add_Called_After);
            }
            else if (invocation.InterceptorProviderType == typeof(IRemoveOnInterceptorProvider))
            {
                TypeInv3_e.States.Add(StateTypes.Interceptor1_Remove_Called_Before);

                invocation.Proceed();

                TypeInv3_e.States.Add(StateTypes.Interceptor1_Remove_Called_After);
            }
            else
                throw new Exception("Invalid");
        }
    }

    #endregion

}
