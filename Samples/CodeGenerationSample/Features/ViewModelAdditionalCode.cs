using NIntercept;
using NIntercept.Definition;
using System.Reflection.Emit;

namespace CodeGenerationSample
{
    public class ViewModelAdditionalCode : AdditionalCode
    {
        private NotifyPropertyChangedFeature notifyPropertyChangedFeature;
        private DelegateCommandBuilderFeature delegateCommandBuilderFeature;

        public ViewModelAdditionalCode()
        {
            notifyPropertyChangedFeature = new NotifyPropertyChangedFeature();
            delegateCommandBuilderFeature = new DelegateCommandBuilderFeature();
        }

        public override void BeforeDefine(ProxyScope proxyScope)
        {
            notifyPropertyChangedFeature.ImplementFeature(proxyScope);
            delegateCommandBuilderFeature.ImplementFeature(proxyScope);
        }

        public override void AfterDefine(ProxyScope proxyScope)
        {
            base.AfterDefine(proxyScope);
        }

        public override void BeforeInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter)
            {
                // never called with clean proxy method builder
                notifyPropertyChangedFeature.CheckEquals(proxyScope, il, callbackMethodDefinition.Method);
            }
        }

        public override void AfterInvoke(ProxyScope proxyScope, ILGenerator il, CallbackMethodDefinition callbackMethodDefinition)
        {
            if (callbackMethodDefinition.MethodDefinition.MethodDefinitionType == MethodDefinitionType.Setter)
            {
                // never called with clean proxy method builder
                notifyPropertyChangedFeature.InvokeOnPropertyChanged(proxyScope, il, callbackMethodDefinition.Method);
            }
        }
    }
}
