# NIntercept
 
> Allows to create proxies and intercept property, indexer, method and event calls. 


Methods:

* **CreateClassProxy** (creates a class that inherits from the base class, overrides **virtual** methods and call base methods)
* **CreateClassProxyWithTarget**
* **CreateInterfaceProxyWithTarget** (creates a class that implements the **interface** and calls target methods)

Supported:

* _Generics_
* _Parameters ref and out_
* _Multi signatures_
* Interception on _private members_ for **CreateClassProxy**

## Samples

### CreateClassProxy

```cs
class Program
{
    static void Main(string[] args)
    {
        var generator = new ProxyGenerator(new PersistentProxyBuilder());

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

        Console.ReadKey();
    }
}

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

public class LogInterceptor : Interceptor
{
    protected override void OnEnter(IInvocation invocation)
    {
        Console.WriteLine($"[LogInterceptor] Enter '{invocation.Member.Name}'");
    }

    protected override void OnException(IInvocation invocation, Exception exception)
    {
        Console.WriteLine($"[LogInterceptor] Exception '{invocation.Member.Name}', Result: '{invocation.ReturnValue}'");
    }

    protected override void OnExit(IInvocation invocation)
    {
        Console.WriteLine($"[LogInterceptor] Exit '{invocation.Member.Name}', Result: '{invocation.ReturnValue}'");
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
```

Output

```
[LogInterceptor] Enter 'MyProperty'
[PropertyChangedInterceptor] Notify property changed 'MyProperty'
[LogInterceptor] Exit 'MyProperty', Result: ''
[LogInterceptor] Enter 'MyProperty'
[LogInterceptor] Exit 'MyProperty', Result: 'New value'
Property: 'New value'
[LogInterceptor] Enter 'MyMethod'
[LogInterceptor] Exit 'MyMethod', Result: ''
[LogInterceptor] Enter 'MyProperty'
[LogInterceptor] Exit 'MyProperty', Result: 'B'
Property: 'B'
[LogInterceptor] Enter 'MyEvent'
[EventInterceptor] Enter add new subscriber 'MyEvent'
[EventInterceptor] Exit add new subscriber 'MyEvent'
[LogInterceptor] Exit 'MyEvent', Result: ''
[LogInterceptor] Enter 'MyEvent'
[LogInterceptor] Exit 'MyEvent', Result: ''
```

### CreateClassProxyWithTarget

```cs
var target = new MyClass();
var proxy = generator.CreateClassProxyWithTarget<MyClass>(target, new MyInterceptor());
proxy.MethodA("My value");
```

### CreateInterfaceProxyWithTarget

```cs
var target = new MyService();
var proxy = generator.CreateInterfaceProxyWithTarget<IMyService>(target, new MyInterceptor());
proxy.MethodA("My value");
```

## Interceptors

Create a class that implements **IInterceptor** interface or use **Interceptor** / **AsyncInterceptor** base classes.


## Attributes

2 ways to define interceptors 

```cs
// global for type
var proxy = generator.CreateClassProxy<MyClass>(new MyInterceptor(), new MyInterceptor2());
```

... Or with **attributes**

| Interface | Target | Attribute |
| --- | --- | --- |
| IInterceptorProvider | class / interface | AllInterceptorAttribute
| IPropertyGetInterceptorProvider | Property getter | PropertyGetInterceptorAttribute
| IPropertySetInterceptorProvider | Property setter | PropertySetInterceptorAttribute
| IMethodInterceptorProvider | Method | MethodInterceptorAttribute
| IAddEventInterceptorProvider | Add event | AddEventInterceptorAttribute
| IRemoveEventInterceptorProvider | Remove event |RemoveEventInterceptorAttribute

_Create a custom attribute for multiple targets_

```cs
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
public class MultiTargetInterceptorAttribute : Attribute, 
    IPropertyGetInterceptorProvider, 
    IPropertySetInterceptorProvider,
    IMethodInterceptorProvider
{
    public Type InterceptorType
    {
        get { return typeof(LogInterceptor); }
    }
}

public class MyClass
{
    [MultiTargetInterceptor]
    public virtual string MyProperty {get; set;}

    [MultiTargetInterceptor]
    public virtual void MyMethod(string value)
    {

    }
}
```

Note: for interfaces, add the the attributes on interface members.

## Async interception 

```cs
public class MyAsyncInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var context = invocation.GetAwaitableContext(); // here

        var viewModel = invocation.Proxy as MainWindowViewModel;
        if (viewModel != null)
            viewModel.IsBusy = true;

        // async await void best practices
        Task.Delay(3000).Await(() =>
        {
            if (viewModel != null)
                viewModel.IsBusy = false;

            context.Proceed(); // here

        }, ex => { });
    }
}
```

With async method, use **AsyncInterceptor**

```cs
public class MyClass
{
    public virtual async Task Method()
    {
        Console.WriteLine("Enter async Method");
        await Task.Delay(2000);
        Console.WriteLine("Exit async Method");
    }
}

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
        Console.WriteLine($"[MyAsyncInterceptor] Exit '{invocation.Member.Name}'");
    }
}
```

## Mixin

Example add INotifyPropertyChanged to Proxy created

