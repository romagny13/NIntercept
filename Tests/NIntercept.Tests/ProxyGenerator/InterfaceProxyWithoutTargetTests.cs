using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace NIntercept.Tests
{
    [TestClass]
    public class InterfaceProxyWithoutTargetTests
    {
        [TestMethod]
        public void Works()
        {
            var container = new UnityContainer();

            container.RegisterType<IntTitle>(new ContainerControlledLifetimeManager());
            container.RegisterType<IntMyEvent>(new ContainerControlledLifetimeManager());

            ObjectFactory.SetDefaultFactory(type => container.Resolve(type));
            
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<ITypeW1>();

            // property
            proxy.Title = "New Value";
            var title = proxy.Title;

            Assert.AreEqual("New Value", title);

            // method

            Assert.AreEqual("Ok", proxy.GetMessage());

            // event
            bool isCalled = false;
            EventHandler ev = null;
            ev = (s, e) =>
            {
                isCalled = true;
            };

            proxy.MyEvent += ev;

            Assert.AreEqual(true, IntMyEvent.IsAdd);

            proxy.MyEvent -= ev;

            Assert.AreEqual(true, IntMyEvent.IsRemove);
        }
    }

    public interface ITypeW1
    {
        [GetSetTitle]
        string Title { get; set; }

        [MethodInterceptor(typeof(GetMessageInterceptor))]
        string GetMessage();

        [AddRemoveMyEvent]
        event EventHandler MyEvent;
    }

    public class GetSetTitleAttribute : Attribute, IPropertyGetInterceptorProvider, IPropertySetInterceptorProvider
    {
        public Type InterceptorType => typeof(IntTitle);

    }

    public class AddRemoveMyEventAttribute : Attribute, IAddEventInterceptorProvider, IRemoveEventInterceptorProvider
    {
        public Type InterceptorType => typeof(IntMyEvent);

    }

    public class IntTitle : IInterceptor
    {
        private string _title;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.CallerMethod.Name.StartsWith("set_"))
            {
                _title= invocation.GetParameter<string>(0); 
            }
            else
            {
                invocation.ReturnValue = _title;
            }
        }
    }

    public class GetMessageInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = "Ok";
        }
    }

    public class IntMyEvent : IInterceptor
    {
        public static bool IsAdd { get;  set; }
        public static bool IsRemove { get; set; }

        private event EventHandler _myEvent;

        public void Intercept(IInvocation invocation)
        {
            if (invocation.CallerMethod.Name.StartsWith("add_"))
            {
                IsAdd = true;
                _myEvent += invocation.GetParameter<EventHandler>(0);
            }
            else
            {
                IsRemove = true;
                _myEvent -= invocation.GetParameter<EventHandler>(0);
            }
        }
    }
}
