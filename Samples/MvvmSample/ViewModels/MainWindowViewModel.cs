using NIntercept;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmSample.ViewModels
{

    public class MainWindowViewModel
    {
        private DelegateCommand updateTitleCommand;

        [PropertyChanged]
        public virtual bool IsBusy { get; set; }

        public MainWindowViewModel()
        {
            Title = "Main title";
        }

        [PropertyChanged]
        public virtual string Title { get; set; }

        public DelegateCommand UpdateTitleCommand
        {
            get
            {
                if (updateTitleCommand == null)
                    updateTitleCommand = new DelegateCommand(ExecuteUpdateTitleCommand);
                return updateTitleCommand;
            }
        }

        [MethodInterceptor(typeof(MyAsyncInterceptor))]
        [MethodInterceptor(typeof(CanExecuteCommandInterceptor))]
        protected virtual void ExecuteUpdateTitleCommand()
        {
            Title += "!";
        }
    }

    public class MyAsyncInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var context = invocation.GetAwaitableContext();

            var viewModel = invocation.Proxy as MainWindowViewModel;
            if (viewModel != null)
                viewModel.IsBusy = true;

            Task.Delay(3000).Await(() =>
            {
                if (viewModel != null)
                    viewModel.IsBusy = false;
                context.Proceed();
            }, ex => { });
        }
    }

    public class CanExecuteCommandInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (MessageBox.Show("Can Execute?", $"Execute '{invocation.Member.Name}'", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                invocation.Proceed();
            else
                MessageBox.Show("Cancelled.", $"Execute '{invocation.Member.Name}'", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }

    public interface INotifyPropertyChangedMixin : INotifyPropertyChanged
    {
        void OnPropertyChanged(object target, string propertyName);
    }

    public class PropertyChangedMixin : INotifyPropertyChangedMixin
    {
        public void OnPropertyChanged(object target, string propertyName)
        {
            PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
    public class PropertyChangedAttribute : Attribute, ISetterInterceptorProvider
    {
        public Type InterceptorType => typeof(PropertyChangedInterceptor);
    }

    public class PropertyChangedInterceptor : Interceptor
    {
        protected override void OnEnter(IInvocation invocation) { }
        protected override void OnException(IInvocation invocation, Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
        }

        protected override void OnExit(IInvocation invocation)
        {
            INotifyPropertyChangedMixin mixin = invocation.Proxy as INotifyPropertyChangedMixin;
            if (mixin != null)
            {
                string propertyName = invocation.Member.Name;
                mixin.OnPropertyChanged(invocation.Proxy, propertyName);
            }
        }
    }
}
