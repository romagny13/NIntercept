using NIntercept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//namespace Overview1
//{
//    // Class Proxy => inherit from class
//    // OVERRIDE only VIRTUAL properties, methods, events
//    // no need to copy attributes to type and methods ("get_...", "set_..","Method", "add_...", "remove_...")
//    // option do not override member ?? cannot with method names

//    [AllInterceptor(typeof(LogInterceptor))] // inherit
//    public class MyClass
//    {
//        [PropertySetInterceptor(typeof(LogInterceptor))] // inherit
//        public virtual string Title { get; set; }

//        [MethodInterceptor(typeof(LogInterceptor))] // inherit
//        public virtual string Method()
//        {
//            return "Ok";
//        }

//        [AddEventInterceptor(typeof(LogInterceptor))] // inherit
//        public virtual event EventHandler MyEvent;
//    }

//    public class LogInterceptor : Interceptor
//    {
//        protected override void OnEnter(IInvocation invocation)
//        {
//            var parameter = invocation.Parameters.Length > 0 ? invocation.Parameters[0] : null;
//            Console.WriteLine($"[LogInterceptor] Enter '{invocation.CallerMethod.Name}' '{parameter}'");
//        }

//        protected override void OnException(IInvocation invocation, Exception exception)
//        {
//            Console.WriteLine($"[LogInterceptor] Exception '{invocation.CallerMethod.Name}', '{exception.Message}'");
//        }

//        protected override void OnExit(IInvocation invocation)
//        {
//            Console.WriteLine($"[LogInterceptor] Exit '{invocation.CallerMethod.Name}', Result: '{invocation.ReturnValue}'");
//        }
//    }

//    public class MyClass_Proxy : MyClass
//    {
//        private IInterceptor[] interceptors;

//        public MyClass_Proxy(IInterceptor[] interceptors)
//        {
//            this.interceptors = interceptors;
//        }

//        public override string Title
//        {
//            get
//            {
//                // BindingFlags only if non public ?
//                var invocation = new MyClass_get_Title_Invocation(
//                    interceptors,
//                    typeof(MyClass).GetProperty("Title"),
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[0]);
//                invocation.Proceed();
//                return (string)invocation.ReturnValue;
//            }
//            set
//            {
//                var invocation = new MyClass_set_Title_Invocation(
//                  interceptors,
//                  typeof(MyClass).GetProperty("Title"),
//                  (MethodInfo)MethodBase.GetCurrentMethod(),
//                  this, new object[] { value });
//                invocation.Proceed();
//            }
//        }

//        public string get_Title_Callback()
//        {
//            return base.Title;
//        }

//        public void set_Title_Callback(string value)
//        {
//            base.Title = value;
//        }

//        public override string Method()
//        {
//            var invocation = new MyClass_Method_Invocation(
//                    interceptors,
//                    (MethodInfo)MethodBase.GetCurrentMethod(), // method from handle
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[0]);
//            invocation.Proceed();
//            return (string)invocation.ReturnValue;
//        }

//        public string Method_Callback()
//        {
//            return base.Method();
//        }

//        public override event EventHandler MyEvent
//        {
//            add
//            {
//                var invocation = new MyClass_add_MyEvent_Invocation(
//                    interceptors,
//                    typeof(MyClass).GetEvent("MyEvent"), // BindingsFalgs only for NonPublic 
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[] { value });
//                invocation.Proceed();
//            }
//            remove
//            {
//                var invocation = new MyClass_remove_MyEvent_Invocation(
//                interceptors,
//                typeof(MyClass).GetEvent("MyEvent"), // BindingsFalgs only for NonPublic 
//                (MethodInfo)MethodBase.GetCurrentMethod(),
//                this, new object[] { value });
//                invocation.Proceed();
//            }
//        }

//        public void add_MyEvent_Callback(EventHandler value)
//        {
//            base.MyEvent += value;
//        }

//        public void remove_MyEvent_Callback(EventHandler value)
//        {
//            base.MyEvent -= value;
//        }
//    }

//    public class MyClass_get_Title_Invocation : Invocation
//    {
//        public MyClass_get_Title_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IPropertyGetInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            ReturnValue = (Proxy as MyClass_Proxy).get_Title_Callback();
//        }
//    }

