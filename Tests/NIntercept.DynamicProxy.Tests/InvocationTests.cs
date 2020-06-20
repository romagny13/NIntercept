using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace NIntercept.Tests
{
    [TestClass]
    public class InvocationTests
    {
        [TestMethod]
        public void GetParameterByName()
        {
            var array = new object[]
          {
                10,
                "A"
          };
            var invocation = new InvocationMock(null, new IInterceptor[0], (MethodInfo)MethodBase.GetCurrentMethod(), (MethodInfo)MethodBase.GetCurrentMethod(), this, array);

            var v = invocation.GetParameter<int>(0);
            var v2 = invocation.GetParameter<string>(1);

            Assert.AreEqual(10, v);
            Assert.AreEqual("A", v2);
        }
    }

    
}
