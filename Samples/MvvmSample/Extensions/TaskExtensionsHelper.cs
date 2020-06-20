namespace System.Threading.Tasks
{
    public class TaskExtensionsHelper
    {
        private static Action<Exception> _onException;

        internal static void HandleException(Exception ex, Action<Exception> onException)
        {
            onException?.Invoke(ex);
            _onException?.Invoke(ex);
        }

        public static void SetDefaultExceptionHandling(in Action<Exception> onException)
        {
            if (onException is null)
                throw new ArgumentNullException(nameof(onException));

            _onException = onException;
        }

        public static void RemoveDefaultExceptionHandling()
        {
            _onException = null;
        }
    }
}