//    public class MyClass_set_Title_Invocation : Invocation
//    {
//        public MyClass_set_Title_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IPropertySetInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as MyClass_Proxy).set_Title_Callback((string)Parameters[0]);
//        }
//    }

//    public class MyClass_Method_Invocation : Invocation
//    {
//        public MyClass_Method_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IMethodInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            ReturnValue = (Proxy as MyClass_Proxy).Method_Callback();
//        }
//    }

//    public class MyClass_add_MyEvent_Invocation : Invocation
//    {
//        public MyClass_add_MyEvent_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IAddEventInterceptorProvider);

//        //public T GetInterceptorProviderType<T>() where T: IInterceptorProvider
//        //{
//        //    return(T) InterceptorProviderType; // NO, require instance
//        //}

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as MyClass_Proxy).add_MyEvent_Callback((EventHandler)Parameters[0]);
//        }
//    }

//    public class MyClass_remove_MyEvent_Invocation : Invocation
//    {
//        public MyClass_remove_MyEvent_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IRemoveEventInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as MyClass_Proxy).remove_MyEvent_Callback((EventHandler)Parameters[0]);
//        }
//    }
//}

//// proxy
//// command => invocation => interceptor => callback => update Title property (proxy) ... set title called .. invoc..interceptor ... set target title .. notify after

//namespace Overview2
//{
//    // Interface proxy => Implement interface
//    // New slot for all members
//    // copy attributes for interface and members implemented
//    // methods callback body and indexers hard to implement // require to know the body

//    public class LogInterceptor : Interceptor
//    {
//        protected override void OnEnter(IInvocation invocation)
//        {
//            var parameter = invocation.Parameters.Length > 0 ? invocation.Parameters[0] : null;
//            Console.WriteLine($"[LogInterceptor] Enter '{invocation.CallerMethod.Name}' '{parameter}'");
//        }

//        protected override void OnException(IInvocation invocation, Exception exception)
//        {
//            Console.WriteLine($"[LogInterceptor] Exception '{invocation.CallerMethod.Name}', '{exception.Message}'");
//        }

//        protected override void OnExit(IInvocation invocation)
//        {
//            Console.WriteLine($"[LogInterceptor] Exit '{invocation.CallerMethod.Name}', Result: '{invocation.ReturnValue}'");
//        }
//    }

//    [AllInterceptor(typeof(LogInterceptor))] // copy
//    public interface IMyClass
//    {
//        [PropertySetInterceptor(typeof(LogInterceptor))] // copy
//        string Title { get; set; }

//        [MethodInterceptor(typeof(LogInterceptor))] // copy
//        string Method();

//        [AddEventInterceptor(typeof(LogInterceptor))] // copy
//        event EventHandler MyEvent;
//    }

//    public class IMyClass_Proxy : IMyClass
//    {
//        private IInterceptor[] interceptors;

//        public IMyClass_Proxy(IInterceptor[] interceptors)
//        {
//            this.interceptors = interceptors;
//        }

//        private string _title; // create field for property

//        public string Title // no override
//        {
//            get
//            {
//                // BindingFlags only if non public ?
//                var invocation = new IMyClass_get_Title_Invocation(
//                    interceptors,
//                    typeof(IMyClass).GetProperty("Title"),
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[0]);
//                invocation.Proceed();
//                return (string)invocation.ReturnValue;
//            }
//            set
//            {
//                var invocation = new IMyClass_set_Title_Invocation(
//                  interceptors,
//                  typeof(IMyClass).GetProperty("Title"),
//                  (MethodInfo)MethodBase.GetCurrentMethod(),
//                  this, new object[] { value });
//                invocation.Proceed();
//            }
//        }

//        public string get_Title_Callback()
//        {
//            return this._title; // ?? target , field ?
//        }

//        public void set_Title_Callback(string value)
//        {
//            this._title = value;
//        }

//        public string Method()
//        {
//            var invocation = new IMyClass_Method_Invocation(
//                    interceptors,
//                    (MethodInfo)MethodBase.GetCurrentMethod(), // method from handle
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[0]);
//            invocation.Proceed();
//            return (string)invocation.ReturnValue;
//        }

