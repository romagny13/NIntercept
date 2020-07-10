using NIntercept;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Unity;

namespace NetConsoleSample
{  

    class Program
    {
        static void Main(string[] args)
        {
            var generator = new ProxyGenerator(new ModuleScope(true));

            RunTests(generator);
        }

        private static void RunTests(ProxyGenerator generator)
        {
            var watcher = new TestWatcher();

            watcher.Start();

            RunCompleteSample(generator);
            RunProtectedMembersSample(generator);
            RunInterceptorSelectorSample(generator);
            RunInterfaceProxyWithoutTargetSample(generator);
            RunMultiTargetAttributeSample(generator);
            RunMixinsSample(generator);
            RunConstructorInjectionSample(generator);
            RunIndexerSamples(generator);

            watcher.Stop();

            RunAsyncMethodsSample(generator).GetAwaiter().GetResult();

            if (!generator.ModuleScope.IsSaved)
                generator.ModuleScope.Save();

            Console.WriteLine();
            Console.WriteLine($"Test duration: {watcher.Milliseconds}ms (async sample with Task.Delay not include)");
            Console.WriteLine("Press Enter to restart the test");
            var k = Console.ReadKey();
            if (k.Key == ConsoleKey.Enter)
                RunTests(generator);
        }

        private static void RunCompleteSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- Property, method, event ----------------------");
            var proxy = generator.CreateClassProxy<MyClass>();
            // property
            proxy.MyProperty = "New value";
            Console.WriteLine($"Property: '{proxy.MyProperty}'");

            // method
            proxy.MyMethod("A");
            Console.WriteLine($"Property: '{proxy.MyProperty}'");

            // event
            EventHandler handler = null;
            handler = (s, e) =>
            {
                Console.WriteLine("Event raised");
            };
            proxy.MyEvent += handler;

            proxy.MyEvent -= handler;
        }

        private static void RunProtectedMembersSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- Protected Members ----------------------");

            var proxy = generator.CreateClassProxy<ProtectedMembers>();

