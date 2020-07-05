using Prism.Commands;
using System.Threading.Tasks;

namespace CodeGenerationSample.ViewModels
{
    public class MainWindowViewModel
    {
       // private DelegateCommand updateTitleCommand;

        public virtual bool IsBusy { get; set; }

        public MainWindowViewModel()
        {
            Title = "Main title";
        }

        public virtual string Title { get; set; }

        //public DelegateCommand UpdateTitleCommand
        //{
        //    get
        //    {
        //        if (updateTitleCommand == null)
        //            updateTitleCommand = new DelegateCommand(ExecuteUpdateTitleCommand);
        //        return updateTitleCommand;
        //    }
        //}

        [Command("UpdateTitleCommand")]
        protected async void ExecuteUpdateTitleCommand()
        {
            IsBusy = true;

            await Task.Delay(2000).ConfigureAwait(false);

            Title += "!";

            IsBusy = false;
        }
    }
}
