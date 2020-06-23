using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NIntercept.Definition;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Serialize_Proxy()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxy<MyClassSer>();

            proxy.Title = "The Title";
            proxy.MyItem = new MyItem { MyProperty = "A" };

            string json = JsonConvert.SerializeObject(proxy);

            Assert.AreEqual("{\"Title\":\"The Title\",\"MyItem\":{\"MyProperty\":\"A\"}}", json);
        }

        [TestMethod]
        public void Populate_Proxy()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxy<MyClassSer>();

            string json = "{\"Title\":\"The Title\",\"MyItem\":{\"MyProperty\":\"A\"}}";

            JsonConvert.PopulateObject(json, proxy);

            Assert.AreEqual("The Title", proxy.Title);
            Assert.AreEqual("A", proxy.MyItem.MyProperty);
        }
    }

    public class MyClassSer
    {
        public virtual string Title { get; set; }

        public MyItem MyItem { get; set; }

        public MyClassSer()
        {

        }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public class ProxyGeneratorContext
    {
        private ModuleScope moduleScope;
        private IProxyBuilder proxyTypeBuilder;
        private IProxyPropertyBuilder proxyPropertyBuilder;
        private IProxyMethodBuilder proxyMethodBuilder;
        private IProxyEventBuilder proxyEventBuilder;
        private ICallbackMethodBuilder callbackMethodBuilder;
        private IInvocationTypeBuilder invocationTypeBuilder;

        public ModuleScope ModuleScope
        {
            get { return moduleScope; }
            set { moduleScope = value; }
        }

        public IProxyBuilder ProxyTypeBuilder
        {
            get { return proxyTypeBuilder; }
            set { proxyTypeBuilder = value; }
        }

        public IProxyPropertyBuilder ProxyPropertyBuilder
        {
            get { return proxyPropertyBuilder; }
            set { proxyPropertyBuilder = value; }
        }

        public IProxyMethodBuilder ProxyMethodBuilder
        {
            get { return proxyMethodBuilder; }
            set { proxyMethodBuilder = value; }
        }

        public IProxyEventBuilder ProxyEventBuilder
        {
            get { return proxyEventBuilder; }
            set { proxyEventBuilder = value; }
        }

        public ICallbackMethodBuilder CallbackMethodBuilder
        {
            get { return callbackMethodBuilder; }
            set { callbackMethodBuilder = value; }
        }

        public IInvocationTypeBuilder InvocationTypeBuilder
        {
            get { return invocationTypeBuilder; }
            set { invocationTypeBuilder = value; }
        }

        // factory ?

        // selector ?

        public void Initialize()
        {

        }

        public Type CreateProxyType(ProxyTypeDefinition typeDefinition, ProxyGeneratorOptions options, IInterceptor[] interceptors)
        {
            Type buildType = moduleScope.ProxyTypeRegistry.GetBuidType(typeDefinition.FullName, options);
            if (buildType == null)
            {
                TypeBuilder typeBuilder = ProxyTypeBuilder.CreateType(typeDefinition, interceptors);
                buildType = typeBuilder.BuildType();
                proxyTypeBuilder.ModuleScope.ProxyTypeRegistry.Add(typeDefinition.FullName, options, buildType);
            }
            return buildType;
        }

        public PropertyBuilder CreateProperty(TypeBuilder proxyTypeBuilder, PropertyDefinition propertyDefinition, FieldBuilder[] fields)
        {
            return ProxyPropertyBuilder.CreateProperty(moduleScope, proxyTypeBuilder, propertyDefinition, fields);
        }

        public MethodBuilder CreateMethod(TypeBuilder proxyTypeBuilder, MethodDefinition methodDefinition, MemberInfo member, FieldBuilder[] fields)
        {
            return ProxyMethodBuilder.CreateMethod(moduleScope, proxyTypeBuilder, methodDefinition, member, fields);
        }

        public EventBuilder CreateEvent(TypeBuilder proxyTypeBuilder, EventDefinition eventDefinition, FieldBuilder[] fields)
        {
            return ProxyEventBuilder.CreateEvent(moduleScope, proxyTypeBuilder, eventDefinition, fields);
        }

        public MethodBuilder CreateCallbackMethod(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, MethodDefinition methodDefinition, FieldBuilder[] fields)
        {
            return CallbackMethodBuilder.CreateMethod(moduleBuilder, typeBuilder, methodDefinition.MethodCallbackDefinition, fields);
        }

        protected Type CreateInvocationType(ModuleScope moduleScope, TypeBuilder proxyTypeBuilder, MethodDefinition methodDefinition, ModuleBuilder moduleBuilder, MethodBuilder callbackMethodBuilder)
        {
            Type invocationType = moduleScope.InvocationTypeRegistry.GetBuildType(methodDefinition.InvocationTypeDefinition.Name);
            if (invocationType == null)
            {
                invocationType = InvocationTypeBuilder.CreateType(moduleBuilder, proxyTypeBuilder, methodDefinition.InvocationTypeDefinition, callbackMethodBuilder);
                moduleScope.InvocationTypeRegistry.Add(methodDefinition.InvocationTypeDefinition.Name, invocationType);
            }
            return invocationType;
        }
    }
}
