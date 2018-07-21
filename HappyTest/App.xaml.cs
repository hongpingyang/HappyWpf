using HappyCore;
using HappyCore.Events;
using System.Windows;

namespace HappyTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ///注册单例 事件器
            ServiceLocator.instance.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator));

        }
    }
}
