﻿using NIntercept.Definition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace NIntercept.Tests.Builder
{
    [TestClass]
    public class ProxyTypeBuilderTests
    {
        [TestMethod]
        public void Attributes_Added_Correctly_WithParent()
        {
            var builder = new ProxyBuilder(TestHelpers.GetNewModuleScope());
            var collector = new ModuleDefinition();
            var typeDef = collector.GetOrAdd(typeof(TypeM1), null);

            TypeBuilder typeBuilder = builder.CreateType(typeDef, new IInterceptor[0]);
            Type type = TypeBuilderHelper.BuildType(typeBuilder);

            var typeAtt = type.GetCustomAttributes();
            var propSetAtt = type.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            var propGetAtt = type.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            var metAtt = type.GetMethod("Method").GetCustomAttributes();
            var evAddAtt = type.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            var evRemoveAtt = type.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();

            Assert.AreEqual(2, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(1).GetType());

            Assert.AreEqual(1, propGetAtt.Count());
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), propGetAtt.FirstOrDefault().GetType());
            Assert.AreEqual(2, propSetAtt.Count()); // + compiler attribute 
            Assert.AreEqual(typeof(PropertySetInterceptorAttribute), propSetAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), propSetAtt.ElementAt(1).GetType());

            Assert.AreEqual(1, metAtt.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), metAtt.FirstOrDefault().GetType());

            Assert.AreEqual(1, evRemoveAtt.Count());
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), evRemoveAtt.ElementAt(0).GetType());
            Assert.AreEqual(2, evAddAtt.Count()); // + compiler attribute 
            Assert.AreEqual(typeof(AddEventInterceptorAttribute), evAddAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(CompilerGeneratedAttribute), evAddAtt.ElementAt(1).GetType());
        }

        [TestMethod]
        public void Attributes_Added_Correctly_Full_Props_Events()
        {
            var builder = new ProxyBuilder(TestHelpers.GetNewModuleScope());
            var collector = new ModuleDefinition();
            var typeDef = collector.GetOrAdd(typeof(TypeM1Full), null);

            TypeBuilder typeBuilder = builder.CreateType(typeDef, new IInterceptor[0]);
            Type type = TypeBuilderHelper.BuildType(typeBuilder);

            var typeAtt = type.GetCustomAttributes();
            var propSetAtt = type.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            var propGetAtt = type.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            var metAtt = type.GetMethod("Method").GetCustomAttributes();
            var evAddAtt = type.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            var evRemoveAtt = type.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();

            Assert.AreEqual(2, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(1).GetType());

            Assert.AreEqual(0, propGetAtt.Count());
            Assert.AreEqual(1, propSetAtt.Count());
            Assert.AreEqual(typeof(PropertySetInterceptorAttribute), propSetAtt.FirstOrDefault().GetType());

            Assert.AreEqual(1, metAtt.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), metAtt.FirstOrDefault().GetType());

            Assert.AreEqual(0, evRemoveAtt.Count());
            Assert.AreEqual(1, evAddAtt.Count());
            Assert.AreEqual(typeof(AddEventInterceptorAttribute), evAddAtt.FirstOrDefault().GetType());
        }

        [TestMethod]
        public void Attributes_Added_Correctly_With_Interface()
        {
            var builder = new ProxyBuilder(TestHelpers.GetNewModuleScope());
            var collector = new ModuleDefinition();
            var typeDef = collector.GetOrAdd(typeof(ITypeM2), null);

            TypeBuilder typeBuilder = builder.CreateType(typeDef, new IInterceptor[0]);
            Type type = TypeBuilderHelper.BuildType(typeBuilder);

            var typeAtt = type.GetCustomAttributes();
            var propSetAtt = type.GetProperty("MyProperty").SetMethod.GetCustomAttributes();
            var propGetAtt = type.GetProperty("MyProperty").GetMethod.GetCustomAttributes();
            var metAtt = type.GetMethod("Method").GetCustomAttributes();
            var evAddAtt = type.GetEvent("MyEvent").AddMethod.GetCustomAttributes();
            var evRemoveAtt = type.GetEvent("MyEvent").RemoveMethod.GetCustomAttributes();

            Assert.AreEqual(2, typeAtt.Count());
            Assert.AreEqual(typeof(SerializableAttribute), typeAtt.FirstOrDefault().GetType());
            Assert.AreEqual(typeof(AllInterceptorAttribute), typeAtt.ElementAt(1).GetType());

            Assert.AreEqual(0, propGetAtt.Count());
            Assert.AreEqual(1, propSetAtt.Count());
            Assert.AreEqual(typeof(PropertySetInterceptorAttribute), propSetAtt.FirstOrDefault().GetType());

            Assert.AreEqual(1, metAtt.Count());
            Assert.AreEqual(typeof(MethodInterceptorAttribute), metAtt.FirstOrDefault().GetType());

            Assert.AreEqual(0, evRemoveAtt.Count());
            Assert.AreEqual(1, evAddAtt.Count());
            Assert.AreEqual(typeof(AddEventInterceptorAttribute), evAddAtt.FirstOrDefault().GetType());
        }
    }
}