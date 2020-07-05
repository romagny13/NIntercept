# NIntercept

> Allows to create **proxies** for class and interface, **intercept** properties, methods, events and **customize the code generated**.


Proxies

* **Class Proxy**: a **Proxy** that inherits from the **class** is created. **Virtual** members (properties, methods end events) are **overridden**. The **base class members** are **invoked** after interception.
* **Class Proxy with target**: The **target members** are **invoked** after interception.
* **Interface Proxy with target**: a **Proxy** that implements the **interface** is created. The **target members** are **invoked** after interception.
* **Interface Proxy without target**: interceptors are used to get and set the **return value**

Interception:

* **by attribute** (built-in or custom)
* **global interceptor**
* Implement **IInterceptor** or use **Interceptor** and **AsyncInterceptor** base classes


Invocation Members:

* **CallerMethod**: the method of the proxy
* **InterceptorProviderType**: IGetterInterceptorProvider, ISetterInterceptorProvider,IMethodInterceptorProvider ,IAddOnInterceptorProvider, IRemoveOnInterceptorProvider
* **Member** : property, method, event of the class to invoke after interception
* **Parameters**: the method parameters
* **Proxy**: the class or interface proxy
* **ReturnValue**: the result value
* **Target**: the target or null
* **GetAwaitableContext**: allows to wait before calling Proceed
* **GetParameter** allows to get typed parameter value
* **Proceed**: call next interceptor or invoke member

Options:

* **Mixins**: allows to **add features** to **proxy** created.
* **AdditionalCode** : allows to **customize the code generated**.
* **ClassProxyMemberSelector**: allows to **filter members to include** for **Class Proxy**. For example create an attribute and include only virtual members decorated with the attribute.
* **AdditionalTypeAttributes**: allows to add **custom attributes** on proxy generated.
* **ConstructorSelector**: allows to **select** the **base constructor** to call.

And

* **ConstructorInjectionResolver**: allows to **resolve constructor injections**.

Supported:

* _Generics_
* _Parameters ref and out_
* _Multi signatures_
* Interception on _private members_ for **ClassProxy**

## Install

