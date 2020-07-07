using NIntercept;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeGenerationSample
{

    public class NotifyPropertyChangedFeature
    {
        private static readonly ConstructorInfo PropertyChangedEventArgsConstructor = typeof(PropertyChangedEventArgs).GetConstructor(new Type[] { typeof(string) });
        private static readonly MethodInfo InvokeMethod = typeof(PropertyChangedEventHandler).GetMethod("Invoke", new Type[] { typeof(object), typeof(PropertyChangedEventArgs) });

        private static Type interfaceType = typeof(INotifyPropertyChanged);

        public const string SetConstant = "set_";
        public const string FieldName = "_propertyChanged";
        public const string EventName = "PropertyChanged";
        public const string ParameterName = "propertyName";
        public const string MethodName = "OnPropertyChanged";

        public bool HasINotifyPropertyChangedInterface(TypeBuilder typeBuilder)
        {
            return typeBuilder.ImplementedInterfaces.FirstOrDefault(p => p.UnderlyingSystemType == typeof(INotifyPropertyChanged)) != null;
        }

        public void ImplementFeature(ProxyScope proxyScope)
        {
            // interface
            AddINotifyPropertyChangedInterface(proxyScope);

            // event
            CreatePropertyChangedEventHandler(proxyScope);

            // Method
            CreateOnPropertyChangedMethod(proxyScope);
        }

        public void AddINotifyPropertyChangedInterface(ProxyScope proxyScope)
        {
            if (!proxyScope.HasImplementedInterface(interfaceType))
                proxyScope.AddInterfaceImplementation(interfaceType);
        }

        public void CreatePropertyChangedEventHandler(ProxyScope proxyScope)
        {
            EventBuilder propertyChangedEventBuilder = proxyScope.Events.FirstOrDefault(p => p.GetName() == EventName);
            if (propertyChangedEventBuilder == null)
            {
                FieldBuilder eventField = proxyScope.DefineField(FieldName, typeof(PropertyChangedEventHandler), FieldAttributes.Private);
                proxyScope.DefineFullEvent(EventName, EventAttributes.None, typeof(PropertyChangedEventHandler), eventField);
            }
        }

        public void CreateOnPropertyChangedMethod(ProxyScope proxyScope)
        {
            FieldBuilder eventField = proxyScope.Fields.FirstOrDefault(p => p.Name == FieldName);
            if (eventField == null)
                throw new ArgumentException($"No field '{FieldName}' found.");

            var onPropertyChangedMethodBuilder = GetOnPropertyChangedMethod(proxyScope);
            if (onPropertyChangedMethodBuilder == null)
            {
                onPropertyChangedMethodBuilder = proxyScope.DefineMethod(MethodName, MethodAttributes.Family, typeof(void), new Type[] { typeof(string) });

                onPropertyChangedMethodBuilder.DefineParameter(1, ParameterAttributes.None, ParameterName);

                var il = onPropertyChangedMethodBuilder.GetILGenerator();

                var isNullLocal = il.DeclareLocal(typeof(bool));
                var label = il.DefineLabel();

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, eventField);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Cgt_Un);
                il.Emit(OpCodes.Stloc, isNullLocal);
                il.Emit(OpCodes.Ldloc, isNullLocal);
                il.Emit(OpCodes.Brfalse_S, label);

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, eventField);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Newobj, PropertyChangedEventArgsConstructor);
                il.Emit(OpCodes.Callvirt, InvokeMethod);

                il.MarkLabel(label);

                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ret);
            }
        }

        public bool IsSetMethod(string methodName)
        {
            return methodName.StartsWith(SetConstant);
        }

        public string GetPropertyName(string methodName)
        {
            return methodName.Substring(SetConstant.Length);
        }

        public bool CheckEquals(ProxyScope proxyScope, ILGenerator il, MethodInfo method)
        {
            if (!IsSetMethod(method.Name))
                return false;

            string propertyName = GetPropertyName(method.Name);
            var equalLocalBuilder = il.DeclareLocal(typeof(bool));
            var equalLabel = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, method.DeclaringType.GetMethod($"get_{propertyName}"));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ceq);

            il.Emit(OpCodes.Stloc, equalLocalBuilder);
            il.Emit(OpCodes.Ldloc, equalLocalBuilder);

            il.Emit(OpCodes.Brfalse_S, equalLabel);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(equalLabel);

            return true;
        }

        public bool InvokeOnPropertyChanged(ProxyScope proxyScope, ILGenerator il, MethodInfo method)
        {
            if (!IsSetMethod(method.Name))
                return false;

            string propertyName = GetPropertyName(method.Name);
            MethodBuilder onPropertyChangedMethodBuilder = GetOnPropertyChangedMethod(proxyScope);
            if (onPropertyChangedMethodBuilder == null)
                throw new ArgumentNullException(nameof(onPropertyChangedMethodBuilder));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, propertyName);
            il.Emit(OpCodes.Call, onPropertyChangedMethodBuilder);

            return true;
        }

        public MethodBuilder GetOnPropertyChangedMethod(ProxyScope proxyScope)
        {
            return proxyScope.Methods.FirstOrDefault(p => p.Name == MethodName);
        }
    }


}
