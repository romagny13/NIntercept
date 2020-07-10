using Prism.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CodeGenerationSample.ViewModels
{
    [ViewModel]
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Title = "The title";
        }

        public virtual string Title { get; set; }
        public virtual bool CanExecute { get; set; }
        public virtual bool IsBusy { get; set; }

        //private DelegateCommand<string> updateTitleGenericCommand;
        //public DelegateCommand<string> UpdateTitleGenericCommand
        //{
        //    get
        //    {
        //        if (updateTitleGenericCommand == null)
        //            updateTitleGenericCommand = new DelegateCommand<string>(ExecuteUpdateTitleGenericCommand).ObservesCanExecute(() => CanExecute);
        //        return updateTitleGenericCommand;
        //    }
        //}


        [Command("UpdateTitleCommand", CanExecuteMethodName = nameof(CanExecuteUpdateTitle), CanExecutePropertyNames = new[] { nameof(CanExecute) })]
        protected async void ExecuteUpdateTitleCommand()
        {
            IsBusy = true;

            await Task.Delay(2000).ConfigureAwait(false);

            Title += "!";

            IsBusy = false;
        }

        [Command("UpdateTitleGenericCommand", CanExecuteMethodName = nameof(CanExecuteUpdateGenericTitle), CanExecutePropertyNames = new[] { nameof(CanExecute) })]
        protected async void ExecuteUpdateTitleGenericCommand(string title)
        {
            IsBusy = true;

            await Task.Delay(2000).ConfigureAwait(false);

            Title = title;

            IsBusy = false;
        }

        protected bool CanExecuteUpdateTitle()
        {
            return CanExecute; 
        }

        protected bool CanExecuteUpdateGenericTitle(string title)
        {
            return CanExecute; 
        }
    }
}
