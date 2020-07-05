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

    public class DelegateCommandBuilderFeature
    {
        public void ImplementFeature(ProxyScope proxyScope)
        {
            TypeBuilder typeBuilder = proxyScope.TypeBuilder;
            ProxyTypeDefinition typeDefinition = proxyScope.TypeDefinition;

            Dictionary<string, MethodInfo> commands = GetCommandsToCreate(typeDefinition);
            foreach (var kv in commands)
            {
                CreateDelegateCommand(typeBuilder, kv.Key, kv.Value);
            }      
        }

        protected virtual void CreateDelegateCommand(TypeBuilder typeBuilder, string commandName, MethodInfo method)
        {
            FieldBuilder commandFieldBuilder = typeBuilder.DefineField($"_{NamingHelper.ToCamelCase(commandName)}", typeof(DelegateCommand), FieldAttributes.Private);

            MethodBuilder get_MyCommandMethodBuilder = typeBuilder.DefineMethod($"get_{commandName}", MethodAttributes.Public | MethodAttributes.HideBySig, typeof(DelegateCommand), new Type[0]);

            var il = get_MyCommandMethodBuilder.GetILGenerator();

            var isNullLocalBuilder = il.DeclareLocal(typeof(bool));
            var commandLocalBuilder = il.DeclareLocal(typeof(DelegateCommand));

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, commandFieldBuilder);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Stloc, isNullLocalBuilder);

            il.Emit(OpCodes.Ldloc, isNullLocalBuilder);

            var isNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, method);
            il.Emit(OpCodes.Newobj, typeof(Action).GetConstructors()[0]);

            il.Emit(OpCodes.Newobj, typeof(DelegateCommand).GetConstructor(new Type[] { typeof(Action) }));
            il.Emit(OpCodes.Stfld, commandFieldBuilder);

            il.MarkLabel(isNullLabel);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, commandFieldBuilder);

            il.Emit(OpCodes.Stloc, commandLocalBuilder);

            il.Emit(OpCodes.Ldloc, commandLocalBuilder);
            il.Emit(OpCodes.Ret);

            PropertyBuilder commandPropertyBuilder = typeBuilder.DefineProperty(commandName, PropertyAttributes.None, typeof(DelegateCommand), new Type[0]);
            commandPropertyBuilder.SetGetMethod(get_MyCommandMethodBuilder);
        }

        protected virtual Dictionary<string, MethodInfo> GetCommandsToCreate(ProxyTypeDefinition typeDefinition)
        {
            var methods = typeDefinition.Type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(method => method.DeclaringType != typeof(object) && !method.IsSpecialName);

            Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    commands.Add(attribute.CommandName, method);
                }
            }

            return commands;
        }
    }
}
