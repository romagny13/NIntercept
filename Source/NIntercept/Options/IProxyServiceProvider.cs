using NIntercept.Builder;

namespace NIntercept
{
    public interface IProxyServiceProvider
    {
        ICallbackMethodBuilder CallbackMethodBuilder { get; }
        IProxyEventBuilder ProxyEventBuilder { get; }
        IProxyMethodBuilder ProxyMethodBuilder { get; }
        IProxyPropertyBuilder ProxyPropertyBuilder { get; }
    }
}
