using NIntercept;
using NIntercept.Definition;
using System.Reflection.Emit;


namespace CodeGenerationSample
{
    public class ViewModelBuilder : AdditionalCode
    {
        private NotifyPropertyChangedFeature notifyPropertyChangedFeature;
        private DelegateCommandBuilder delegateCommandBuilder;

        public ViewModelBuilder()
        {
            notifyPropertyChangedFeature = new NotifyPropertyChangedFeature();
            delegateCommandBuilder = new DelegateCommandBuilder();
        }

        public override void BeforeDefine(ProxyScope proxyScope)
        {
            if (ViewModelSupport.IsViewModel(proxyScope.TypeDefinition.Type))
            {
                notifyPropertyChangedFeature.ImplementFeature(proxyScope);

                var commands = ViewModelSupport.GetCommandInfos(proxyScope.TypeDefinition.Type);
                delegateCommandBuilder.CreateCommands(proxyScope, commands);
            }
        }

        public override void AfterDefine(ProxyScope proxyScope)
        {
            base.AfterDefine(proxyScope);
        }

        public override void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            // never called with clean proxy method builder

            if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter && ViewModelSupport.IsViewModel(proxyScope.TypeDefinition.Type))
            {
                notifyPropertyChangedFeature.CheckEquals(proxyScope, il, callbackMethodDefinition.Method);
            }
        }

        public override void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            // never called with clean proxy method builder

            if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter && ViewModelSupport.IsViewModel(proxyScope.TypeDefinition.Type))
            {
                notifyPropertyChangedFeature.InvokeOnPropertyChanged(proxyScope, il, callbackMethodDefinition.Method);

                string propertyName = (callbackMethodDefinition.MethodDefinition as SetMethodDefinition).PropertyDefinition.Name;
                delegateCommandBuilder.RaiseCanExecuteChangedFor(proxyScope, il, propertyName);
            }
        }
    }
}
