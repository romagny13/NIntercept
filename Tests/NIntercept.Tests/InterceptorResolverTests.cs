using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIntercept.Tests
{

    [TestClass]
    public class InterceptorResolverTests
    {

        [TestInitialize()]
        public void Startup()
        {
            InterceptorResolver.ClearCache();
        }

        [TestMethod]
        public void Stores_Each_Member_In_Cache()
        {
            var m1 = typeof(TypeRI1).GetMethod("Method");
            var m2 = typeof(TypeRI2).GetMethod("Method");

            var interceptorsPerMember = new TypeAccessor(typeof(InterceptorResolver)).Fields["interceptorsPerMemberCache"].GetValue<Dictionary<MemberInfo, IInterceptor[]>>();

            Assert.AreEqual(0, interceptorsPerMember.Count);

            var all1 = InterceptorResolver.GetAllInterceptors(m1, new IInterceptor[0], typeof(IMethodInterceptorProvider));
            var all2 = InterceptorResolver.GetAllInterceptors(m2, new IInterceptor[0], typeof(IMethodInterceptorProvider));

            Assert.AreEqual(1, all1.Length);
            Assert.AreEqual(typeof(IntForRI1), all1.ElementAt(0).GetType());
            Assert.AreEqual(0, all2.Length);
            Assert.AreEqual(2, interceptorsPerMember.Count);
            Assert.AreEqual(1, interceptorsPerMember[m1].Length);
            Assert.AreEqual(typeof(IntForRI1), interceptorsPerMember[m1][0].GetType());
            Assert.AreEqual(0, interceptorsPerMember[m2].Length);
        }

        [TestMethod]
        public void Returns_All_Interceptors()
        {
            var m1 = typeof(TypeRI1).GetMethod("Method");

            var interceptorsPerMember = new TypeAccessor(typeof(InterceptorResolver)).Fields["interceptorsPerMemberCache"].GetValue<Dictionary<MemberInfo, IInterceptor[]>>();

            Assert.AreEqual(0, interceptorsPerMember.Count);

            var all1 = InterceptorResolver.GetAllInterceptors(m1, new IInterceptor[0], typeof(IMethodInterceptorProvider));

            Assert.AreEqual(1, all1.Length);
            Assert.AreEqual(typeof(IntForRI1), all1.ElementAt(0).GetType());
            Assert.AreEqual(1, interceptorsPerMember.Count);
            Assert.AreEqual(1, interceptorsPerMember[m1].Length);
            Assert.AreEqual(typeof(IntForRI1), interceptorsPerMember[m1][0].GetType());

            var all2 = InterceptorResolver.GetAllInterceptors(m1, new IInterceptor[] { new IntForRI1_b() }, typeof(IMethodInterceptorProvider));

            Assert.AreEqual(2, all2.Length);
            Assert.AreEqual(typeof(IntForRI1_b), all2.ElementAt(0).GetType());
            Assert.AreEqual(typeof(IntForRI1), all2.ElementAt(1).GetType());
            Assert.AreEqual(1, interceptorsPerMember.Count);
            Assert.AreEqual(1, interceptorsPerMember[m1].Length);
            Assert.AreEqual(typeof(IntForRI1), interceptorsPerMember[m1][0].GetType());
        }

        [TestMethod]
        public void Add_Method_InterceptionAttributes_To_Interceptors()
        {
            var result = InterceptorResolver.GetAllInterceptors(typeof(TypeC1).GetMethod("MethodA"), new IInterceptor[] { new Int2() }, typeof(IMethodInterceptorProvider));

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(typeof(Int2), result[0].GetType());
            Assert.AreEqual(typeof(Int1), result[1].GetType());
        }

        [TestMethod]
        public void Adds_Type_And_Method_InterceptionAttributes_To_Interceptors()
        {
            var result = InterceptorResolver.GetAllInterceptors(typeof(TypeC2).GetMethod("MethodA"), new IInterceptor[] { new Int2() }, typeof(IMethodInterceptorProvider));

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(typeof(Int2), result[0].GetType());
            Assert.AreEqual(typeof(Int3), result[1].GetType());
            Assert.AreEqual(typeof(Int1), result[2].GetType());
        }

        [TestMethod]
        public void Returns_Original_With_No_Interceptors_Added()
        {
            var result = InterceptorResolver.GetAllInterceptors(typeof(TypeC3).GetMethod("MethodA"), new IInterceptor[] { new Int2() }, typeof(IInterceptorProvider));

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(typeof(Int2), result[0].GetType());
        }
    }


    public class TypeRI1
    {
        [MethodInterceptor(typeof(IntForRI1))]
        public void Method()
        {

        }
    }

    public class TypeRI2
    {
        public void Method()
        {

        }
    }


    public class IntForRI1 : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class IntForRI1_b : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class TypeC1
    {
        [MethodInterceptor(typeof(Int1))]
        public void MethodA()
        {

        }
    }

    [AllInterceptor(typeof(Int3))]
    public class TypeC2
    {
        [MethodInterceptor(typeof(Int1))]
        public void MethodA()
        {

        }
    }

    public class TypeC3
    {
        public void MethodA()
        {

        }
    }
}
