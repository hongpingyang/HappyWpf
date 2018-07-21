using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Events
{
    public class EventSubscription : IEventSubscription
    {
        private readonly IDelegateReference _actionReference;

        public EventSubscription(IDelegateReference actionReference)
        {
            _actionReference = actionReference ?? throw new ArgumentNullException(nameof(actionReference));
        }

        public Action Action =>(Action)_actionReference.Target;

        public virtual void InvokeAction(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action();
        }

        public  void InvokePublish(params object[] arguments)
        {
            if(Action!=null)
               InvokeAction(Action);
        }
    }

    public class EventSubscription<T> : IEventSubscription
    {
        private readonly IDelegateReference _actionReference;
        private readonly IDelegateReference _filterReference;

        public EventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            _actionReference = actionReference ?? throw new ArgumentNullException(nameof(actionReference));
            _filterReference = filterReference ?? throw new ArgumentNullException(nameof(filterReference));
        }

        public Action<T> Action => (Action<T>)_actionReference.Target;

        public Predicate<T> Filter => (Predicate<T>)_filterReference.Target;

        public virtual void InvokeAction(Action<T> action,T argument)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            action(argument);
        }

        public void InvokePublish(params object[] arguments)
        {
            if (Action != null&& Filter!=null)
            { 
              T argument = default(T);
              if (arguments != null && arguments.Length > 0 && arguments[0] != null)
              {
                argument = (T)arguments[0];
              }

              if (Filter(argument))
              {

                InvokeAction(Action, argument);
              }
            }
        }
    }
}
