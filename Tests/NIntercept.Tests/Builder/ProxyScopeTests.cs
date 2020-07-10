using Microsoft.VisualStudio.TestTools.UnitTesting;
using NIntercept.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NIntercept.Tests.Builder
{
    [TestClass]
    public class ProxyScopeTests
    {
        [TestMethod]
        public void Set_Properties_Methods_Events_Correctly()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("MyProperty", proxyScope.Properties[0].Name);

            Assert.AreEqual(10, proxyScope.Methods.Count); // get set meth, add, remove
            Assert.AreEqual("get_MyProperty_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[1].Name);
            Assert.AreEqual("set_MyProperty_Callback", proxyScope.Methods[2].Name);
            Assert.AreEqual("set_MyProperty", proxyScope.Methods[3].Name);
            Assert.AreEqual("Method_Callback", proxyScope.Methods[4].Name);
            Assert.AreEqual("Method", proxyScope.Methods[5].Name);
            Assert.AreEqual("add_MyEvent_Callback", proxyScope.Methods[6].Name);
            Assert.AreEqual("add_MyEvent", proxyScope.Methods[7].Name);
            Assert.AreEqual("remove_MyEvent_Callback", proxyScope.Methods[8].Name);
            Assert.AreEqual("remove_MyEvent", proxyScope.Methods[9].Name);

            Assert.AreEqual(1, proxyScope.Events.Count);
            Assert.AreEqual("MyEvent", proxyScope.Events[0].GetName());
        }

        [TestMethod]
        public void Set_Fields_Correctly()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(1, proxyScope.ConstructorFields.Length);
            Assert.AreEqual("__interceptors", proxyScope.ConstructorFields[0].Name);

            Assert.AreEqual(9, proxyScope.Fields.Count);
            Assert.AreEqual("__interceptors", proxyScope.Fields[0].Name);
            Assert.AreEqual("TypeScope1_MyProperty", proxyScope.Fields[1].Name);
            Assert.AreEqual("TypeScope1_Proxy_get_MyProperty", proxyScope.Fields[2].Name);
            Assert.AreEqual("TypeScope1_Proxy_set_MyProperty", proxyScope.Fields[3].Name);

            Assert.AreEqual("TypeScope1_Method", proxyScope.Fields[4].Name);
            Assert.AreEqual("TypeScope1_Proxy_Method", proxyScope.Fields[5].Name);

            Assert.AreEqual("TypeScope1_MyEvent", proxyScope.Fields[6].Name);
            Assert.AreEqual("TypeScope1_Proxy_add_MyEvent", proxyScope.Fields[7].Name);
            Assert.AreEqual("TypeScope1_Proxy_remove_MyEvent", proxyScope.Fields[8].Name);
        }

        [TestMethod]
        public void DefineField_Adds_FieldBuilder()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(1, proxyScope.ConstructorFields.Length);
            Assert.AreEqual("__interceptors", proxyScope.ConstructorFields[0].Name);
            Assert.AreEqual(1, proxyScope.Fields.Count);
            Assert.AreEqual("__interceptors", proxyScope.Fields[0].Name);

            var field = proxyScope.DefineField("A", typeof(string), FieldAttributes.Private);
            Assert.AreEqual(1, proxyScope.ConstructorFields.Length);
            Assert.AreEqual(2, proxyScope.Fields.Count);
            Assert.AreEqual("A", proxyScope.Fields[1].Name);
            Assert.AreEqual(field, proxyScope.Fields[1]);
        }

        [TestMethod]
        public void DefineProperty_Adds_PropertyBuilder()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Properties.Count);

            var property = proxyScope.DefineProperty("P", PropertyAttributes.None, typeof(string), new Type[] { typeof(string) });
            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("P", proxyScope.Properties[0].Name);
            Assert.AreEqual(property, proxyScope.Properties[0]);
        }

        [TestMethod]
        public void DefineFullProperty_Adds_Methods_And_Property()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Properties.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            FieldBuilder field = proxyScope.DefineField("_myProperty", typeof(EventHandler), FieldAttributes.Private);

            var p = proxyScope.DefineFullProperty("MyProperty", PropertyAttributes.None, typeof(void), new Type[0], MethodAttributes.Public, field);
            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("MyProperty", proxyScope.Properties[0].Name);

            Assert.AreEqual(2, proxyScope.Methods.Count);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[0].Name);
            Assert.AreEqual("set_MyProperty", proxyScope.Methods[1].Name);
        }

        [TestMethod]
        public void DefineReadOnlyProperty_Adds_Methods_And_Property()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Properties.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            FieldBuilder field = proxyScope.DefineField("_myProperty", typeof(EventHandler), FieldAttributes.Private);

            var p = proxyScope.DefineReadOnlyProperty("MyProperty", PropertyAttributes.None, typeof(void), new Type[0], MethodAttributes.Public, field);
            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("MyProperty", proxyScope.Properties[0].Name);

            Assert.AreEqual(1, proxyScope.Methods.Count);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[0].Name);
        }

        [TestMethod]
        public void DefineMethod_Adds_MethodBuilder()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Methods.Count);

            var m = proxyScope.DefineMethod("M", MethodAttributes.Public, typeof(string), new Type[] { typeof(string) });
            Assert.AreEqual(1, proxyScope.Methods.Count);
            Assert.AreEqual("M", proxyScope.Methods[0].Name);

            var m2 = proxyScope.DefineMethod("M2", MethodAttributes.Public);
            Assert.AreEqual(2, proxyScope.Methods.Count);
            Assert.AreEqual("M2", proxyScope.Methods[1].Name);
        }

        [TestMethod]
        public void DefineEvent_Adds_EventBuilder()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Events.Count);

            var m = proxyScope.DefineEvent("E", EventAttributes.None, typeof(EventHandler));
            Assert.AreEqual(1, proxyScope.Events.Count);
            Assert.AreEqual("E", proxyScope.Events[0].GetName());
        }

        [TestMethod]
        public void DefineFullEvent_Adds_Methods_And_Event()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(0, proxyScope.Events.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            FieldBuilder field = proxyScope.DefineField("_myEvent", typeof(EventHandler), FieldAttributes.Private);

            var m = proxyScope.DefineFullEvent("MyEvent", EventAttributes.None, typeof(EventHandler), field);
            Assert.AreEqual(1, proxyScope.Events.Count);
            Assert.AreEqual("MyEvent", proxyScope.Events[0].GetName());

            Assert.AreEqual(2, proxyScope.Methods.Count);
            Assert.AreEqual("add_MyEvent", proxyScope.Methods[0].Name);
            Assert.AreEqual("remove_MyEvent", proxyScope.Methods[1].Name);
        }

        [TestMethod]
        public void CreateInterceptableProperty_Adds_Fields_Property_Methods_And_Mapping()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            //  proxyScope.DefineTypeAndMembers();

            proxyScope.GetType().GetMethod("CreateFields", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(proxyScope, new object[0]);

            Assert.AreEqual(1, proxyScope.Fields.Count);
            Assert.AreEqual(0, proxyScope.Properties.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            var propertyBuilder = proxyScope.CreateInterceptableProperty(typeDefinition.PropertyDefinitions[0]);

            Assert.AreEqual(4, proxyScope.Fields.Count);
            Assert.AreEqual("TypeScope1_MyProperty", proxyScope.Fields[1].Name);
            Assert.AreEqual("TypeScope1_Proxy_get_MyProperty", proxyScope.Fields[2].Name);
            Assert.AreEqual("TypeScope1_Proxy_set_MyProperty", proxyScope.Fields[3].Name);

            Assert.AreEqual(4, proxyScope.Methods.Count);
            Assert.AreEqual("get_MyProperty_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[1].Name);
            Assert.AreEqual("set_MyProperty_Callback", proxyScope.Methods[2].Name);
            Assert.AreEqual("set_MyProperty", proxyScope.Methods[3].Name);

            var acc = new TypeAccessor(proxyScope);

            var mappings = acc.Fields["propertyMappings"].GetValue() as List<PropertyMapping>;

            Assert.AreEqual(1, mappings.Count);
            Assert.AreEqual("TypeScope1_MyProperty", mappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_get_MyProperty", mappings[0].GetMethodField.Name);
            Assert.AreEqual("TypeScope1_Proxy_set_MyProperty", mappings[0].SetMethodField.Name);

            Assert.AreEqual("MyProperty", mappings[0].Property.Name);
            Assert.AreEqual("get_MyProperty", mappings[0].GetMethodBuilder.Name);
            Assert.AreEqual("set_MyProperty", mappings[0].SetMethodBuilder.Name);
        }

        [TestMethod]
        public void CreateInterceptableMethod_Adds_Fields_Methods_And_Mapping()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            //  proxyScope.DefineTypeAndMembers();

            proxyScope.GetType().GetMethod("CreateFields", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(proxyScope, new object[0]);

            Assert.AreEqual(1, proxyScope.Fields.Count);
            Assert.AreEqual(0, proxyScope.Properties.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            var m = proxyScope.CreateInterceptableMethod(typeDefinition.MethodDefinitions[0], typeDefinition.MethodDefinitions[0].Method);

            Assert.AreEqual(3, proxyScope.Fields.Count);
            Assert.AreEqual("TypeScope1_Method", proxyScope.Fields[1].Name);
            Assert.AreEqual("TypeScope1_Proxy_Method", proxyScope.Fields[2].Name);

            Assert.AreEqual(2, proxyScope.Methods.Count);
            Assert.AreEqual("Method_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("Method", proxyScope.Methods[1].Name);

            var acc = new TypeAccessor(proxyScope);

            var mappings = acc.Fields["methodMappings"].GetValue() as List<MethodMapping>;

            Assert.AreEqual(1, mappings.Count);
            Assert.AreEqual("TypeScope1_Method", mappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_Method", mappings[0].MethodField.Name);

            Assert.AreEqual("Method", mappings[0].Method.Name);
            Assert.AreEqual("Method", mappings[0].MethodBuilder.Name);
        }


        [TestMethod]
        public void CreateInterceptableEvent_Adds_Fields_Event_Methods_And_Mapping()
        {
            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, null);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            //  proxyScope.DefineTypeAndMembers();

            proxyScope.GetType().GetMethod("CreateFields", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(proxyScope, new object[0]);

            Assert.AreEqual(1, proxyScope.Fields.Count);
            Assert.AreEqual(0, proxyScope.Properties.Count);
            Assert.AreEqual(0, proxyScope.Methods.Count);

            var er = proxyScope.CreateInterceptableEvent(typeDefinition.EventDefinitions[0]);

            Assert.AreEqual(4, proxyScope.Fields.Count);
            Assert.AreEqual("TypeScope1_MyEvent", proxyScope.Fields[1].Name);
            Assert.AreEqual("TypeScope1_Proxy_add_MyEvent", proxyScope.Fields[2].Name);
            Assert.AreEqual("TypeScope1_Proxy_remove_MyEvent", proxyScope.Fields[3].Name);

            Assert.AreEqual(4, proxyScope.Methods.Count);
            Assert.AreEqual("add_MyEvent_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("add_MyEvent", proxyScope.Methods[1].Name);
            Assert.AreEqual("remove_MyEvent_Callback", proxyScope.Methods[2].Name);
            Assert.AreEqual("remove_MyEvent", proxyScope.Methods[3].Name);

            var acc = new TypeAccessor(proxyScope);

            var mappings = acc.Fields["eventMappings"].GetValue() as List<EventMapping>;

            Assert.AreEqual(1, mappings.Count);
            Assert.AreEqual("TypeScope1_MyEvent", mappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_add_MyEvent", mappings[0].AddMethodField.Name);
            Assert.AreEqual("TypeScope1_Proxy_remove_MyEvent", mappings[0].RemoveMethodField.Name);

            Assert.AreEqual("MyEvent", mappings[0].Event.Name);
            Assert.AreEqual("add_MyEvent", mappings[0].AddMethodBuilder.Name);
            Assert.AreEqual("remove_MyEvent", mappings[0].RemoveMethodBuilder.Name);
        }

        [TestMethod]
        public void MixinTest()
        {
            var o = new ProxyGeneratorOptions();
            o.AddMixinInstance(new MyMixin());

            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(EmpyType), null, o);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("MyProperty", proxyScope.Properties[0].Name);

            Assert.AreEqual(10, proxyScope.Methods.Count); // get set meth, add, remove
            Assert.AreEqual("get_MyProperty_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[1].Name);
            Assert.AreEqual("set_MyProperty_Callback", proxyScope.Methods[2].Name);
            Assert.AreEqual("set_MyProperty", proxyScope.Methods[3].Name);
            Assert.AreEqual("Method_Callback", proxyScope.Methods[4].Name);
            Assert.AreEqual("Method", proxyScope.Methods[5].Name);
            Assert.AreEqual("add_MyEvent_Callback", proxyScope.Methods[6].Name);
            Assert.AreEqual("add_MyEvent", proxyScope.Methods[7].Name);
            Assert.AreEqual("remove_MyEvent_Callback", proxyScope.Methods[8].Name);
            Assert.AreEqual("remove_MyEvent", proxyScope.Methods[9].Name);

            Assert.AreEqual(1, proxyScope.Events.Count);
            Assert.AreEqual("MyEvent", proxyScope.Events[0].GetName());

            // fields
            Assert.AreEqual(2, proxyScope.ConstructorFields.Length);
            Assert.AreEqual("__interceptors", proxyScope.ConstructorFields[0].Name);
            Assert.AreEqual("__myMixin", proxyScope.ConstructorFields[1].Name);

            Assert.AreEqual(10, proxyScope.Fields.Count);
            Assert.AreEqual("__interceptors", proxyScope.Fields[0].Name);
            Assert.AreEqual("__myMixin", proxyScope.Fields[1].Name);
            Assert.AreEqual("MyMixin_MyProperty", proxyScope.Fields[2].Name);
            Assert.AreEqual("EmpyType_Proxy_get_MyProperty", proxyScope.Fields[3].Name);
            Assert.AreEqual("EmpyType_Proxy_set_MyProperty", proxyScope.Fields[4].Name);

            Assert.AreEqual("MyMixin_Method", proxyScope.Fields[5].Name);
            Assert.AreEqual("EmpyType_Proxy_Method", proxyScope.Fields[6].Name);

            Assert.AreEqual("MyMixin_MyEvent", proxyScope.Fields[7].Name);
            Assert.AreEqual("EmpyType_Proxy_add_MyEvent", proxyScope.Fields[8].Name);
            Assert.AreEqual("EmpyType_Proxy_remove_MyEvent", proxyScope.Fields[9].Name);

            // mappings
            var acc = new TypeAccessor(proxyScope);

            var propertyMappings = acc.Fields["propertyMappings"].GetValue() as List<PropertyMapping>;

            Assert.AreEqual(1, propertyMappings.Count);
            Assert.AreEqual("MyMixin_MyProperty", propertyMappings[0].MemberField.Name);
            Assert.AreEqual("EmpyType_Proxy_get_MyProperty", propertyMappings[0].GetMethodField.Name);
            Assert.AreEqual("EmpyType_Proxy_set_MyProperty", propertyMappings[0].SetMethodField.Name);

            Assert.AreEqual("MyProperty", propertyMappings[0].Property.Name);
            Assert.AreEqual("get_MyProperty", propertyMappings[0].GetMethodBuilder.Name);
            Assert.AreEqual("set_MyProperty", propertyMappings[0].SetMethodBuilder.Name);

            var methodMappings = acc.Fields["methodMappings"].GetValue() as List<MethodMapping>;

            Assert.AreEqual(1, methodMappings.Count);
            Assert.AreEqual("MyMixin_Method", methodMappings[0].MemberField.Name);
            Assert.AreEqual("EmpyType_Proxy_Method", methodMappings[0].MethodField.Name);

            Assert.AreEqual("Method", methodMappings[0].Method.Name);
            Assert.AreEqual("Method", methodMappings[0].MethodBuilder.Name);

            var eventMappings = acc.Fields["eventMappings"].GetValue() as List<EventMapping>;

            Assert.AreEqual(1, eventMappings.Count);
            Assert.AreEqual("MyMixin_MyEvent", eventMappings[0].MemberField.Name);
            Assert.AreEqual("EmpyType_Proxy_add_MyEvent", eventMappings[0].AddMethodField.Name);
            Assert.AreEqual("EmpyType_Proxy_remove_MyEvent", eventMappings[0].RemoveMethodField.Name);

            Assert.AreEqual("MyEvent", eventMappings[0].Event.Name);
            Assert.AreEqual("add_MyEvent", eventMappings[0].AddMethodBuilder.Name);
            Assert.AreEqual("remove_MyEvent", eventMappings[0].RemoveMethodBuilder.Name);
        }

        [TestMethod]
        public void Mixin_Do_Not_Include_Doublons_Test()
        {
            var o = new ProxyGeneratorOptions();
            o.AddMixinInstance(new MyMixin());

            var moduleScope = new ModuleScope();
            var generator = new ProxyGenerator(moduleScope);
            var typeDefinition = generator.GetTypeDefinition(typeof(TypeScope1), null, o);

            TypeBuilder typeBulder = moduleScope.Module.DefineType(typeDefinition.FullName, typeDefinition.TypeAttributes);

            var proxyScope = new ProxyScope(moduleScope, typeBulder, typeDefinition);
            proxyScope.DefineTypeAndMembers();

            Assert.AreEqual(1, proxyScope.Properties.Count);
            Assert.AreEqual("MyProperty", proxyScope.Properties[0].Name);

            Assert.AreEqual(10, proxyScope.Methods.Count); // get set meth, add, remove
            Assert.AreEqual("get_MyProperty_Callback", proxyScope.Methods[0].Name);
            Assert.AreEqual("get_MyProperty", proxyScope.Methods[1].Name);
            Assert.AreEqual("set_MyProperty_Callback", proxyScope.Methods[2].Name);
            Assert.AreEqual("set_MyProperty", proxyScope.Methods[3].Name);
            Assert.AreEqual("Method_Callback", proxyScope.Methods[4].Name);
            Assert.AreEqual("Method", proxyScope.Methods[5].Name);
            Assert.AreEqual("add_MyEvent_Callback", proxyScope.Methods[6].Name);
            Assert.AreEqual("add_MyEvent", proxyScope.Methods[7].Name);
            Assert.AreEqual("remove_MyEvent_Callback", proxyScope.Methods[8].Name);
            Assert.AreEqual("remove_MyEvent", proxyScope.Methods[9].Name);

            Assert.AreEqual(1, proxyScope.Events.Count);
            Assert.AreEqual("MyEvent", proxyScope.Events[0].GetName());


            // fields
            Assert.AreEqual(2, proxyScope.ConstructorFields.Length);
            Assert.AreEqual("__interceptors", proxyScope.ConstructorFields[0].Name);
            Assert.AreEqual("__myMixin", proxyScope.ConstructorFields[1].Name);

            Assert.AreEqual(10, proxyScope.Fields.Count);
            Assert.AreEqual("__interceptors", proxyScope.Fields[0].Name);
            Assert.AreEqual("__myMixin", proxyScope.Fields[1].Name);
            Assert.AreEqual("TypeScope1_MyProperty", proxyScope.Fields[2].Name);
            Assert.AreEqual("TypeScope1_Proxy_get_MyProperty", proxyScope.Fields[3].Name);
            Assert.AreEqual("TypeScope1_Proxy_set_MyProperty", proxyScope.Fields[4].Name);

            Assert.AreEqual("TypeScope1_Method", proxyScope.Fields[5].Name);
            Assert.AreEqual("TypeScope1_Proxy_Method", proxyScope.Fields[6].Name);

            Assert.AreEqual("TypeScope1_MyEvent", proxyScope.Fields[7].Name);
            Assert.AreEqual("TypeScope1_Proxy_add_MyEvent", proxyScope.Fields[8].Name);
            Assert.AreEqual("TypeScope1_Proxy_remove_MyEvent", proxyScope.Fields[9].Name);

            // mappings
            var acc = new TypeAccessor(proxyScope);

            var propertyMappings = acc.Fields["propertyMappings"].GetValue() as List<PropertyMapping>;

            Assert.AreEqual(1, propertyMappings.Count);
            Assert.AreEqual("TypeScope1_MyProperty", propertyMappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_get_MyProperty", propertyMappings[0].GetMethodField.Name);
            Assert.AreEqual("TypeScope1_Proxy_set_MyProperty", propertyMappings[0].SetMethodField.Name);

            Assert.AreEqual("MyProperty", propertyMappings[0].Property.Name);
            Assert.AreEqual("get_MyProperty", propertyMappings[0].GetMethodBuilder.Name);
            Assert.AreEqual("set_MyProperty", propertyMappings[0].SetMethodBuilder.Name);

            var methodMappings = acc.Fields["methodMappings"].GetValue() as List<MethodMapping>;

            Assert.AreEqual(1, methodMappings.Count);
            Assert.AreEqual("TypeScope1_Method", methodMappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_Method", methodMappings[0].MethodField.Name);

            Assert.AreEqual("Method", methodMappings[0].Method.Name);
            Assert.AreEqual("Method", methodMappings[0].MethodBuilder.Name);

            var eventMappings = acc.Fields["eventMappings"].GetValue() as List<EventMapping>;

            Assert.AreEqual(1, eventMappings.Count);
            Assert.AreEqual("TypeScope1_MyEvent", eventMappings[0].MemberField.Name);
            Assert.AreEqual("TypeScope1_Proxy_add_MyEvent", eventMappings[0].AddMethodField.Name);
            Assert.AreEqual("TypeScope1_Proxy_remove_MyEvent", eventMappings[0].RemoveMethodField.Name);

            Assert.AreEqual("MyEvent", eventMappings[0].Event.Name);
            Assert.AreEqual("add_MyEvent", eventMappings[0].AddMethodBuilder.Name);
            Assert.AreEqual("remove_MyEvent", eventMappings[0].RemoveMethodBuilder.Name);
        }

    }

    public class EmpyType
    {

    }

    public class TypeScope1
    {
        public virtual string MyProperty { get; set; }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    public interface IMyMixin
    {
        string MyProperty { get; set; }

        event EventHandler MyEvent;

        void Method();
    }

    public class MyMixin : IMyMixin
    {
        public virtual string MyProperty { get; set; }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

}
