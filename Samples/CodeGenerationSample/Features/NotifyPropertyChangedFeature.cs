using NIntercept;
using NIntercept.Definition;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeGenerationSample
{
    public class NotifyPropertyChangedFeature
    {
        private static readonly ConstructorInfo PropertyChangedEventArgsConstructor = typeof(PropertyChangedEventArgs).GetConstructor(new Type[] { typeof(string) });
        private static readonly MethodInfo InvokeMethod = typeof(PropertyChangedEventHandler).GetMethod("Invoke", new Type[] { typeof(object), typeof(PropertyChangedEventArgs) });

        public const string SetConstant = "set_";
        public const string FieldName = "_propertyChanged";
        public const string EventName = "PropertyChanged";
        public const string ParameterName = "propertyName";
        public const string MethodName = "OnPropertyChanged";
        private MethodBuilder onPropertyChangedMethodBuilder;

        public MethodBuilder OnPropertyChangedMethodBuilder
        {
            get { return onPropertyChangedMethodBuilder; }
        }

        public void ImplementFeature(TypeBuilder typeBuilder, ProxyTypeDefinition typeDefinition)
        {
            typeBuilder.AddInterfaceImplementation(typeof(INotifyPropertyChanged));

            FieldBuilder eventFieldBuilder = typeBuilder.DefineField(FieldName, typeof(PropertyChangedEventHandler), FieldAttributes.Private);
            EventBuilder propertyChangedEventBuilder = typeBuilder.DefineFullEvent(EventName, EventAttributes.None, typeof(PropertyChangedEventHandler), eventFieldBuilder);

            onPropertyChangedMethodBuilder = typeBuilder.DefineMethod(MethodName, MethodAttributes.Family, typeof(void), new Type[] { typeof(string) });

            onPropertyChangedMethodBuilder.DefineParameter(1, ParameterAttributes.None, ParameterName);

            var il = onPropertyChangedMethodBuilder.GetILGenerator();

            var isNullLocal = il.DeclareLocal(typeof(bool));
            var label = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, eventFieldBuilder);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc, isNullLocal);
            il.Emit(OpCodes.Ldloc, isNullLocal);
            il.Emit(OpCodes.Brfalse_S, label);

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, eventFieldBuilder);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, PropertyChangedEventArgsConstructor);
            il.Emit(OpCodes.Callvirt, InvokeMethod);

            il.MarkLabel(label);

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);
        }

        public bool IsMethodSet(string methodName)
        {
            return methodName.StartsWith(SetConstant);
        }

        public string GetPropertyName(string methodName)
        {
            return methodName.Substring(SetConstant.Length);
        }

        public bool InvokeOnPropertyChanged(ILGenerator il, string methodName)
        {
            if (!IsMethodSet(methodName))
                return false;

            string propertyName = GetPropertyName(methodName);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, propertyName);
            il.Emit(OpCodes.Call, onPropertyChangedMethodBuilder);

            return true;
        }
    }

   
}
