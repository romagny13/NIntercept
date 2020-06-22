using System;

namespace NIntercept
{
    public interface IInterceptorProvider
    {
        Type InterceptorType { get; }
    }
}
