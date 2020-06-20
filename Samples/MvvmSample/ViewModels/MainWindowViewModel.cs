using NIntercept;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmSample.ViewModels
{
    public interface IPropertyChangedAware : INotifyPropertyChanged
    {
        void OnPropertyChanged(string propertyName);
    }

    // [AllInterceptor(typeof(AllPropertyChangedInterceptor))]
    public class MainWindowViewModel : IPropertyChangedAware
    {
        private DelegateCommand updateTitleCommand;

        [PropertyChanged]
        public virtual bool IsBusy { get; set; }

        public MainWindowViewModel()
        {
            Title = "Main title";
        }

        //[PropertySetInterceptor(typeof(CanExecuteCommandInterceptor))]
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            {
                invocation.Proceed();
            }
            else
            {
                MessageBox.Show("Cancelled.", $"Execute '{invocation.Member.Name}'", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
    public class PropertyChangedAttribute : Attribute, IPropertySetInterceptorProvider
    {
        public Type InterceptorType => typeof(PropertyChangedInterceptor);
    }

    // property targetted
    public class PropertyChangedInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var inpc = invocation.Proxy as IPropertyChangedAware;
            if (inpc != null)
            {
                string propertyName = invocation.Member.Name;
                inpc.OnPropertyChanged(propertyName);
            }
        }
    }


    // for all properties of class
    //public class AllPropertyChangedInterceptor2 : IInterceptor
    //{
    //    public void Intercept(IInvocation invocation)
    //    {
    //        invocation.Proceed();

    //        var method = invocation.Method;
    //        if (method.Name.StartsWith(SetPrefix))
    //        {
    //            var inpc = invocation.Proxy as IPropertyChangedAware;
    //            if (inpc != null)
    //                inpc.OnPropertyChanged(invocation.Method.Name);
    //        }
    //    }
    //}

}
