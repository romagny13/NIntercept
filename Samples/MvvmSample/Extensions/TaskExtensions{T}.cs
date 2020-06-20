namespace System.Threading.Tasks
{
    public static class MvvmLibraryTaskExtensionsT
    {
        private static async void HandleAwait<T>(Task<T> task, bool continueOnCapturedContext, Action<T> onCompleted, Action<Exception> onException)
        {
            try
            {
                T result = await task.ConfigureAwait(continueOnCapturedContext);
                onCompleted?.Invoke(result);
            }
            catch (Exception ex)
            {
                HandleException(ex, onException);
            }
        }

        private static void HandleException(Exception ex, Action<Exception> onException)
        {
            TaskExtensionsHelper.HandleException(ex, onException);
        }

        public static void Await<T>(this Task<T> task)
        {
            HandleAwait(task, false, null, null);
        }

        public static void Await<T>(this Task<T> task, bool continueOnCapturedContext)
        {
            HandleAwait(task, continueOnCapturedContext, null, null);
        }

        public static void Await<T>(this Task<T> task, Action<Exception> onException)
        {
            HandleAwait(task, false, null, onException);
        }

        public static void Await<T>(this Task<T> task, Action<T> onCompleted, Action<Exception> onException)
        {
            HandleAwait(task, false, onCompleted, onException);
        }

        public static void Await<T>(this Task<T> task, bool continueOnCapturedContext, Action<T> onCompleted, Action<Exception> onException)
        {
            HandleAwait(task, continueOnCapturedContext, onCompleted, onException);
        }

    }
}
