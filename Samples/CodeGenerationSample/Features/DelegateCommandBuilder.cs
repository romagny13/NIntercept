using NIntercept;
using NIntercept.Helpers;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeGenerationSample
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        private static string[] emptyList;
        private readonly string commandName;
        private string canExecuteMethodName;
        private string[] canExecuteProperties;

        static CommandAttribute()
        {
            emptyList = new string[0];
        }

        public CommandAttribute(string commandName)
        {
            if (commandName is null)
                throw new ArgumentNullException(nameof(commandName));

            this.commandName = commandName;
        }

        public string CommandName
        {
            get { return commandName; }
        }

        public string CanExecuteMethodName
        {
            get { return canExecuteMethodName; }
            set { canExecuteMethodName = value; }
        }

        public string[] CanExecutePropertyNames
        {
            get { return canExecuteProperties ?? emptyList; }
            set { canExecuteProperties = value; }
        }
    }

    public class DelegateCommandBuilder
    {
        private static readonly MethodInfo RaiseCanExecuteChangedMethod = typeof(DelegateCommand).GetMethod("RaiseCanExecuteChanged");
        private static readonly ConstructorInfo ActionConstructor = typeof(Action).GetConstructors()[0];
        private static readonly ConstructorInfo FuncBoolConstructor = typeof(Func<bool>).GetConstructors()[0];
        private static readonly ConstructorInfo DelegateCommandWithPredicateConstructor = typeof(DelegateCommand).GetConstructor(new Type[] { typeof(Action), typeof(Func<bool>) });
        private static readonly ConstructorInfo DelegateCommandConstructor = typeof(DelegateCommand).GetConstructor(new Type[] { typeof(Action) });

        public void CreateCommands(ProxyScope proxyScope, IEnumerable<CommandInfo> commands)
        {
            foreach (var command in commands)
                CreateDelegateCommand(proxyScope, command.CommandName, command.ExecuteMethod, command.CanExecuteMethod);
        }

        protected Type GetParameterType(MethodInfo executeMethod)
        {
            var parameters = executeMethod.GetParameters();
            if (parameters.Length > 1)
                throw new ArgumentException($"Invalid execute method '{executeMethod.Name}'");
            if (parameters.Length == 1)
                return parameters[0].ParameterType;

            return null;
        }

        protected virtual MethodBuilder CreateDelegateCommand(ProxyScope proxyScope, string commandName, MethodInfo executeMethod, MethodInfo canExecuteMethod)
        {
            if (commandName is null)
                throw new ArgumentNullException(nameof(commandName));
            if (executeMethod is null)
                throw new ArgumentNullException(nameof(executeMethod));

            Type parameterType = GetParameterType(executeMethod);
            Type delegateCommandType = null;
            ConstructorInfo actionCtor = null;
            if (parameterType == null)
            {
                delegateCommandType = typeof(DelegateCommand);
                actionCtor = ActionConstructor;
            }
            else
            {
                delegateCommandType = typeof(DelegateCommand<>).MakeGenericType(new Type[] { parameterType });
                actionCtor = typeof(Action<>).MakeGenericType(new Type[] { parameterType }).GetConstructors()[0];
            }

            PropertyBuilder propertyBuilder = proxyScope.DefineProperty(commandName, PropertyAttributes.None, delegateCommandType, new Type[0]);

            FieldBuilder field = proxyScope.DefineField($"_{NamingHelper.ToCamelCase(commandName)}", delegateCommandType, FieldAttributes.Private);

            // get method
            MethodBuilder methodBuilder = proxyScope.DefineMethod($"get_{commandName}", MethodAttributes.Public | MethodAttributes.HideBySig, delegateCommandType, new Type[0]);

            // body
            var il = methodBuilder.GetILGenerator();

            var isNullLocal = il.DeclareLocal(typeof(bool));
            var commandLocal = il.DeclareLocal(delegateCommandType);
            var isNullLabel = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc, isNullLocal);

            il.Emit(OpCodes.Ldloc, isNullLocal);

            il.Emit(OpCodes.Brfalse_S, isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, executeMethod);
            il.Emit(OpCodes.Newobj, actionCtor);

            if (canExecuteMethod != null)
            {
                ConstructorInfo delegateCommandWithPredicateCtor = parameterType == null ? DelegateCommandWithPredicateConstructor : delegateCommandType.GetConstructors()[1];
                ConstructorInfo funcCtor = parameterType == null ? FuncBoolConstructor : typeof(Func<,>).MakeGenericType(new Type[] { parameterType, typeof(bool) }).GetConstructors()[0];
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, canExecuteMethod);
                il.Emit(OpCodes.Newobj, funcCtor);
                il.Emit(OpCodes.Newobj, delegateCommandWithPredicateCtor);
            }
            else
            {
                ConstructorInfo delegateCommandCtor = parameterType == null ? DelegateCommandConstructor : delegateCommandType.GetConstructors()[0];
                il.Emit(OpCodes.Newobj, delegateCommandCtor);
            }
            il.Emit(OpCodes.Stfld, field);

            il.MarkLabel(isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc, commandLocal);
            il.Emit(OpCodes.Ldloc, commandLocal);
            il.Emit(OpCodes.Ret);

            // set property method
            propertyBuilder.SetGetMethod(methodBuilder);

            return methodBuilder;
        }

        public void RaiseCanExecuteChangedFor(ProxyScope proxyScope, ILGenerator il, string propertyName)
        {
            if (il is null)
                throw new ArgumentNullException(nameof(il));
            if (propertyName is null)
                throw new ArgumentNullException(nameof(propertyName));

            var commands = ViewModelSupport.GetCommandInfos(proxyScope.TypeDefinition.Type).Where(p => p.CanExecutePropertyNames.Contains(propertyName));
            if (commands.Count() == 0)
                return;

            foreach (var command in commands)
            {
                // find the method builder created for the proxy
                var getMethod = proxyScope.Methods.FirstOrDefault(p => p.Name == $"get_{command.CommandName}");
                if (getMethod == null)
                    throw new ArgumentException($"No method found for the command '{command.CommandName}'");

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, getMethod);
                il.Emit(OpCodes.Callvirt, RaiseCanExecuteChangedMethod);
            }
        }
    }

    public class CommandInfo
    {
        private readonly Type type;
        private string commandName;
        private MethodInfo executeMethod;
        private readonly MethodInfo canExecuteMethod;
        private readonly string[] canExecutePropertyNames;

        public CommandInfo(Type type, string commandName, MethodInfo executeMethod, MethodInfo canExecuteMethod, string[] canExecutePropertyNames)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (commandName is null)
                throw new ArgumentNullException(nameof(commandName));
            if (executeMethod is null)
                throw new ArgumentNullException(nameof(executeMethod));

            this.type = type;
            this.commandName = commandName;
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.canExecutePropertyNames = canExecutePropertyNames;
        }

        public Type Type
        {
            get { return type; }
        }

        public string CommandName
        {
            get { return commandName; }
        }

        public MethodInfo ExecuteMethod
        {
            get { return executeMethod; }
        }

        public MethodInfo CanExecuteMethod
        {
            get { return canExecuteMethod; }
        }

        public string[] CanExecutePropertyNames
        {
            get { return canExecutePropertyNames; }
        }
    }
}
