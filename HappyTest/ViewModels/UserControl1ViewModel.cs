using System.Windows.Input;
using HappyCore;
using HappyCore.Events;

namespace HappyTest.ViewModels
{
    public class UserControl1ViewModel : BindableBase
    {
        IEventAggregator eventAggregator;
        int clicktimes = 0;
        private string _text = "";

        public UserControl1ViewModel()
        {
            eventAggregator = ServiceLocator.instance.Resolve<IEventAggregator>();
        }
        
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public ICommand BtnClick
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CommandParameter)
                    {
                        string  str = ((CommandParameter)o).Parameter as string;
                        if (str == null)
                            return;
                        Trigger_Click(str);
                    }
                });
            }
        }

        private void Trigger_Click(string str)
        {
            clicktimes++;
            eventAggregator.Publish("测试", str+":" + clicktimes);
        }
    }
}
