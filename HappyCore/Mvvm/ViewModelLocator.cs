using System.Windows;

namespace HappyCore
{
    public static class ViewModelLocator
    {
        /// <summary>
        /// 自动定位ViewModel
        /// </summary>
        public static DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", 
            typeof(bool), typeof(ViewModelLocator), 
            new PropertyMetadata(false, AutoWireViewModelChanged));
        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }
        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ViewModelLocationProvider.AutoWireViewModelChanged(d, Bind);
        }
        /// <summary>
        /// 设置 DataContext
        /// </summary>
        /// <param name="view">The View to set the DataContext on</param>
        /// <param name="viewModel">The object to use as the DataContext for the View</param>
        private static void Bind(object view, object viewModel)
        {
            FrameworkElement element = view as FrameworkElement;
            if (element != null)
                element.DataContext = viewModel;
        }
    }
}