            proxy.Execute();
        }

        private static void RunInterceptorSelectorSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- InterceptorSelector ----------------------");

            var options = new ProxyGeneratorOptions();
            options.InterceptorSelector = new MyInterceptorSelector();
            options.ClassProxyMemberSelector = new MyClassProxyMemberSelector();

            var proxy = generator.CreateClassProxy<MyClassWithInterceptorSelector>(options, new GetMethodInterceptor(), new SetMethodInterceptor(), new MethodInterceptor());

            proxy.MyProperty = "New Value";

            Console.WriteLine($"Get: {proxy.MyProperty}");

            proxy.MyMethod();

            proxy.MyMethod("My parameter");

            proxy.MyMethod2(); // not intercepted
        }

        private static void RunMultiTargetAttributeSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- Multi targets attribute ----------------------");

            var proxy = generator.CreateClassProxy<MyClass2>();
            proxy.MyMethod("Message");
            proxy.MyProperty = "New value B";
            Console.WriteLine($"Prop:'{proxy.MyProperty}'");
        }

        private static void RunIndexerSamples(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- Indexer with collection ----------------------");
            var proxy = generator.CreateClassProxy<MyCollection>();
            proxy.Add("Item 1");
            Console.WriteLine($"Item value at index 0: '{proxy[0]}'");
            proxy[0] = "New Item value";

            Console.WriteLine("----------------------  Indexer with interface ----------------------");
            var proxyB = generator.CreateInterfaceProxyWithTarget<IMyCollection>(new MyCollectionNotVirtual());

            proxyB.Add("Item 1");
            Console.WriteLine($"Item value at index 0: '{proxyB[0]}'");
            proxyB[0] = "New Item value";
        }

        private static void RunConstructorInjectionSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- Constructor injection ----------------------");

            IUnityContainer container = new UnityContainer();
            container.RegisterType<IMyService, MyService>();

            generator.ConstructorInjectionResolver = new UnityConstructorInjectionResolver(container);
            var proxy = generator.CreateClassProxy<MyClassWithInjections>();
            proxy.Method();

            generator.ConstructorInjectionResolver = null;
        }

        // caution with async void, only for demo
        private async static Task RunAsyncMethodsSample(ProxyGenerator generator)
        {
            Console.WriteLine("----------------------   Async Sample - Task   ----------------------");
            var proxy = generator.CreateClassProxy<MyClassWithAsyncMethods>(new MyAsyncInterceptor());
            await proxy.Method();
            Console.WriteLine("----------------------  Async Sample - Task{T}  ----------------------");
            var result = await proxy.MethodWithResult();
            Console.WriteLine($"Result:{result}");
        }

        private static void RunMixinsSample(ProxyGenerator generator)
        {
            Console.WriteLine("----------------------           Mixins          ----------------------");

            var options = new ProxyGeneratorOptions();
            options.AddMixinInstance(new PropertyChangedMixin());

            var proxy = generator.CreateClassProxy<MyClassWithMixin>(options); // new LogInterceptor());

            proxy.Title = "New title";

            proxy.UpdateTitle(); // caution with ClassProxy and InterfaceProxy with targets, cannot work because the target is called, and this is not the property Title of the proxy that is called

        }

        private static void RunInterfaceProxyWithoutTargetSample(ProxyGenerator generator)
        {
            Console.WriteLine("---------------------- InterfaceProxy without target----------------------");

            var proxy = generator.CreateInterfaceProxyWithoutTarget<IMyService>();

            var result = proxy.GetMessage("Marie");
            Console.WriteLine($"Result:{result}");
        }
    }

    public class LogInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation)
        {
            var parameter = invocation.Parameters.Length > 0 ? invocation.Parameters[0] : null;
            Console.WriteLine($"[LogInterceptor] Enter '{invocation.CallerMethod.Name}' '{parameter}'");
        }

        protected override void OnException(IInvocation invocation, Exception exception)
        {
            Console.WriteLine($"[LogInterceptor] Exception '{invocation.CallerMethod.Name}', '{exception.Message}'");
        }

        protected override void OnExit(IInvocation invocation)
        {
            Console.WriteLine($"[LogInterceptor] Exit '{invocation.CallerMethod.Name}', Result: '{invocation.ReturnValue}'");
        }
    }


    #region Complete sample

    [AllInterceptor(typeof(LogInterceptor))]
    public class MyClass : INotifyPropertyChangedAware
    {
        private string myProperty;
        [SetterInterceptor(typeof(PropertyChangedInterceptor))]
        public virtual string MyProperty
        {
            get { return myProperty; }
            set { myProperty = value; }
        }

        [MethodInterceptor(typeof(MyMethodInterceptor))]
        public virtual void MyMethod(string value)
        {
            this.myProperty = value;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [AddOnInterceptor(typeof(EventInterceptor))]
        public virtual event EventHandler MyEvent;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MyItem { }

    public interface INotifyPropertyChangedAware : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }

    public class PropertyChangedInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation)
        {

        }

        protected override void OnException(IInvocation invocation, Exception exception)
        {

        }

        protected override void OnExit(IInvocation invocation)
        {
            var inpc = invocation.Proxy as INotifyPropertyChangedAware;
            if (inpc != null)
            {
                string propertyName = invocation.Member.Name;
                Console.WriteLine($"[PropertyChangedInterceptor] Notify property changed '{propertyName}'");
                inpc.OnPropertyChanged(propertyName);
            }
        }
    }

    public class MyMethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var value = invocation.GetParameter<string>(0);
            if (value == "A")
                invocation.Parameters[0] = "B";

            invocation.Proceed();
        }
    }

    public class EventInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation)
        {
            Console.WriteLine($"[EventInterceptor] Enter add new subscriber '{invocation.Member.Name}'");
        }

        protected override void OnException(IInvocation invocation, Exception exception)
        {

        }

        protected override void OnExit(IInvocation invocation)
        {
            Console.WriteLine($"[EventInterceptor] Exit add new subscriber '{invocation.Member.Name}'");
        }
    }

    #endregion

    #region Protected Members

    [AllInterceptor(typeof(LogInterceptor))]
    public class ProtectedMembers
    {
        private string myVar;

        protected virtual string MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public void Execute()
        {
            MyProperty = "New Value";
            Console.WriteLine($"MyProperty:{MyProperty}");

            Method();

            MyEvent += PrivateMembers_MyEvent;
            MyEvent -= PrivateMembers_MyEvent;
        }

        private void PrivateMembers_MyEvent(object sender, EventArgs e)
        {

        }

        protected virtual void Method()
        {
            Console.WriteLine("In method");
        }

        protected virtual event EventHandler MyEvent;
    }

    #endregion

    #region InterceptorSelector 

    public class GetMethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[GetMethodInterceptor] {invocation.CallerMethod.Name}");
            invocation.Proceed();
        }
    }

    public class SetMethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[SetMethodInterceptor] {invocation.CallerMethod.Name}");
            invocation.Proceed();
        }
    }

    public class MethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[MethodInterceptor] {invocation.CallerMethod.Name}");
            invocation.Proceed();
        }
    }

    public class MyInterceptorSelector : IInterceptorSelector
    {
        private static readonly IInterceptor[] emptyInterceptorArray = new IInterceptor[0];

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (IsGetMethod(method))
            {
                var getMethodInterceptor = interceptors.First(p => p.GetType() == typeof(GetMethodInterceptor));
                return new IInterceptor[] { getMethodInterceptor };
            }
            else if (IsSetMethod(method))
            {
                var setMethodInterceptor = interceptors.First(p => p.GetType() == typeof(SetMethodInterceptor));
                return new IInterceptor[] { setMethodInterceptor };
            }
            else if (IsAddmethod(method)) { }
            else if (IsRemoveMethod(method)) { }
            else
            {
                var methodInterceptor = interceptors.First(p => p.GetType() == typeof(MethodInterceptor));
                return new IInterceptor[] { methodInterceptor };
            }
            return emptyInterceptorArray;
        }

        private bool IsGetMethod(MethodInfo method)
        {
            return method.Name.StartsWith("get_");
        }

        private bool IsSetMethod(MethodInfo method)
        {
            return method.Name.StartsWith("set_");
        }

        private bool IsAddmethod(MethodInfo method)
        {
            return method.Name.StartsWith("add_");
        }

        private bool IsRemoveMethod(MethodInfo method)
        {
            return method.Name.StartsWith("remove_");
        }
    }

    public class MyClassWithInterceptorSelector
    {
        public virtual string MyProperty { get; set; }

        public virtual void MyMethod()
        {
            Console.WriteLine("In MyMethod");
        }

        public virtual void MyMethod2()
        {
            Console.WriteLine("In MyMethod2");
        }

        public virtual void MyMethod(string p1)
        {
            Console.WriteLine($"In MyMethod with p1 {p1}");
        }

        public void MyMethodNotVirtual()
        {
            Console.WriteLine("In MyMethodNotVirtual");
        }

        public static void MyStaticMethod()
        {
            Console.WriteLine("In MyStaticMethod");
        }
    }

    public class MyClassProxyMemberSelector : ClassProxyMemberSelector
    {
        public override bool Filter(MemberInfo member)
        {
            if (member.DeclaringType == typeof(MyClassWithInterceptorSelector) && member.Name == "MyMethod2")
                return false;

            return true;
        }
    }

    #endregion


    #region multi targets

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class MultiTargetInterceptorAttribute : Attribute,
        IGetterInterceptorProvider,
        ISetterInterceptorProvider,
        IMethodInterceptorProvider
    {
        public Type InterceptorType
        {
            get { return typeof(LogInterceptor); }
        }
    }

    public class MyClass2
    {
        private string myProperty;
        [MultiTargetInterceptor]
        public virtual string MyProperty
        {
            get { return myProperty; }
            set { myProperty = value; }
        }

        [MultiTargetInterceptor]
        public virtual void MyMethod(string value)
        {
            this.myProperty = value;
        }
    }

    #endregion

    #region Indexers

    public interface IMyCollection : IEnumerable<string>
    {
        [MultiTargetInterceptor]
        string this[int index] { get; set; }

        [MultiTargetInterceptor]
        void Add(string item);
    }

    public class MyCollection : IMyCollection
    {
        private List<string> items = new List<string>();

        [MultiTargetInterceptor]
        public virtual string this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        [MultiTargetInterceptor]
        public virtual void Add(string item)
        {
            items.Add(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class MyCollectionNotVirtual : IMyCollection
    {
        private List<string> items = new List<string>();

        public string this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public void Add(string item)
        {
            items.Add(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    #endregion

    #region Async Sample

    public class MyClassWithAsyncMethods
    {
        public virtual async Task Method()
        {
            Console.WriteLine("Enter async Method");
            await Task.Delay(2000);
            Console.WriteLine("Exit async Method");
        }

        public virtual async Task<string> MethodWithResult()
        {
            Console.WriteLine("Enter async Method with result");
            await Task.Delay(2000);
            Console.WriteLine("Exit async Method with result");
            return "Ok";
        }
    }

    //public class MyAsyncInterceptor : IInterceptor
    //{
    //    public void Intercept(IInvocation invocation)
    //    {
    //        Console.WriteLine($"Before interceptor {invocation.Member.Name}");
    //        invocation.Proceed();
    //        Func<Task> continuation = async () =>
    //        {
    //            await (Task)invocation.ReturnValue;
    //            Console.WriteLine($"After interceptor {invocation.Member.Name}");
    //        };
    //        invocation.ReturnValue = continuation();
    //    }
    //}

    // OR
    public class MyAsyncInterceptor : AsyncInterceptor
    {
        protected override void OnEnter(IInvocation invocation)
        {
            Console.WriteLine($"[MyAsyncInterceptor] Enter '{invocation.Member.Name}'");
        }

        protected override void OnException(IInvocation invocation, Exception exception)
        {
            Console.WriteLine($"[MyAsyncInterceptor] Exception '{invocation.Member.Name}', {exception.Message}");
        }

        protected override void OnExit(IInvocation invocation)
        {
            Console.WriteLine($"[MyAsyncInterceptor] Exit '{invocation.Member.Name}' {invocation.ReturnValue}");
        }
    }

    #endregion

    #region Mixin

    public class MyClassWithMixin
    {
        private string title;

        [SetterInterceptor(typeof(PropertyChangedMixinInterceptor))]
        public virtual string Title { get => title; set => title = value; }

        public MyClassWithMixin()
        {

        }

        public virtual void UpdateTitle()
        {
            Title += "!";
        }
    }

    public interface IPropertyChangedMixin : INotifyPropertyChanged
    {
        void OnPropertyChanged(object target, string propertyName);
    }

    [Serializable]
    public class PropertyChangedMixin : IPropertyChangedMixin
    {
        public void OnPropertyChanged(object target, string propertyName)
        {
            PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropertyChangedMixinInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation) { }
        protected override void OnException(IInvocation invocation, Exception exception) { }

        protected override void OnExit(IInvocation invocation)
        {
            IPropertyChangedMixin mixin = invocation.Proxy as IPropertyChangedMixin;
            if (mixin != null)
            {
                string propertyName = invocation.Member.Name;
                Console.WriteLine($"[PropertyChangedMixinInterceptor] Notify changed:{propertyName}");
                mixin.OnPropertyChanged(invocation.Proxy, propertyName);
            }
        }
    }

    #endregion

    #region InterfaceProxy without target

    public interface IMyService
    {
        [MethodInterceptor(typeof(GetMessageInterceptor))]
        string GetMessage(string name);
    }

    public class GetMessageInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            // invocation.Proceed(); do not call Proceed

            invocation.ReturnValue = $"Hello {invocation.Parameters[0]}!";
        }
    }

    #endregion

    #region Constructor Injection

    public class MyService : IMyService
    {
        public string GetMessage(string name)
        {
            return $"Hello {name}!";
        }
    }

    public class MyClassWithInjections
    {
        private readonly IMyService myService;

        public MyClassWithInjections(IMyService myService)
        {
            this.myService = myService;
        }

        public void Method()
        {
            Console.WriteLine($"{myService.GetMessage("World")}");
        }
    }

    public class UnityConstructorInjectionResolver : IConstructorInjectionResolver
    {
        private readonly IUnityContainer container;

        public UnityConstructorInjectionResolver(IUnityContainer container)
        {
            if (container is null)
                throw new ArgumentNullException(nameof(container));

            this.container = container;
        }

        public object Resolve(ParameterInfo parameter)
        {
            if (parameter.Name == "p1")
                return "Ok";

            if (parameter.Name == "p2")
                return 10;

            return container.Resolve(parameter.ParameterType);
        }
    }
    #endregion

}
