using System;
using System.Threading.Tasks;

namespace HappyCore.Events
{
    public class BackgroundEventSubscription: EventSubscription
    {
        public BackgroundEventSubscription(IDelegateReference actionReference) :
            base(actionReference)
        {

        }

        public override void InvokeAction(Action action)
        {
            Task.Run(action);
        }
    }

    public class BackgroundEventSubscription<T> : EventSubscription<T>
    {

        public BackgroundEventSubscription(IDelegateReference actionReference,IDelegateReference filterReference) :
            base(actionReference, filterReference)
        {

        }

        public override void InvokeAction(Action<T> action, T argument)
        {
            Task.Run(()=>action(argument));
        }
    }
}
