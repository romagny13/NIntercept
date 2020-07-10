using NIntercept.Builder;
using NIntercept.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NIntercept.Tests
{

    public class AdditionalCodeMock1 : AdditionalCode
    {

    }

    public class AdditionalCodeMock2 : AdditionalCode
    {

    }

    public class ConstructorSelectorMock1 : DefaultConstructorSelector
    {

    }

    public class ConstructorSelectorMock2 : DefaultConstructorSelector
    {

    }

    public class ClassProxyMemberSelectorMock1 : ClassProxyMemberSelector
    {

    }

    public class ClassProxyMemberSelectorMock2 : ClassProxyMemberSelector
    {

    }

    public class InterceptableMethodBuilderMock1 : InterceptableMethodBuilder
    {

    }

    public class InterceptableMethodBuilderMock2 : InterceptableMethodBuilder
    {

    }

    public class MYInterceptorSelectorMock : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptors;
        }
    }

    public class MYInterceptorSelectorMock2 : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptors;
        }
    }
}
