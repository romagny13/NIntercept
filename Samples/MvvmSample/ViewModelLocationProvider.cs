using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmSample
{

    public class ViewModelLocationProvider
    {
        private readonly static Dictionary<Type, Type> customRegistrations;
        private readonly static Dictionary<Type, Type> viewTypeToViewModelTypeCache;
        private static Func<Type, Type> defaultViewTypeToViewModelTypeResolver;
        private static Func<Type, Type> viewTypeToViewModelTypeResolver;
        private static Func<Type, object> viewModelFactory;

        static ViewModelLocationProvider()
        {
            customRegistrations = new Dictionary<Type, Type>();
            viewTypeToViewModelTypeCache = new Dictionary<Type, Type>();

            // By Type or feature
            defaultViewTypeToViewModelTypeResolver = new Func<Type, Type>(viewType =>
            {
                var viewTypeFullName = viewType.FullName; // "Sample.Views.MainWindow"
                Assembly assembly = viewType.Assembly; // the assembly that contains ViewModels

                // resolve ViewModel Type Full Name => "Sample.ViewModels.MainWindowViewModel"
                var viewModelTypeFullName = viewTypeFullName.Replace(".Views.", ".ViewModels."); // ignored by feature
                viewModelTypeFullName = viewTypeFullName.EndsWith("View") ? $"{viewModelTypeFullName}Model" : $"{viewModelTypeFullName}ViewModel";

                // use the assembly directly to avoid problems with Type.GetType and AssemblyQualifiedName
                Type viewModelType = assembly.GetType(viewModelTypeFullName);
                return viewModelType;
            });

            SetViewModelFactoryToDefault();
        }

        public static void SetViewModelFactory(Func<Type, object> viewModelFactory)
        {
            if (viewModelFactory == null)
                throw new ArgumentNullException(nameof(viewModelFactory));

            ViewModelLocationProvider.viewModelFactory = viewModelFactory;
        }

        public static void ResetConvention()
        {
            viewTypeToViewModelTypeResolver = null;
        }

        public static void SetViewModelFactoryToDefault()
        {
            viewModelFactory = new Func<Type, object>(viewModelType => Activator.CreateInstance(viewModelType));
        }

        public static void ChangeConvention(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            if (viewTypeToViewModelTypeResolver == null)
                throw new ArgumentNullException(nameof(viewTypeToViewModelTypeResolver));

            ViewModelLocationProvider.viewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        public static void RegisterCustom(Type viewType, Type viewModelType)
        {
            if (viewType == null)
                throw new ArgumentNullException(nameof(viewType));
            if (viewModelType == null)
                throw new ArgumentNullException(nameof(viewModelType));

            customRegistrations[viewType] = viewModelType;
        }

        public static Type ResolveViewModelType(Type viewType)
        {
            if (viewType is null)
                throw new ArgumentNullException(nameof(viewType));

            Type viewModelType = null;
            if (!viewTypeToViewModelTypeCache.TryGetValue(viewType, out viewModelType))
            {
                if (customRegistrations.TryGetValue(viewType, out Type customRegistration))
                    viewModelType = customRegistration;
                else
                {
                    if (viewTypeToViewModelTypeResolver != null)
                        viewModelType = viewTypeToViewModelTypeResolver(viewType);
                    else
                        viewModelType = defaultViewTypeToViewModelTypeResolver(viewType);
                }

                if (viewModelType != null)
                    viewTypeToViewModelTypeCache[viewType] = viewModelType;
            }
            return viewModelType;
        }

        public static object CreateViewModelInstance(Type viewModelType)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            var viewModel = viewModelFactory(viewModelType);
            return viewModel;
        }
    }
}
