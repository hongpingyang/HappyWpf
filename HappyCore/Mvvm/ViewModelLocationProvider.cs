using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace HappyCore
{

    /// <summary>
    /// ViewModel定位器
    /// </summary>
    public static class ViewModelLocationProvider
    {
        /// <summary>
        /// A dictionary that contains all the registered factories for the views.
        /// </summary>
        static Dictionary<string, Func<object>> _funcMapping = new Dictionary<string, Func<object>>();

        /// <summary>
        /// A dictionary that contains all the registered ViewModel types for the views.
        /// </summary>
        static Dictionary<string, Type> _typeMapping = new Dictionary<string, Type>();

        /// <summary>
        /// The default view model factory which provides the ViewModel type as a parameter.
        /// </summary>
        static Func<Type, object> _defaultViewModelFactory = type => Activator.CreateInstance(type);

        /// <summary>
        /// 默认路径
        /// </summary>
        static Func<Type, Type> _defaultViewTypeToViewModelTypeResolver =
            viewType =>
            {
                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
                return Type.GetType(viewModelName);
            };

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory which provides the ViewModel type as a parameter.</param>
        public static void SetDefaultViewModelFactory(Func<Type, object> viewModelFactory)
        {
            _defaultViewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view type to view model type resolver.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The view type to view model type resolver.</param>
        public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Automatically looks up the viewmodel that corresponds to the current view, using two strategies:
        /// It first looks to see if there is a mapping registered for that view, if not it will fallback to the convention based approach.
        /// </summary>
        /// <param name="view">The dependency object, typically a view.</param>
        /// <param name="setDataContextCallback">The call back to use to create the binding between the View and ViewModel</param>
        public static void AutoWireViewModelChanged(object view, Action<object, object> setDataContextCallback)
        {
            // _funcMapping
            object viewModel = GetViewModelForView(view);
            
            if (viewModel == null)
            {
                // TypeMappings
                var viewModelType = GetViewModelTypeForView(view.GetType());

                //默认路径
                if (viewModelType == null)
                    viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

                if (viewModelType == null)
                    return;

                //构造
                viewModel =_defaultViewModelFactory(viewModelType);
            }

            setDataContextCallback(view, viewModel);
        }

        /// <summary>
        /// Gets the view model for the specified view.
        /// </summary>
        /// <param name="view">The view that the view model wants.</param>
        /// <returns>The ViewModel that corresponds to the view passed as a parameter.</returns>
        private static object GetViewModelForView(object view)
        {
            var viewKey = view.GetType().ToString();

            // Mapping of view models base on view type (or instance) goes here
            if (_funcMapping.ContainsKey(viewKey))
                return _funcMapping[viewKey]();

            return null;
        }

        /// <summary>
        /// Gets the ViewModel type for the specified view.
        /// </summary>
        /// <param name="view">The View that the ViewModel wants.</param>
        /// <returns>The ViewModel type that corresponds to the View.</returns>
        private static Type GetViewModelTypeForView(Type view)
        {
            var viewKey = view.ToString();

            if (_typeMapping.ContainsKey(viewKey))
                return _typeMapping[viewKey];

            return null;
        }

        /// <summary>
        /// string/Func
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register<T>(Func<object> factory)
        {
            Register(typeof(T).ToString(), factory);
        }

        /// <summary>
        /// string/Func
        /// </summary>
        /// <param name="viewTypeName">The name of the view type.</param>
        /// <param name="factory">The ViewModel factory.</param>
        public static void Register(string viewTypeName, Func<object> factory)
        {
            _funcMapping[viewTypeName] = factory;
        }

        /// <summary>
        /// string /type
        /// </summary>
        /// <typeparam name="T">The View</typeparam>
        /// <typeparam name="VM">The ViewModel</typeparam>
        public static void Register<T, VM>()
        {
            var viewType = typeof(T);
            var viewModelType = typeof(VM);

            Register(viewType.ToString(), viewModelType);
        }

        /// <summary>
        /// string /type
        /// </summary>
        /// <param name="viewTypeName">The View type name</param>
        /// <param name="viewModelType">The ViewModel type</param>
        public static void Register(string viewTypeName, Type viewModelType)
        {
            _typeMapping[viewTypeName] = viewModelType;
        }
    }
}
