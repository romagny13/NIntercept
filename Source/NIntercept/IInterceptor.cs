namespace NIntercept
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}