[NuGet Package](https://www.nuget.org/packages/NIntercept/)

![Nuget](https://img.shields.io/nuget/v/nintercept.svg?style=for-the-badge)

```
Install-Package NIntercept
```

## Samples

### Class Proxy

> A **Proxy** that inherits from the **class** is created. **Virtual** members (properties, methods end events) are **overridden**. **Interceptors** allow to intercept these members. The **base class members** are **invoked** after interception.

**Sample 1** with global interceptor

Create a **class** with **virtual** members

```cs
public class MyClass
{
    public virtual string MyProperty { get; set; }

    public virtual void MyMethod()
    {

    }

    public virtual event EventHandler MyEvent;
}
```

Create an **interceptor**

```cs
public class LogInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine($"[LogInterceptor] Before {invocation.CallerMethod.Name}");

        invocation.Proceed();

        Console.WriteLine($"[LogInterceptor] After {invocation.CallerMethod.Name}");
    }
}
```

Create and use the **proxy**

```cs
var generator = new ProxyGenerator();
var proxy = generator.CreateClassProxy<MyClass>(new LogInterceptor());

proxy.MyProperty = "New Value"; // Setter

Console.WriteLine($"MyProperty: {proxy.MyProperty}"); // Getter

proxy.MyMethod(); // Method

EventHandler handler = null;
handler = (s, e) =>
{

};
proxy.MyEvent += handler; // AddOn

proxy.MyEvent -= handler; // RemoveOn
```

**Output**

```
[LogInterceptor] Before set_MyProperty
[LogInterceptor] After set_MyProperty
[LogInterceptor] Before get_MyProperty
[LogInterceptor] After get_MyProperty
MyProperty: New Value
[LogInterceptor] Before MyMethod
[LogInterceptor] After MyMethod
[LogInterceptor] Before add_MyEvent
[LogInterceptor] After add_MyEvent
[LogInterceptor] Before remove_MyEvent
[LogInterceptor] After remove_MyEvent
```

**Sample 2** : **with attributes** (Create some interceptors for each attribute)

```cs
[AllInterceptor(typeof(LogInterceptor))]
public class MyClass
{
    [GetterInterceptor(typeof(MyGetterInterceptor))]
    [SetterInterceptor(typeof(MySetterInterceptor))]
    public virtual string MyProperty { get; set; }

    [MethodInterceptor(typeof(MyMethodInterceptor))]
    public virtual void MyMethod()
    {

    }

    [AddOnInterceptor(typeof(MyAddOnInterceptor))]
    [AddOnInterceptor(typeof(MyRemoveOnInterceptor))]
    public virtual event EventHandler MyEvent;
}
```

```cs
var generator = new ProxyGenerator();
var proxy = generator.CreateClassProxy<MyClass>();

// etc.
```

| Interface | Target | Attribute |
| --- | --- | --- |
| IInterceptorProvider | class / interface | AllInterceptorAttribute
| IGetterInterceptorProvider | Property getter | GetterInterceptorAttribute
| ISetterInterceptorProvider | Property setter | SetterInterceptorAttribute
| IMethodInterceptorProvider | Method | MethodInterceptorAttribute
| IAddOnInterceptorProvider | Add event | AddOnInterceptorAttribute
| IRemoveOnInterceptorProvider | Remove event | RemoveOnInterceptorAttribute

**Sample 3**: Create a **custom interceptor attribute**


```cs
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class MyGetSetInterceptorAttribute : Attribute,
    IGetterInterceptorProvider,
    ISetterInterceptorProvider
{
    public Type InterceptorType => typeof(MyGetSetInterceptor);
}

public class MyGetSetInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine($"[MyGetSetInterceptor] Before {invocation.CallerMethod.Name}");

        invocation.Proceed();

        Console.WriteLine($"[MyGetSetInterceptor] After {invocation.CallerMethod.Name}");
    }
}
```

**Decorate** the property

```cs
public class MyClass
{
    [MyGetSetInterceptor]
    public virtual string MyProperty { get; set; }
}
```

**Output**

```
[MyGetSetInterceptor] Before set_MyProperty
[MyGetSetInterceptor] After set_MyProperty
[MyGetSetInterceptor] Before get_MyProperty
[MyGetSetInterceptor] After get_MyProperty
MyProperty: New Value
```

_Notes:_

* The interceptor allows to update parameters, the return value

**Sample**

```cs
public class MyInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        invocation.Parameters[0] = "New value";

        invocation.Proceed();

        invocation.ReturnValue = "New result";
    }
}
```

* We can use the `Interceptor` class (`Proceed` is automatically invoked)

```cs
public class MyInterceptor : Interceptor
{
    protected override void OnEnter(IInvocation invocation)
    {
        
    }

    protected override void OnException(IInvocation invocation, Exception exception)
    {
        
    }

    protected override void OnExit(IInvocation invocation)
    {
        
    }
}
```

### Class Proxy with target

> A **Proxy** that inherits from the **class** is created. **Virtual** members (properties, methods end events) are **overridden**. **Interceptors** allow to intercept these members. The **target members** are **invoked** after interception.

```cs
var target = new MyClass();
var proxy = generator.CreateClassProxyWithTarget<MyClass>(target, new MyInterceptor());
proxy.Method();
```

### Interface Proxy with target

> A **Proxy** that implements the **interface** is created. **Interceptors** allow to intercept these members. The **target members** are **invoked** after interception.


```cs
var target = new MyService();
var proxy = generator.CreateInterfaceProxyWithTarget<IMyService>(target, new MyInterceptor());
proxy.Method();
```

_Note: for Interface Proxy, add interceptor attributes on interface members_

### Interface Proxy without target

> A **Proxy** that implements the **interface** is created. The **interceptors** are used to get and set the **return value**.

```cs
public interface IMyService
{
    [MethodInterceptor(typeof(GetMessageInterceptor))]
    string GetMessage(string name);
}

public class GetMessageInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        // invocation.Proceed(); DO NOT CALL Proceed

        invocation.ReturnValue = $"Hello {invocation.Parameters[0]}!";
    }
}
```

```cs
var proxy = generator.CreateInterfaceProxyWithoutTarget<IMyService>();
var message = proxy.GetMessage("World");
Console.WriteLine(message); // write "Hello World!"
```

## Async interception 

**Sample 1** with `GetAwaitableContext`

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

**Sample 2** With `AsyncInterceptor` for async Method

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

## Constructor Injection Resolver

> Allows to resolve **constructor parameters** of the class for a **Class Proxy**.

**Sample** use **Unity Container** to resolve injection parameters

```cs
public interface IMyService
{
    string GetMessage(string name);
}

public class MyService : IMyService
{
    public string GetMessage(string name)
    {
        return $"Hello {name}!";
    }
}

public class MyClass
{
    private readonly IMyService myService;

    public MyClass(IMyService myService)
    {
        this.myService = myService;
    }

    public void Method()
    {
        Console.WriteLine($"{myService.GetMessage("World")}");
    }
}
```

Create the `Constructor Injection Resolver`

```cs
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
        return container.Resolve(parameter.ParameterType);
    }
}
```

Define the `options` and create the proxy

```cs
IUnityContainer container = new UnityContainer();
container.RegisterType<IMyService, MyService>();

var options = new ProxyGeneratorOptions();
options.ConstructorInjectionResolver = new UnityConstructorInjectionResolver(container);

var proxy = generator.CreateClassProxy<MyClass>(options);

proxy.Method();
```

_Note: By default the first constructor of the proxied class is selected. Create a custom ConstructorSelector (and set the option) to change this behavior._


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

... Or get the **same instance** of the **interceptor**. Usefull for Example with **InterfaceProxy without target**.


```cs
var container = new UnityContainer();

container.RegisterType<MyInterceptor>(new ContainerControlledLifetimeManager());

ObjectFactory.SetDefaultFactory(type => container.Resolve(type));
var generator = new ProxyGenerator();

var proxy = generator.CreateInterfaceProxyWithoutTarget<IMyClass>();

proxy.Title = "New Value";
var title = proxy.Title;
```

```cs
public class MyInterceptor : IInterceptor
{
    private string _title;

    public void Intercept(IInvocation invocation)
    {
        if (invocation.CallerMethod.Name.StartsWith("set_"))
            _title= invocation.GetParameter<string>(0); 
        else
            invocation.ReturnValue = _title;
    }
}

public interface IMyClass
{
    [GetSetTitle]
    string Title { get; set; }
}

public class GetSetTitleAttribute : Attribute, 
    IGetterInterceptorProvider, 
    ISetterInterceptorProvider
{
    public Type InterceptorType => typeof(MyInterceptor);
}
```

## Mixins

> Allows to add features to proxy generated.

**Sample** add **INotifyPropertyChanged** to Proxy created

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

Define the **Mixin**

```cs
public interface IPropertyChangedMixin : INotifyPropertyChanged
{
    void OnPropertyChanged(object target, string propertyName);
}

public class PropertyChangedMixin : IPropertyChangedMixin
{
    public void OnPropertyChanged(object target, string propertyName)
    {
        PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
```

Define an attribute and an **interceptor**

```cs
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
public class PropertyChangedAttribute : Attribute, ISetterInterceptorProvider
{
    public Type InterceptorType => typeof(PropertyChangedInterceptor);
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
        IPropertyChangedMixin mixin = invocation.Proxy as IPropertyChangedMixin;
        if (mixin != null)
        {
            string propertyName = invocation.Member.Name;
            mixin.OnPropertyChanged(invocation.Proxy, propertyName);
        }
    }
}
```

Define the **options**

```cs
var options = new ProxyGeneratorOptions();
options.AddMixinInstance(new PropertyChangedMixin());
var proxy = generator.CreateClassProxy<MainWindowViewModel>(options);
```

_Note: **caution** for **proxies** with **target**. We leave the proxy after calling a target member_


## Additional Code 

> Allows to **customize the code generated**. It's an "alternative" to Mixins. Require to write **il code** with [System.Reflection.Emit](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit?view=netcore-3.1).

Methods 

* **BeforeDefine**: called just after the TypeBuilder for the Proxy is defined.
* **AfterDefine**: called after all members defined.
* **BeforeInvoke**: called before invoke the method on the target or the base class.
* **AfterInvoke**: called after invoke the method

_Note: the **ProxyScope** allows to find field, property, method, event builders._

**Sample:**

```cs
public class MyAdditionalCode : AdditionalCode
{
    private static readonly MethodInfo WriteLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });

    public override void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
    {
        il.Emit(OpCodes.Ldstr, $"Before {callbackMethodDefinition.Method.Name}");
        il.Emit(OpCodes.Call, WriteLineMethod);
    }

    public override void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
    {
        il.Emit(OpCodes.Ldstr, $"After {callbackMethodDefinition.Method.Name}");
        il.Emit(OpCodes.Call, WriteLineMethod);
    }
}
```

Create a simple  **class**

```cs
public class MyClass
{
    public virtual void MyMethod()
    {
        Console.WriteLine("In My Method");
    }
}
```

Define the **options**

```cs
var generator = new ProxyGenerator();

var options = new ProxyGeneratorOptions();
options.AdditionalCode = new MyAdditionalCode();

var proxy = generator.CreateClassProxy<MyClass>(options);
proxy.MyMethod();
```

**Output**

```
Before MyMethod
In My Method
After MyMethod
```

Code Generated for the callback method:

```cs
public void MyMethod_Callback()
{
    Console.WriteLine("Before MyMethod");
    base.MyMethod();
    Console.WriteLine("After MyMethod");
}
```

**Advanced sample**:  Take a look at the **CodeGenerationSample**

_For example allows to implement INotifyPropertyChanged or create the commands for the ViewModel Proxy generated._

```cs
public class ViewModelAdditionalCode : AdditionalCode
{
    private NotifyPropertyChangedFeature notifyPropertyChangedFeature;
    private DelegateCommandBuilderFeature delegateCommandBuilderFeature;

    public ViewModelAdditionalCode()
    {
        notifyPropertyChangedFeature = new NotifyPropertyChangedFeature();
        delegateCommandBuilderFeature = new DelegateCommandBuilderFeature();
    }

    public override void BeforeDefine(ProxyScope proxyScope)
    {
        notifyPropertyChangedFeature.ImplementFeature(proxyScope);
        delegateCommandBuilderFeature.ImplementFeature(proxyScope);
    }

    public override void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
    {
        if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter)
        {
            // never called with clean proxy method builder
            notifyPropertyChangedFeature.CheckEquals(proxyScope, il, callbackMethodDefinition.Method);
        }
    }

    public override void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
    {
        if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter)
        {
            // never called with clean proxy method builder
            notifyPropertyChangedFeature.InvokeOnPropertyChanged(proxyScope, il, callbackMethodDefinition.Method);
        }
    }
}
```

_Another alternative is to create a **custom ProxyMethodBuilder** and change the **service provider** (ProxyGeneratorOptions). Update the proxy and implement INotifyPropertyChanged (add INotifyPropertyChanged interface, event and protected method to raise the event)  and call OnPropertyChanged method in properties._


## Save The Assembly (.NET Framework Only)

```cs
var generator = new ProxyGenerator(new ModuleScope(true)); // save
// with strong name
// var generator = new ProxyGenerator(new ModuleScope(true, true)); // save, strong name
// Or change the assembly and module names
// var generator = new ProxyGenerator(new ModuleScope("MyAssembly","MyModule",true, true)); 

var proxy = generator.CreateClassProxy<MyClass>();

// save the assembly
generator.ModuleScope.Save();
```

Recommendations: 

* Use this feature only for Debug
* Decompiler : [ILSpy](https://github.com/icsharpcode/ILSpy) (IL with C# comments, not fail to decompile unlike JustDecompile)

## Reflection 

* allows to access to private/public/static members (fields, properties, methods, constructors, events accessors)
* invoke usefull methods
* intercept get, set, add, remove and methods

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



