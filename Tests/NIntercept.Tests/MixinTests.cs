using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NIntercept.Tests
{
    [TestClass]
    public class MixinTests
    {
        [TestInitialize()]
        public void Startup()
        {
            MyClass.States.Clear();
            MyClass_b.States.Clear();
        }

        [TestMethod]
        public void Mixin_With_ClassProxy()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.AddMixinInstance(new PropertyChangedNotifier());

            var proxy = generator.CreateClassProxy<MyClass>(options);

            Assert.AreEqual(0, MyClass.States.Count);

            proxy.Title = "New title";

            Assert.AreEqual(1, MyClass.States.Count);
            Assert.AreEqual(StateTypes.Notify, MyClass.States[0]);

            MyClass.States.Clear();

            proxy.UpdateTitle();

            Assert.AreEqual(1, MyClass.States.Count);
            Assert.AreEqual(StateTypes.Notify, MyClass.States[0]);
        }

        [TestMethod]
        public void Mixin_With_ClassProxy_With_Target()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.AddMixinInstance(new PropertyChangedNotifier());

            var target = new MyClass();

            var proxy = generator.CreateClassProxyWithTarget<MyClass>(target, options);

            Assert.AreEqual(0, MyClass.States.Count);

            proxy.Title = "New title";

            Assert.AreEqual(1, MyClass.States.Count);
            Assert.AreEqual(StateTypes.Notify, MyClass.States[0]);

            MyClass.States.Clear();

            proxy.UpdateTitle(); // target.Title => out of the Proxy, cannot intercept

            Assert.AreEqual(0, MyClass.States.Count);
        }

        [TestMethod]
        public void Mixin_With_InterfaceProxy_With_Target()
        {
            var generator = new ProxyGenerator();

            var options = new ProxyGeneratorOptions();
            options.AddMixinInstance(new PropertyChangedNotifier());

            var target = new MyClass_b();

            var proxy = generator.CreateInterfaceProxyWithTarget<IMyClass_b>(target, options);

            Assert.AreEqual(0, MyClass_b.States.Count);

            proxy.Title = "New title";

            Assert.AreEqual(1, MyClass_b.States.Count);
            Assert.AreEqual(StateTypes.Notify, MyClass_b.States[0]);

            MyClass_b.States.Clear();

            proxy.UpdateTitle(); // target.Title => out of the Proxy, cannot intercept

            Assert.AreEqual(0, MyClass_b.States.Count);
        }
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

    public class PropertyChangedNotifierInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation) { }
        protected override void OnException(IInvocation invocation, Exception exception) { }

        protected override void OnExit(IInvocation invocation)
        {
            IPropertyChangedNotifier propertyChangedNotifier = invocation.Proxy as IPropertyChangedNotifier;
            if (propertyChangedNotifier != null)
            {
                string propertyName = invocation.Member.Name;
                MyClass.States.Add(StateTypes.Notify);
                propertyChangedNotifier.OnPropertyChanged(invocation.Proxy, propertyName);
            }
        }
    }

    public class PropertyChangedNotifierInterceptor_b : Interceptor
    {
        protected override void OnEnter(IInvocation invocation) { }
        protected override void OnException(IInvocation invocation, Exception exception) { }

        protected override void OnExit(IInvocation invocation)
        {
            IPropertyChangedNotifier propertyChangedNotifier = invocation.Proxy as IPropertyChangedNotifier;
            if (propertyChangedNotifier != null)
            {
                string propertyName = invocation.Member.Name;
                MyClass_b.States.Add(StateTypes.Notify);
                propertyChangedNotifier.OnPropertyChanged(invocation.Proxy, propertyName);
            }
        }
    }

    public class MyClass
    {
        private string title;

        public static List<StateTypes> States = new List<StateTypes>();

        [PropertySetInterceptor(typeof(PropertyChangedNotifierInterceptor))]
        public virtual string Title { get => title; set => title = value; }

        public MyClass()
        {

        }

        public virtual void UpdateTitle()
        {
            Title += "!";
        }
    }

    public interface IMyClass_b
    {
        [PropertySetInterceptor(typeof(PropertyChangedNotifierInterceptor_b))]
        string Title { get; set; }

        void UpdateTitle();
    }

    public class MyClass_b: IMyClass_b
    {
        private string title;

        public static List<StateTypes> States = new List<StateTypes>();

        public virtual string Title { get => title; set => title = value; }

        public MyClass_b()
        {

        }

        public virtual void UpdateTitle()
        {
            Title += "!";
        }
    }
}
