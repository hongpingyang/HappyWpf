using HappyCore;
using HappyCore.Events;
using System;

namespace HappyTest.ViewModels
{
    class UserControl3ViewModel : BindableBase
    {
        IEventAggregator eventAggregator;
        private string _text = "";
        public UserControl3ViewModel()
        {
            eventAggregator = ServiceLocator.instance.Resolve<IEventAggregator>();
            eventAggregator.Subscribe<string>("测试", ShowStr, new Predicate<string>(s => { if (s == "点击次数:12") return false; else return true; }));
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private void ShowStr(string str)
        {
            Text = str;
            if (str == "测试:20")
                eventAggregator.UnSubscribe("测试");
        }
    }
}
