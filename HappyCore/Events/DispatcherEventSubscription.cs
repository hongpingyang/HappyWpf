using System;
using System.Threading;
using System.Windows.Threading;

namespace HappyCore.Events
{
    public class DispatcherEventSubscription:EventSubscription
    {
        public  Dispatcher UIDispatcher;

        public DispatcherEventSubscription(IDelegateReference actionReference)
            :base(actionReference)
        {
            UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        public override void InvokeAction(Action action)
        {
            if (UIDispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                UIDispatcher.BeginInvoke(action, null);
            }
        }
    }

    public class DispatcherEventSubscription<T> : EventSubscription<T>
    {
        public  Dispatcher UIDispatcher;

        public DispatcherEventSubscription(IDelegateReference actionReference,IDelegateReference filterReference)
            : base(actionReference,filterReference)
        {
            UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        public override void InvokeAction(Action<T> action, T argument)
        {
            if (UIDispatcher.CheckAccess())
            {
                action(argument);
            }
            else
            {
                UIDispatcher.BeginInvoke((Action<T>)((o) => action((T)o)), argument);
            }
        }
    }
}

