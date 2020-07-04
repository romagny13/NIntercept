using System;
using System.ComponentModel;
using System.Windows;

namespace CodeGenerationSample
{
    public class ViewModelLocator
    {
        public static readonly DependencyProperty AutoWireViewModelProperty;

        static ViewModelLocator()
        {
            AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, OnAuoWireViewModelChanged));
        }

        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        private static void OnAuoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
                return;

            bool autoWireViewModel = (bool)e.NewValue;
            if (autoWireViewModel)
            {
                var frameworkElement = d as FrameworkElement;
                if (frameworkElement == null)
                    throw new ArgumentException($"AutoWireViewModel Attached Property require a FrameworkElement. Type '{d.GetType()}'");

                ResolveViewModel(frameworkElement);
            }
        }

        private static void ResolveViewModel(FrameworkElement frameworkElement)
        {
            if (frameworkElement is null)
                throw new ArgumentNullException(nameof(frameworkElement));

            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(frameworkElement.GetType());
            if (viewModelType != null)
            {
                var viewModel = ViewModelLocationProvider.CreateViewModelInstance(viewModelType);
                frameworkElement.DataContext = viewModel;
            }
            else
            {
                throw new ArgumentException($"Not ViewModelType found for'{frameworkElement.GetType()}'");
            }
        }
    }
}
