using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeGenerationSample
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewModelAttribute : Attribute
    {

    }

    public class ViewModelSupport
    {
        private static readonly Dictionary<Type, bool> typeCache;
        private static readonly Dictionary<Type, List<CommandInfo>> commandCache;

        static ViewModelSupport()
        {
            typeCache = new Dictionary<Type, bool>();
            commandCache = new Dictionary<Type, List<CommandInfo>>();
        }

        public static bool IsViewModel(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            bool isViewModel = false;
            if (!typeCache.TryGetValue(type, out isViewModel))
            {
                var attribute = type.GetCustomAttribute<ViewModelAttribute>();
                isViewModel = attribute != null;
                typeCache.Add(type, isViewModel);
            }
            return isViewModel;
        }

        public static List<CommandInfo> GetCommandInfos(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            List<CommandInfo> commands = null;
            if (commandCache.TryGetValue(type, out commands))
            {
                return commands;
            }

            commands = new List<CommandInfo>();

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
               .Where(method => method.DeclaringType != typeof(object) && !method.IsSpecialName).ToArray();
            var properties = type.GetProperties();

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    MethodInfo canExecuteMethod = null;
                    if (attribute.CanExecuteMethodName != null)
                    {
                        canExecuteMethod = methods.FirstOrDefault(p => p.Name == attribute.CanExecuteMethodName);
                        if (canExecuteMethod == null)
                            throw new ArgumentException($"Cannot find the method for the CanExecuteMethodName '{attribute.CanExecuteMethodName}'");
                    }
                    commands.Add(new CommandInfo(type, attribute.CommandName, method, canExecuteMethod, attribute.CanExecutePropertyNames));
                }
            }

            commandCache.Add(type, commands);

            return commands;
        }

    }
}