//        public string Method_Callback()
//        {
//            return default; // target ?? handler ? default ??
//        }

//        // create field
//        private event EventHandler _myEvent;

//        public event EventHandler MyEvent
//        {
//            add
//            {
//                var invocation = new IMyClass_add_MyEvent_Invocation(
//                    interceptors,
//                    typeof(IMyClass).GetEvent("MyEvent"), // BindingsFalgs only for NonPublic 
//                    (MethodInfo)MethodBase.GetCurrentMethod(),
//                    this, new object[] { value });
//                invocation.Proceed();
//            }
//            remove
//            {
//                var invocation = new IMyClass_remove_MyEvent_Invocation(
//                interceptors,
//                typeof(IMyClass).GetEvent("MyEvent"), // BindingsFalgs only for NonPublic 
//                (MethodInfo)MethodBase.GetCurrentMethod(),
//                this, new object[] { value });
//                invocation.Proceed();
//            }
//        }

//        public void add_MyEvent_Callback(EventHandler value)
//        {
//            _myEvent += value;
//        }

//        public void remove_MyEvent_Callback(EventHandler value)
//        {
//            _myEvent -= value;
//        }
//    }

//    public class IMyClass_get_Title_Invocation : Invocation
//    {
//        public IMyClass_get_Title_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IPropertyGetInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            ReturnValue = (Proxy as IMyClass_Proxy).get_Title_Callback();
//        }
//    }

//    public class IMyClass_set_Title_Invocation : Invocation
//    {
//        public IMyClass_set_Title_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IPropertySetInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as IMyClass_Proxy).set_Title_Callback((string)Parameters[0]);
//        }
//    }

//    public class IMyClass_Method_Invocation : Invocation
//    {
//        public IMyClass_Method_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IMethodInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            ReturnValue = (Proxy as IMyClass_Proxy).Method_Callback();
//        }
//    }

//    public class IMyClass_add_MyEvent_Invocation : Invocation
//    {
//        public IMyClass_add_MyEvent_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IAddEventInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as IMyClass_Proxy).add_MyEvent_Callback((EventHandler)Parameters[0]);
//        }
//    }

//    public class IMyClass_remove_MyEvent_Invocation : Invocation
//    {
//        public IMyClass_remove_MyEvent_Invocation(IInterceptor[] interceptors, MemberInfo member, MethodInfo callerMethod, object proxy, object[] parameters)
//            : base(interceptors, member, callerMethod, proxy, parameters)
//        {
//        }

//        public override Type InterceptorProviderType => typeof(IRemoveEventInterceptorProvider);

//        protected override void InvokeMethodOnTarget()
//        {
//            (Proxy as IMyClass_Proxy).remove_MyEvent_Callback((EventHandler)Parameters[0]);
//        }
//    }


//    public interface IMyCollection
//    {
//        object this[int index] { get; set; }
//    }

//    public class IMyCollection_Proxy : IMyCollection
//    {
//        public object this[int index]
//        {
//            get
//            {
//                return get_Item_Callback(index);
//            }
//            set
//            {
//                set_Item_Callback(index, value);
//            }
//        }

//        public object get_Item_Callback(int index)
//        {
//            return null;
//        }

//        public void set_Item_Callback(int index, object value)
//        {

//        }
//    }

//    public class MyCollection
//    {
//        private List<object> items = new List<object>();

//        public object this[int index]
//        {
//            get
//            {
//                return items[index];
//            }
//            set
//            {
//                items[index] = value;
//            }
//        }
//    }

//    public class IMYCollection_CodedProxy: IMyCollection
//    {
//        private readonly MyCollection target;
//        private List<object> items = new List<object>();

//        public IMYCollection_CodedProxy(MyCollection target)
//        {
//            this.target = target;
//        }

//        public object this[int index]
//        {
//            get
//            {
//                // invocation => callback
//                return get_Item_Callback(index);
//            }
//            set
//            {
//                set_Item_Callback(index, value);
//            }
//        }

//        public object get_Item_Callback(int index)
//        {
//            return target[index];
//        }

//        public void set_Item_Callback(int index, object value)
//        {
//            target[index] = value;
//        }
//    }
//}
