using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace NIntercept.Tests.Builder
{
    [TestClass]
    public class ModuleScopeTests
    {
#if NET45 || NET472

        [TestMethod]
        public void Save_Assembly()
        {
            var moduleScope = new ModuleScope(true, false);

            bool failed = false;
            try
            {
                moduleScope.Save();
            }
            catch (Exception ex)
            {
                failed = true;
            }

            string path = "NIntercept.DynamicModule.dll";
            Assert.AreEqual(false, failed);
            Assert.IsTrue(File.Exists(path));

            File.Delete(path);
        }

        [TestMethod]
        public void Save_Assembly_With_Name()
        {
            var moduleScope = new ModuleScope("MyAssembly","MyModule", true, false);

            bool failed = false;
            try
            {
                moduleScope.Save();
            }
            catch (Exception ex)
            {
                failed = true;
            }

            string path = "MyModule.dll";
            Assert.AreEqual(false, failed);
            Assert.IsTrue(File.Exists(path));

            File.Delete(path);
        }

        [TestMethod]
        public void Save_Assembly_Strongly()
        {
            var moduleScope = new ModuleScope(true, true);

            bool failed = false;
            try
            {
                moduleScope.Save();
            }
            catch (Exception ex)
            {
                failed = true;
            }

            string path = "NIntercept.DynamicModule.dll";
            Assert.AreEqual(false, failed);
            Assert.IsTrue(File.Exists(path));

            File.Delete(path);
        }
#endif
    }
}
