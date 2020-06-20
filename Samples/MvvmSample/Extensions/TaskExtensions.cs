namespace System.Threading.Tasks
{

    public static class MvvmLibraryTaskExtensions
    {
        private static async void HandleAwait(Task task, bool continueOnCapturedContext, Action onCompleted, Action<Exception> onException)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
                onCompleted?.Invoke();
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

        public static void Await(this Task task)
        {
            HandleAwait(task, false, null, null);
        }

        public static void Await(this Task task, bool continueOnCapturedContext)
        {
            HandleAwait(task, continueOnCapturedContext, null, null);
        }

        public static void Await(this Task task, Action<Exception> onException)
        {
            HandleAwait(task, false, null, onException);
        }

        public static void Await(this Task task, Action onCompleted, Action<Exception> onException)
        {
            HandleAwait(task, false, onCompleted, onException);
        }

        public static void Await(this Task task, bool continueOnCapturedContext, Action onCompleted, Action<Exception> onException)
        {
            HandleAwait(task, continueOnCapturedContext, onCompleted, onException);
        }
    }
}
