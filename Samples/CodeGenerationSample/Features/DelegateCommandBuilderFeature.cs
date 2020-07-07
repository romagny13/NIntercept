using NIntercept;
using NIntercept.Definition;
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
        private string commandName;

        public CommandAttribute(string commandName)
        {
            this.commandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        }

        public string CommandName
        {
            get { return commandName; }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CanExecuteCommandAttribute : Attribute
    {
        private string commandName;

        public CanExecuteCommandAttribute(string commandName)
        {
            this.commandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        }

        public string CommandName
        {
            get { return commandName; }
        }
    }

    public class DelegateCommandBuilderFeature
    {
        public void ImplementFeature(ProxyScope proxyScope)
        {
            ProxyTypeDefinition typeDefinition = proxyScope.TypeDefinition;

            var commands = GetCommandsToCreate(typeDefinition);
            foreach (var command in commands)
            {
                if(command.IsGenericCommand)
                { }
                else
                {
                    CreateDelegateCommand(proxyScope, command.CommandName, command.ExecuteMethod);
                }

                // can execute method ?
            }
        }

        protected virtual void CreateDelegateCommand(ProxyScope proxyScope, string commandName, MethodInfo executeMethod)
        {
            PropertyBuilder propertyBuilder = proxyScope.DefineProperty(commandName, PropertyAttributes.None, typeof(DelegateCommand), new Type[0]);

            FieldBuilder field = proxyScope.DefineField($"_{NamingHelper.ToCamelCase(commandName)}", typeof(DelegateCommand), FieldAttributes.Private);

            // get method
            MethodBuilder methodBuilder = proxyScope.DefineMethod($"get_{commandName}", MethodAttributes.Public | MethodAttributes.HideBySig, typeof(DelegateCommand), new Type[0]);

            // body
            var il = methodBuilder.GetILGenerator();

            var isNullLocalBuilder = il.DeclareLocal(typeof(bool));
            var commandLocalBuilder = il.DeclareLocal(typeof(DelegateCommand));
            var isNullLabel = il.DefineLabel();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc, isNullLocalBuilder);

            il.Emit(OpCodes.Ldloc, isNullLocalBuilder);

            il.Emit(OpCodes.Brfalse_S, isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, executeMethod);
            il.Emit(OpCodes.Newobj, typeof(Action).GetConstructors()[0]);

            il.Emit(OpCodes.Newobj, typeof(DelegateCommand).GetConstructor(new Type[] { typeof(Action) }));
            il.Emit(OpCodes.Stfld, field);

            il.MarkLabel(isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc, commandLocalBuilder);
            il.Emit(OpCodes.Ldloc, commandLocalBuilder);
            il.Emit(OpCodes.Ret);

            // set property method
            propertyBuilder.SetGetMethod(methodBuilder);
        }

        protected virtual List<CommandInfo> GetCommandsToCreate(ProxyTypeDefinition typeDefinition)
        {
            var methods = typeDefinition.Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.DeclaringType != typeof(object) && !method.IsSpecialName).ToArray();

            // execute method
            var executeMethods = GetCommands(methods);
            var canExecuteMethods = GetCanExecuteCommands(methods);

            List<CommandInfo> commands = new List<CommandInfo>();

            foreach (var executeMethod in executeMethods)
            {
                string commandName = executeMethod.Key;
                MethodInfo canExecuteMethod = null;
                canExecuteMethods.TryGetValue(commandName, out canExecuteMethod);
                commands.Add(new CommandInfo(commandName, executeMethod.Value, canExecuteMethod));
            }

            return commands;
        }

        private Dictionary<string, MethodInfo> GetCommands(MethodInfo[] methods)
        {
            var attributes = new Dictionary<string, MethodInfo>();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    // check exists ?
                    attributes.Add(attribute.CommandName, method);
                }
            }
            return attributes;
        }

        private Dictionary<string, MethodInfo> GetCanExecuteCommands(MethodInfo[] methods)
        {
            var attributes = new Dictionary<string, MethodInfo>();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CanExecuteCommandAttribute>();
                if (attribute != null)
                {
                    // check exists ?
                    attributes.Add(attribute.CommandName, method);
                }
            }
            return attributes;
        }
    }

    public class CommandInfo
    {
        private string commandName;
        private MethodInfo executeMethod;
        private MethodInfo canExecuteMethod;

        public CommandInfo(string commandName, MethodInfo executeMethod, MethodInfo canExecuteMethod)
        {
            if (commandName is null)
                throw new ArgumentNullException(nameof(commandName));
            if (executeMethod is null)
                throw new ArgumentNullException(nameof(executeMethod));

            this.commandName = commandName;
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public bool IsGenericCommand
        {
            get { return executeMethod.IsGenericMethod; }
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
    }
}
