using HappyCore;
using HappyCore.Events;

namespace HappyTest.ViewModels
{
    class UserControl2ViewModel : BindableBase
    {
        IEventAggregator eventAggregator;
        private string _text = "";

        public UserControl2ViewModel()
        {
            eventAggregator = ServiceLocator.instance.Resolve<IEventAggregator>();
            eventAggregator.Subscribe<string>("测试", ShowStr);
        }


        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private void ShowStr(string str)
        {
            Text = str;
            if (str == "测试:10")
                eventAggregator.UnSubscribe<string>("测试", ShowStr);
        }
    }
}
