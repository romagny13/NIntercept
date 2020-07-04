using NIntercept;
using NIntercept.Definition;
using System.Reflection.Emit;

namespace CodeGenerationSample
{
    public class ViewModelCodeGenerator : CodeGenerator
    {
        private NotifyPropertyChangedFeature notifyPropertyChangedFeature;
        private DelegateCommandBuilderFeature delegateCommandBuilderFeature;

        public ViewModelCodeGenerator()
        {
            notifyPropertyChangedFeature = new NotifyPropertyChangedFeature();
            delegateCommandBuilderFeature = new DelegateCommandBuilderFeature();
        }

        public override void Define(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            notifyPropertyChangedFeature.ImplementFeature(typeBuilder, typeDefinition);
            delegateCommandBuilderFeature.ImplementFeature(typeBuilder, typeDefinition);
        }

        public override void AfterInvoke(ILGenerator il, TypeBuilder typeBuilder, CallbackMethodDefinition callbackMethodDefinition, FieldBuilder[] fields)
        {
            string methodName = callbackMethodDefinition.Method.Name;
            notifyPropertyChangedFeature.InvokeOnPropertyChanged(il, methodName);
        }
    }
}
