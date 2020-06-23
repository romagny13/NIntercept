namespace NIntercept
{
    public interface IProxyServiceProvider
    {
        ICallbackMethodBuilder CallbackMethodBuilder { get; }
        IInvocationTypeBuilder InvocationTypeBuilder { get; }
        IProxyEventBuilder ProxyEventBuilder { get; }
        IProxyMethodBuilder ProxyMethodBuilder { get; }
        IProxyPropertyBuilder ProxyPropertyBuilder { get; }
    }
}
