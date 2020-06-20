using NIntercept.Definition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace NIntercept.Tests.Definition
{
    [TestClass]
    public class ModuleDefinitionTests
    {
        [TestMethod]
        public void FindTypeDefinitions()
        {
            var moduleDefinition = new ModuleDefinition();

            var t1 = new ProxyTypeDefinitionMock(moduleDefinition, typeof(TypeM1), null);
            moduleDefinition.TypeDefinitions.Add(t1);
            var target = new TypeM1();

            Assert.IsTrue(moduleDefinition.TypeDefinitions.ContainsByType(typeof(TypeM1), null));
            Assert.IsFalse(moduleDefinition.TypeDefinitions.ContainsByType(typeof(TypeM1), target));

            var t2 = new ProxyTypeDefinitionMock(moduleDefinition, typeof(TypeM1), target);
            moduleDefinition.TypeDefinitions.Add(t2);
            Assert.IsTrue(moduleDefinition.TypeDefinitions.ContainsByType(typeof(TypeM1), target));
        }

        [TestMethod]
        public void Returns_The_Same_TypeDefinition()
        {
            var moduleDefinition = new ModuleDefinition();

            var t1 = moduleDefinition.GetOrAdd(typeof(TypeM1), null);

            Assert.AreEqual("TypeM1_Proxy", t1.Name);

            var t2 = moduleDefinition.GetOrAdd(typeof(TypeM1), null);

            Assert.AreEqual("TypeM1_Proxy", t1.Name);
            Assert.AreEqual(t2, t1);
        }

        [TestMethod]
        public void Get_Unique_Name_For_Types()
        {
            var moduleDefinition = new ModuleDefinition();

            var t1 = moduleDefinition.GetOrAdd(typeof(TypeM1), null);

            Assert.AreEqual("TypeM1_Proxy", t1.Name);

            var target = new TypeM1 { MyProperty = "A" };
            var target2 = new TypeM1 { MyProperty = "B" };

            Assert.IsFalse(Equals(target, target2));

            var t2 = moduleDefinition.GetOrAdd(typeof(TypeM1), target);

            Assert.AreEqual("TypeM1_1_Proxy", t2.Name);

            var t3 = moduleDefinition.GetOrAdd(typeof(TypeM1), target2);

            Assert.AreEqual("TypeM1_2_Proxy", t3.Name);
        }

        [TestMethod]
        public void Get_Unique_Name_For_Interfaces()
        {
            var moduleDefinition = new ModuleDefinition();

            var t1 = moduleDefinition.GetOrAdd(typeof(ITypeM1), null);

            Assert.AreEqual("ITypeM1_Proxy", t1.Name);

            var target = new TypeM1 { MyProperty = "A" };
            var target2 = new TypeM1 { MyProperty = "B" };

            Assert.IsFalse(Equals(target, target2));

            var t2 = moduleDefinition.GetOrAdd(typeof(ITypeM1), target);

            Assert.AreEqual("ITypeM1_1_Proxy", t2.Name);

            var t3 = moduleDefinition.GetOrAdd(typeof(ITypeM1), target2);

            Assert.AreEqual("ITypeM1_2_Proxy", t3.Name);
        }

        #region Properties

        [TestMethod]
        public void Get_Update_Type_Names()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), null);
            Assert.AreEqual("Type1_Proxy", typeDef.Name);
            Assert.AreEqual("Interception.Proxies.Type1_Proxy", typeDef.FullName);

            typeDef.Namespace = null;
            Assert.AreEqual("Type1_Proxy", typeDef.FullName);
        }

        [TestMethod]
        public void Find_Virtual_Props()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), null);
            Assert.AreEqual(false, typeDef.IsInterface);
            Assert.AreEqual(typeof(Type1), typeDef.Type);
            Assert.AreEqual(3, typeDef.PropertyDefinitions.Length);
            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            var p2 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("MyPropertyReadOnly", p2.Name);
            var p3 = typeDef.PropertyDefinitions[2];
            Assert.AreEqual("MyPropertyWriteOnly", p3.Name);
        }

        [TestMethod]
        public void Create_Methods_For_Read_Write()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), null);
            Assert.AreEqual(3, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.AreEqual(typeof(string), p1.PropertyType);
            Assert.IsNotNull(p1.PropertyGetMethodDefinition);
            Assert.IsNotNull(p1.PropertySetMethodDefinition);

            var p2 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("MyPropertyReadOnly", p2.Name);
            Assert.AreEqual(typeof(int), p2.PropertyType);
            Assert.IsNotNull(p2.PropertyGetMethodDefinition);
            Assert.IsNull(p2.PropertySetMethodDefinition);

            var p3 = typeDef.PropertyDefinitions[2];
            Assert.AreEqual("MyPropertyWriteOnly", p3.Name);
            Assert.AreEqual(typeof(double), p3.PropertyType);
            Assert.IsNull(p3.PropertyGetMethodDefinition);
            Assert.IsNotNull(p3.PropertySetMethodDefinition);
        }

        [TestMethod]
        public void Get_And_Set_Methods_Details()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), null);
            Assert.AreEqual(3, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[0].PropertyGetMethodDefinition;
            Assert.AreEqual("get_MyProperty", p1.Name);
            Assert.AreEqual(typeof(string), p1.ReturnType);
            Assert.AreEqual(typeof(Type1).GetMethod("get_MyProperty"), p1.Method);

            var p2 = typeDef.PropertyDefinitions[0].PropertySetMethodDefinition;
            Assert.AreEqual("set_MyProperty", p2.Name);
            Assert.AreEqual(typeof(void), p2.ReturnType);
            Assert.AreEqual(typeof(Type1).GetMethod("set_MyProperty"), p2.Method);
        }

        [TestMethod]
        public void Find_Props_Of_Interface()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Interface1), null);
            Assert.AreEqual(true, typeDef.IsInterface);
            Assert.AreEqual(typeof(Interface1), typeDef.Type);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.AreEqual(typeof(string), p1.PropertyType);
            Assert.IsNotNull(p1.PropertyGetMethodDefinition);
            Assert.IsNotNull(p1.PropertySetMethodDefinition);

            var p2 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("MyPropertyReadOnly", p2.Name);
            Assert.AreEqual(typeof(int), p2.PropertyType);
            Assert.IsNotNull(p2.PropertyGetMethodDefinition);
            Assert.IsNull(p2.PropertySetMethodDefinition);

            var p3 = typeDef.PropertyDefinitions[2];
            Assert.AreEqual("MyPropertyWriteOnly", p3.Name);
            Assert.AreEqual(typeof(double), p3.PropertyType);
            Assert.IsNull(p3.PropertyGetMethodDefinition);
            Assert.IsNotNull(p3.PropertySetMethodDefinition);
        }

        [TestMethod]
        public void Find_Attributes_On_Properties()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type2), null);
            Assert.AreEqual(3, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.AreEqual(1, p1.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), p1.PropertyGetInterceptorAttributes[0].InterceptorType);

            Assert.AreEqual(2, p1.PropertySetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int2), p1.PropertySetInterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(typeof(Int3), p1.PropertySetInterceptorAttributes[1].InterceptorType);

            var p2 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("FullProperty", p2.Name);
            Assert.AreEqual(1, p2.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), p2.PropertyGetInterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(0, p2.PropertySetInterceptorAttributes.Length);

            Assert.AreEqual(1, p2.PropertySetMethodDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int3), p2.PropertySetMethodDefinition.InterceptorAttributes[0].InterceptorType);

            var p3 = typeDef.PropertyDefinitions[2];
            Assert.AreEqual("ReadOnlyProp", p3.Name);
            Assert.AreEqual(1, p3.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), p3.PropertyGetInterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(0, p3.PropertySetInterceptorAttributes.Length);
        }

        [TestMethod]
        public void IgnoreCustom_Attributes_On_Properties()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type3), null);
            Assert.AreEqual(2, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.AreEqual(0, p1.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(0, p1.PropertySetInterceptorAttributes.Length);
            Assert.AreEqual(1, p1.Property.GetCustomAttributes(false).Length);
        }

        [TestMethod]
        public void FindCustom_InterceptionAttribute_On_Properties()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type3), null);
            Assert.AreEqual(2, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("MyProperty2", p1.Name);
            Assert.AreEqual(0, p1.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(1, p1.PropertySetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(MySetAtt), p1.PropertySetInterceptorAttributes[0].AttributeType);
            Assert.AreEqual(typeof(Int1), p1.PropertySetInterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(1, p1.Property.GetCustomAttributes(false).Length);
        }

        [TestMethod]
        public void Find_Attributes_On_Properties_Of_Interface()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Interface2), null);

            Assert.AreEqual(2, typeDef.PropertyDefinitions.Length);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.AreEqual(1, p1.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), p1.PropertyGetInterceptorAttributes[0].InterceptorType);

            Assert.AreEqual(1, p1.PropertySetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int2), p1.PropertySetInterceptorAttributes[0].InterceptorType);

            var p2 = typeDef.PropertyDefinitions[1];
            Assert.AreEqual("ReadOnlyProp", p2.Name);
            Assert.AreEqual(1, p2.PropertyGetInterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), p2.PropertyGetInterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(0, p2.PropertySetInterceptorAttributes.Length);
        }

        [TestMethod]
        public void Invocation_And_Callback_For_Properties()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), null);

            var p1 = typeDef.PropertyDefinitions[0];
            Assert.AreEqual("MyProperty", p1.Name);
            Assert.IsNotNull(p1.PropertyGetMethodDefinition.InvocationTypeDefinition);
            Assert.AreEqual("Type1_Proxy_get_MyProperty_Invocation", p1.PropertyGetMethodDefinition.InvocationTypeDefinition.Name);
            Assert.AreEqual("get_MyProperty_Callback", p1.PropertyGetMethodDefinition.MethodCallbackDefinition.Name);
            Assert.IsNotNull(p1.PropertyGetMethodDefinition.MethodCallbackDefinition);
            Assert.AreEqual("Type1_Proxy_set_MyProperty_Invocation", p1.PropertySetMethodDefinition.InvocationTypeDefinition.Name);
            Assert.AreEqual("set_MyProperty_Callback", p1.PropertySetMethodDefinition.MethodCallbackDefinition.Name);
        }

        #endregion // Properties

        #region Methods

        [TestMethod]
        public void Find_Methods()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type4), null);

            Assert.AreEqual(3, typeDef.MethodDefinitions.Length);

            var p1 = typeDef.MethodDefinitions[0];
            Assert.AreEqual("MethodA", p1.Name);
            Assert.AreEqual(typeof(void), p1.ReturnType);
            Assert.AreEqual(0, p1.ParameterDefinitions.Length);

            var p2 = typeDef.MethodDefinitions[1];
            Assert.AreEqual("MethodC", p2.Name);
            Assert.AreEqual(typeof(int), p2.ReturnType);
            Assert.AreEqual(2, p2.ParameterDefinitions.Length);
            Assert.AreEqual(0, p2.ParameterDefinitions[0].Index);
            Assert.AreEqual("a", p2.ParameterDefinitions[0].Name);
            Assert.AreEqual(false, p2.ParameterDefinitions[0].IsByRef);
            Assert.AreEqual(false, p2.ParameterDefinitions[0].IsOut);
            Assert.AreEqual(typeof(string), p2.ParameterDefinitions[0].ParameterType);
            Assert.AreEqual(null, p2.ParameterDefinitions[0].ElementType);

            var p3 = typeDef.MethodDefinitions[2];
            Assert.AreEqual("MethodD", p3.Name);
            Assert.AreEqual(typeof(int), p3.ReturnType);
            Assert.AreEqual(2, p3.ParameterDefinitions.Length);
            Assert.AreEqual(0, p3.ParameterDefinitions[0].Index);
            Assert.AreEqual("a", p3.ParameterDefinitions[0].Name);
            Assert.AreEqual(true, p3.ParameterDefinitions[0].IsByRef);
            Assert.AreEqual(true, p3.ParameterDefinitions[0].IsOut);
            Assert.AreEqual(typeof(int), p3.ParameterDefinitions[0].ElementType);

            Assert.AreEqual(1, p3.ParameterDefinitions[1].Index);
            Assert.AreEqual("b", p3.ParameterDefinitions[1].Name);
            Assert.AreEqual(true, p3.ParameterDefinitions[1].IsByRef);
            Assert.AreEqual(false, p2.ParameterDefinitions[1].IsOut);
            Assert.AreEqual(typeof(int), p3.ParameterDefinitions[1].ElementType);

            Assert.AreEqual(1, p3.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyMethodAtt), p3.InterceptorAttributes[0].AttributeType);
            Assert.AreEqual(typeof(Int1), p3.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void Find_Methods_Generics()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type5), null);

            Assert.AreEqual(2, typeDef.MethodDefinitions.Length);

            var p1 = typeDef.MethodDefinitions[0];
            Assert.AreEqual("MethodA", p1.Name);
            Assert.AreEqual(typeof(int), p1.ReturnType);
            Assert.AreEqual(1, p1.ParameterDefinitions.Length);
            Assert.AreEqual(2, p1.GenericArguments.Length);
            Assert.AreEqual("T", p1.GenericArguments[0].Name);
            Assert.AreEqual("T2", p1.GenericArguments[1].Name);

            var p2 = typeDef.MethodDefinitions[1];
            Assert.AreEqual("MethodB", p2.Name);
            Assert.AreEqual(typeof(int), p2.ReturnType);
            Assert.AreEqual(2, p2.ParameterDefinitions.Length);
            Assert.AreEqual(true, p2.ParameterDefinitions[0].IsByRef);
            Assert.AreEqual(true, p2.ParameterDefinitions[0].IsOut);
            Assert.AreEqual(true, p2.ParameterDefinitions[1].IsByRef);
            Assert.AreEqual(false, p2.ParameterDefinitions[1].IsOut);

            Assert.AreEqual(1, p2.GenericArguments.Length);
            Assert.AreEqual("T", p2.GenericArguments[0].Name);

            Assert.AreEqual(1, p2.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyMethodAtt), p2.InterceptorAttributes[0].AttributeType);
            Assert.AreEqual(typeof(Int1), p2.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void Find_Methods_Generics_For_Interface()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(IType5), null);

            Assert.AreEqual(2, typeDef.MethodDefinitions.Length);

            var p1 = typeDef.MethodDefinitions[0];
            Assert.AreEqual("MethodA", p1.Name);
            Assert.AreEqual(typeof(int), p1.ReturnType);
            Assert.AreEqual(1, p1.ParameterDefinitions.Length);
            Assert.AreEqual(2, p1.GenericArguments.Length);
            Assert.AreEqual("T", p1.GenericArguments[0].Name);
            Assert.AreEqual("T2", p1.GenericArguments[1].Name);

            var p2 = typeDef.MethodDefinitions[1];
            Assert.AreEqual("MethodB", p2.Name);
            Assert.AreEqual(typeof(int), p2.ReturnType);
            Assert.AreEqual(2, p2.ParameterDefinitions.Length);
            Assert.AreEqual(true, p2.ParameterDefinitions[0].IsByRef);
            Assert.AreEqual(true, p2.ParameterDefinitions[0].IsOut);
            Assert.AreEqual(true, p2.ParameterDefinitions[1].IsByRef);
            Assert.AreEqual(false, p2.ParameterDefinitions[1].IsOut);

            Assert.AreEqual(1, p2.GenericArguments.Length);
            Assert.AreEqual("T", p2.GenericArguments[0].Name);

            Assert.AreEqual(1, p2.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyMethodAtt), p2.InterceptorAttributes[0].AttributeType);
            Assert.AreEqual(typeof(Int1), p2.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void Invocation_And_Callback_For_Methods()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type4), null);

            var p1 = typeDef.MethodDefinitions[0];
            Assert.AreEqual("MethodA", p1.Name);
            Assert.IsNotNull(p1.MethodCallbackDefinition);
            Assert.AreEqual("MethodA_Callback", p1.MethodCallbackDefinition.Name);
            Assert.AreEqual(p1.Method, p1.MethodCallbackDefinition.Method);

            Assert.IsNotNull(p1.InvocationTypeDefinition);
            Assert.AreEqual("Type4_Proxy_MethodA_Invocation", p1.InvocationTypeDefinition.Name);
            Assert.AreEqual("Interception.Invocations.Type4_Proxy_MethodA_Invocation", p1.InvocationTypeDefinition.FullName);

            p1.InvocationTypeDefinition.Namespace = null;
            Assert.AreEqual("Type4_Proxy_MethodA_Invocation", p1.InvocationTypeDefinition.FullName);
        }

        [TestMethod]
        public void Change_Names_Only_For_Invocation_Class_With_Signatures()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type7), null);

            Assert.AreEqual("Type7_Proxy_MethodA_Invocation", typeDef.MethodDefinitions[0].InvocationTypeDefinition.Name);
            Assert.AreEqual("MethodA_Callback", typeDef.MethodDefinitions[0].MethodCallbackDefinition.Name);

            Assert.AreEqual("Type7_Proxy_MethodA_1_Invocation", typeDef.MethodDefinitions[1].InvocationTypeDefinition.Name);
            Assert.AreEqual("MethodA_Callback", typeDef.MethodDefinitions[1].MethodCallbackDefinition.Name);
        }


        #endregion // Methods

        #region Events

        [TestMethod]
        public void Find_Events()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type6), null);

            Assert.AreEqual(2, typeDef.EventDefinitions.Length);
            Assert.AreEqual("MyEvent", typeDef.EventDefinitions[0].Name);
            Assert.AreEqual("add_MyEvent", typeDef.EventDefinitions[0].AddEventMethodDefinition.Name);
            Assert.AreEqual(1, typeDef.EventDefinitions[0].AddEventInterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyEventAtt), typeDef.EventDefinitions[0].AddEventInterceptorAttributes[0].AttributeType);
            Assert.AreEqual("remove_MyEvent", typeDef.EventDefinitions[0].RemoveEventMethodDefinition.Name);

            Assert.AreEqual("MyEvent3", typeDef.EventDefinitions[1].Name);
            Assert.AreEqual("add_MyEvent3", typeDef.EventDefinitions[1].AddEventMethodDefinition.Name);
            Assert.AreEqual("remove_MyEvent3", typeDef.EventDefinitions[1].RemoveEventMethodDefinition.Name);
            Assert.AreEqual(typeof(MyEventAtt), typeDef.EventDefinitions[1].AddEventInterceptorAttributes[0].AttributeType);
        }

        [TestMethod]
        public void Find_Events_For_Interface()
        {
            var moduleDefinition = new ModuleDefinition();
            var typeDef = moduleDefinition.GetOrAdd(typeof(IType6), null);

            Assert.AreEqual(2, typeDef.EventDefinitions.Length);
            Assert.AreEqual("MyEvent", typeDef.EventDefinitions[0].Name);
            Assert.AreEqual("add_MyEvent", typeDef.EventDefinitions[0].AddEventMethodDefinition.Name);
            Assert.AreEqual(1, typeDef.EventDefinitions[0].AddEventInterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyEventAtt), typeDef.EventDefinitions[0].AddEventInterceptorAttributes[0].AttributeType);
            Assert.AreEqual("remove_MyEvent", typeDef.EventDefinitions[0].RemoveEventMethodDefinition.Name);

            Assert.AreEqual("MyEvent3", typeDef.EventDefinitions[1].Name);
            Assert.AreEqual("add_MyEvent3", typeDef.EventDefinitions[1].AddEventMethodDefinition.Name);
            Assert.AreEqual("remove_MyEvent3", typeDef.EventDefinitions[1].RemoveEventMethodDefinition.Name);
            Assert.AreEqual(typeof(MyEventAtt), typeDef.EventDefinitions[1].AddEventInterceptorAttributes[0].AttributeType);
        }

        #endregion // Events

        [TestMethod]
        public void TargetIsNotNull()
        {
            var moduleDefinition = new ModuleDefinition();
            var target = new Type1();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), target);
            Assert.IsNotNull(typeDef.Target);
        }

        [TestMethod]
        public void Find_Attribute_On_Type()
        {
            var moduleDefinition = new ModuleDefinition();
            var target = new Type1();
            var typeDef = moduleDefinition.GetOrAdd(typeof(Type1), target);
            Assert.AreEqual(1, typeDef.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(Int1), typeDef.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void AllInterceptorAttribute_Not_Copied_For_Class_Inherit_Attribute()
        {
            var moduleDefinition = new ModuleDefinition();

            var typeDefinition = moduleDefinition.GetOrAdd(typeof(Type8), null);

            Assert.AreEqual(1, typeDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeDefinition.InterceptorAttributes[0].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int1), typeDefinition.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void CustomInterceptorAttribute_Is_Added_To_Virtual_Members()
        {
            var moduleDefinition = new ModuleDefinition();

            var typeDefinition = moduleDefinition.GetOrAdd(typeof(Type8_b), null);

            Assert.AreEqual(1, typeDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyCustomInterceptorAttribute), typeDefinition.InterceptorAttributes[0].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int1), typeDefinition.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void AllInterceptorAttribute_Is_Added_To_Type_For_Interface()
        {
            var moduleDefinition = new ModuleDefinition();

            var typeDefinition = moduleDefinition.GetOrAdd(typeof(IType9), null);

            Assert.AreEqual(1, typeDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeDefinition.InterceptorAttributes[0].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int1), typeDefinition.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void CustomInterceptorAttribute_Is_Added_To_Type_For_Interface()
        {
            var moduleDefinition = new ModuleDefinition();

            var typeDefinition = moduleDefinition.GetOrAdd(typeof(IType9_b), null);

            Assert.AreEqual(1, typeDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(MyCustomInterceptorAttribute), typeDefinition.InterceptorAttributes[0].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int1), typeDefinition.InterceptorAttributes[0].InterceptorType);
        }

        [TestMethod]
        public void InterceptorAttributes_Added_To_Type_For_Interface_Child()
        {
            var moduleDefinition = new ModuleDefinition();

            var parentTypeDefinition = moduleDefinition.GetOrAdd(typeof(IType10_3), null);

            Assert.AreEqual(2, parentTypeDefinition.InterceptorAttributes.Length);
            Assert.AreEqual(typeof(AllInterceptorAttribute), parentTypeDefinition.InterceptorAttributes[0].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int1), parentTypeDefinition.InterceptorAttributes[0].InterceptorType);
            Assert.AreEqual(typeof(MyCustomInterceptorAttribute), parentTypeDefinition.InterceptorAttributes[1].AttributeData.AttributeType);
            Assert.AreEqual(typeof(Int2), parentTypeDefinition.InterceptorAttributes[1].InterceptorType);
        }
    }

    #region Properties

    [AllInterceptor(typeof(Int1))]
    public class Type1
    {
        public virtual string MyProperty { get; set; }

        public virtual int MyPropertyReadOnly { get; }

        private double myPropertyWriteOnly;

        public virtual double MyPropertyWriteOnly
        {
            set { myPropertyWriteOnly = value; }
        }

        public int NotVirtual1 { get; set; }
        public int NotVirtual2 { get; set; }
    }

    public interface Interface1
    {
        string MyProperty { get; set; }
        int MyPropertyReadOnly { get; }
        double MyPropertyWriteOnly { set; }
    }

    public class Type2
    {
        [PropertyGetInterceptor(typeof(Int1))]
        [PropertySetInterceptor(typeof(Int2))]
        [PropertySetInterceptor(typeof(Int3))]
        public virtual string MyProperty { get; set; }

        private int fulleProperty;

        [PropertyGetInterceptor(typeof(Int1))]
        public virtual int FullProperty
        {
            get { return fulleProperty; }
            [MethodInterceptor(typeof(Int3))]
            set { fulleProperty = value; }
        }

        private string readOnlyProp;

        [PropertyGetInterceptor(typeof(Int1))]
        public virtual string ReadOnlyProp
        {
            get { return readOnlyProp; }
        }

    }

    public class Type3
    {
        [MyCustom]
        public virtual string MyProperty { get; set; }

        [MySetAtt]
        public virtual string MyProperty2 { get; set; }
    }

    #endregion // properties

    public interface ITypeM1
    {
        string MyProperty { get; set; }
    }

    public class TypeM1 : ITypeM1
    {
        public string MyProperty { get; set; }
    }


    [AllInterceptor(typeof(Int1))]
    public class Type8
    {
        public virtual string MyProperty { get; set; }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Interface, AllowMultiple = true)]
    public class MyCustomInterceptorAttribute : InterceptorAttributeBase, 
        IPropertySetInterceptorProvider, 
        IMethodInterceptorProvider, 
        IAddEventInterceptorProvider
    {
        public MyCustomInterceptorAttribute(Type interceptorType) : base(interceptorType)
        {
        }
    }

    [MyCustomInterceptor(typeof(Int1))]
    public class Type8_b
    {
        public virtual string MyProperty { get; set; }

        public virtual void Method()
        {

        }

        public virtual event EventHandler MyEvent;
    }

    [AllInterceptor(typeof(Int1))]
    public interface IType9
    {
        string MyProperty { get; set; }

        void Method();

        event EventHandler MyEvent;
    }

    [MyCustomInterceptor(typeof(Int1))]
    public interface IType9_b
    {
        string MyProperty { get; set; }

        void Method();

        event EventHandler MyEvent;
    }


    [AllInterceptor(typeof(Int1))]
    [MyCustomInterceptor(typeof(Int2))]
    public interface IType10
    {
        string MyProperty { get; set; }

        void Method();

        event EventHandler MyEvent;
    }


    public interface IType10_2 : IType10
    {

    }

    public interface IType10_3 : IType10_2
    {

    }

    public class Type4
    {
        public virtual void MethodA()
        {

        }

        public void MethodB()
        {

        }

        public virtual int MethodC(string a, int b)
        {
            return 10;
        }

        [MyMethodAtt]
        public virtual int MethodD(out int a, ref int b)
        {
            a = 0;
            b = 0;
            return 10;
        }
    }

    public interface IType5
    {
        int MethodA<T, T2>(int a);

        [MyMethodAtt]
        int MethodB<T>(out int a, ref int b) where T : new();
    }

    public class Type5 : IType5
    {
        public virtual int MethodA<T, T2>(int a)
        {
            return 10;
        }

        [MyMethodAtt]
        public virtual int MethodB<T>(out int a, ref int b) where T : new()
        {
            a = 0;
            b = 0;
            return 10;
        }
    }

    public interface IType6
    {
        [MyEventAtt]
        event EventHandler<MyEventArgs> MyEvent;

        [MyEventAtt]
        event EventHandler<MyEventArgs> MyEvent3;
    }

    public class Type6 : IType6
    {
        [MyEventAtt]
        public virtual event EventHandler<MyEventArgs> MyEvent;

        public event EventHandler<MyEventArgs> MyEvent2;

        public event EventHandler<MyEventArgs> myEvent3;

        [MyEventAtt]
        public virtual event EventHandler<MyEventArgs> MyEvent3
        {
            add { myEvent3 += value; }
            remove { myEvent3 -= value; }
        }
    }

    public class Type7
    {
        public virtual void MethodA()
        {

        }

        public virtual void MethodA(string a)
        {

        }
    }

    public class MyEventArgs : EventArgs
    {

    }

    public class MySetAtt : PropertySetInterceptorAttribute
    {
        public MySetAtt()
            : base(typeof(Int1))
        {
        }
    }

    public class MyMethodAtt : MethodInterceptorAttribute
    {
        public MyMethodAtt()
            : base(typeof(Int1))
        {
        }
    }

    public class MyEventAtt : AddEventInterceptorAttribute
    {
        public MyEventAtt()
            : base(typeof(Int1))
        {
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class MyCustomAttribute : Attribute
    {

    }

    public interface Interface2
    {
        [PropertyGetInterceptor(typeof(Int1))]
        [PropertySetInterceptor(typeof(Int2))]
        string MyProperty { get; set; }

        [PropertyGetInterceptor(typeof(Int1))]
        string ReadOnlyProp { get; }
    }

    public class Int1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class Int2 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class Int3 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}