```cs
public class MainWindowViewModel
{
    private DelegateCommand updateTitleCommand;

    public MainWindowViewModel()
    {
        Title = "Main title";
    }

    [PropertyChanged]
    public virtual string Title { get; set; }

    public DelegateCommand UpdateTitleCommand
    {
        get
        {
            if (updateTitleCommand == null)
                updateTitleCommand = new DelegateCommand(ExecuteUpdateTitleCommand);
            return updateTitleCommand;
        }
    }

    protected virtual void ExecuteUpdateTitleCommand()
    {
        Title += "!";
    }
}
```

```cs
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class PropertyChangedAttribute : Attribute, IPropertySetInterceptorProvider
{
    public Type InterceptorType => typeof(PropertyChangedInterceptor);
}

public interface IPropertyChangedNotifier : INotifyPropertyChanged
{
    void OnPropertyChanged(object target, string propertyName);
}

[Serializable]
public class PropertyChangedNotifier : IPropertyChangedNotifier
{
    public void OnPropertyChanged(object target, string propertyName)
    {
        PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class PropertyChangedInterceptor : Interceptor
{
    protected override void OnEnter(IInvocation invocation) { }
    protected override void OnException(IInvocation invocation, Exception exception)
    {
        Console.WriteLine($"Error: {exception.Message}");
    }

    protected override void OnExit(IInvocation invocation)
    {
        if (!invocation.CallerMethod.Name.StartsWith("set_"))
            return;

        IPropertyChangedNotifier propertyChangedNotifier = invocation.Proxy as IPropertyChangedNotifier;
        if (propertyChangedNotifier != null)
        {
            string propertyName = invocation.Member.Name;
            propertyChangedNotifier.OnPropertyChanged(invocation.Proxy, propertyName);
        }
    }
}
```

Define the options

```cs
var options = new ProxyGeneratorOptions();
options.AddMixinInstance(new PropertyChangedNotifier());
generator.CreateClassProxy<MainWindowViewModel>(options);
```

## ObjectFactory

Change the default **factory** with an **IoC Container**

```cs
var generator = new ProxyGenerator();

// Example with Unity Container
IUnityContainer container = new UnityContainer();

// here
ObjectFactory.SetDefaultFactory(type => container.Resolve(type));

container.RegisterType<IService1, Service1>();
container.RegisterType<IService2, Service2>();

etc.
```

... Allows to inject dependencies in interceptor


```cs
public class MyInterceptor : IInterceptor
{
    public MyInterceptor(IService1 service1, IService2 service2)
    {
        Service1 = service1;
        Service2 = service2;
    }

    public IService1 Service1 { get; }
    public IService2 Service2 { get; }

    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();
    }
}
```

## Save The Assembly (.NET Framework Only)

```cs
var generator = new ProxyGenerator(new PersistentProxyBuilder());

// or with Default ProxyBuilder
// var generator = new ProxyGenerator(new ProxyBuilder(new ModuleScope(true)));
// Or
// var generator = new ProxyGenerator(new ProxyBuilder(new ModuleScope("MyAssembly","MyModule",true))); 
// with strong name key
// var generator = new ProxyGenerator(new ProxyBuilder(new ModuleScope("MyAssembly","MyModule",true, true))); 
var proxy = generator.CreateClassProxy<MyClass>(new MyInterceptor());

generator.ProxyBuilder.ModuleScope.Save();
```

## Reflection 

> Extension methods for PropertyInfo, EventInfo and MethodInfo.

To use extension methods:

```cs
using System.Reflection.Interception;
```

Property

```cs
// private, static property
var target = new MyClass();
var property = typeof(MyClass).GetProperty("MyProperty", BindingFlags.NonPublic | BindingFlags.Instance);
property.InterceptSet(target, new object[] { "A" }, new LogInterceptor());
var value = property.InterceptGet<string>(target, new object[0], new LogInterceptor());
```

Indexer

```cs
var collection = new MyCollection(); // Collection of strings
var property = typeof(MyCollection).GetProperty("Item");
// set
property.InterceptSet(collection, new object[] { 0, "A" }, new LogInterceptor()); // index 0 => set value "A"
// get
var result = property.InterceptGet(collection, new object[] { 0 }, new LogInterceptor()); // get value at index 0
```

Method

```cs
// private, static method
var target = new MyClass();
var method = typeof(MyClass).GetMethod("MyMethod", BindingFlags.NonPublic | BindingFlags.Instance);
method.Intercept(target, new object[] { "A" }, new LogInterceptor());
```

Event

```cs
// private, static event
var target = new MyClass();
var @event = typeof(MyClass).GetEvent("MyEvent", BindingFlags.NonPublic | BindingFlags.Instance);
EventHandler ev = (s, e) =>
{
   // ...
};
@event.InterceptAdd(target, new object[] { ev }, new LogInterceptor());
@event.InterceptRemove(target, new object[] { ev }, new LogInterceptor());
```

Can be used with **TypeAccessor** 

* allows to access to private/public/static members (fields, properties, methods, constructors, events accessors)
* invoke usefull methods

Sample:

```cs
var target = new MyClass();
var accessor = new TypeAccessor(target);
accessor.Properties["MyProperty"].InterceptSet(new object[] { "New Value" }, new LogInterceptor());
```

## Support

* .NET Framework 4.5 and 4.7.2
* .NET Core 3.1
* .NET Standard 2.0