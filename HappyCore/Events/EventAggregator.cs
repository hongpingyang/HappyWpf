using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace HappyCore.Events
{
    /// <summary>
    /// 事件聚合器
    /// </summary>
    public class EventAggregator: IEventAggregator
    {
        private readonly Dictionary<string, ICollection<IEventSubscription>> Events = new Dictionary<string, ICollection<IEventSubscription>>();

        private readonly static object lockEvent = new object();

        #region  订阅
        public void Subscribe(string subscribeid,Action action)
        {
            Subscribe(subscribeid,action, ThreadOption.PublisherThread, false);
        }

        public void Subscribe<T>(string subscribeid, Action<T> action)
        {
            Subscribe(subscribeid,action, ThreadOption.PublisherThread, false);
        }

        public void Subscribe(string subscribeid, Action action, ThreadOption ThreadOption)
        {
            Subscribe(subscribeid,action, ThreadOption, false);
        }

        public void Subscribe<T>(string subscribeid, Action<T> action, ThreadOption ThreadOption)
        {
            Subscribe(subscribeid,action, ThreadOption, false);
        }

        public void Subscribe(string subscribeid, Action action, bool keepSubscriberReferenceAlive)
        {
            Subscribe(subscribeid,action, ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        public void Subscribe<T>(string subscribeid, Action<T> action, bool keepSubscriberReferenceAlive)
        {
            Subscribe(subscribeid,action, ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="threadOption"></param>
        /// <param name="keepSubscriberReferenceAlive"></param>
        public void Subscribe(string subscribeid, Action action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);

            IEventSubscription subscription;

            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription(actionReference);
                    break;
                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription(actionReference);
                    break;
                case ThreadOption.UIThread:
                    subscription = new DispatcherEventSubscription(actionReference);
                    break;
                default:
                    subscription = new EventSubscription(actionReference);
                    break;
            }

            AddSubscriprion(subscribeid, subscription);
        }

        public void Subscribe<T>(string subscribeid, Action<T> action, Predicate<T> filter)
        {
            Subscribe(subscribeid, action, ThreadOption.PublisherThread, false, filter);
        }

        public  void Subscribe<T>(string subscribeid, Action<T> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            Subscribe(subscribeid, action, threadOption, keepSubscriberReferenceAlive,null);
        }

        /// <summary>
        /// 支持过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscribeid"></param>
        /// <param name="action"></param>
        /// <param name="threadOption"></param>
        /// <param name="keepSubscriberReferenceAlive"></param>
        /// <param name="filter"></param>
        public void Subscribe<T>(string subscribeid,Action<T> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<T> filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference;
            if (filter != null)
            {
                filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
            }
            else
            {
                filterReference = new DelegateReference(new Predicate<T>(delegate { return true; }), false);
            }

            IEventSubscription subscription;

            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription<T>(actionReference, filterReference);
                    break;
                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription<T>(actionReference, filterReference);
                    break;
                case ThreadOption.UIThread:
                    subscription = new DispatcherEventSubscription<T>(actionReference, filterReference);
                    break;
                default:
                    subscription = new EventSubscription<T>(actionReference, filterReference);
                    break;
            }

            AddSubscriprion(subscribeid, subscription);
        }

        private void AddSubscriprion(string subscribeid, IEventSubscription subscription)
        {
            lock (lockEvent)
            {
                ICollection<IEventSubscription> existingEvent = null;
                if (!Events.TryGetValue(subscribeid, out existingEvent))
                {
                    existingEvent = new List<IEventSubscription>() { subscription };
                    Events[subscribeid] = existingEvent;
                }
                else
                {
                    existingEvent.Add(subscription);
                }
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void UnSubscribe(string subscribeid)
        {
            lock (lockEvent)
            {
                Events.Remove(subscribeid);
            }
        }

        /// <summary>
        /// 取消订阅 改变集合
        /// </summary>
        public void UnSubscribe(string subscribeid,Action action)
        {
            lock (lockEvent)
            {
                ICollection<IEventSubscription> existingEvent = null;
                if (Events.TryGetValue(subscribeid, out existingEvent))
                {
                    IEventSubscription eventSubscription = existingEvent.Cast<EventSubscription>().FirstOrDefault(evt => evt.Action == action);
                    if (eventSubscription != null)
                    {
                        existingEvent.Remove(eventSubscription);
                    }
                    Events[subscribeid] = existingEvent;
                }
            }
        }

        /// <summary>
        /// 取消订阅 改变集合
        /// </summary>
        public void UnSubscribe<T>(string subscribeid, Action<T> action)
        {
            lock (lockEvent)
            {
                ICollection<IEventSubscription> existingEvent = null;
                if (Events.TryGetValue(subscribeid, out existingEvent))
                {
                    IEventSubscription eventSubscription = existingEvent.Cast<EventSubscription<T>>().FirstOrDefault(evt => evt.Action == action);
                    if (eventSubscription != null)
                    {
                        existingEvent.Remove(eventSubscription);
                    }
                    Events[subscribeid] = existingEvent;
                }
            }
        }

        #endregion

        #region 发布
        public void Publish(string publishid)
        {
            lock (lockEvent)
            {
                ICollection<IEventSubscription> existingEvent = null;
                if (Events.TryGetValue(publishid, out existingEvent))
                {
                    foreach (var item in existingEvent.ToArray())
                    {
                        item.InvokePublish();
                    }
                }
            }
        }

        public void Publish<T>(string publishid, T TPayload)
        {
            lock (lockEvent)
            {
                ICollection<IEventSubscription> existingEvent = null;
                if (Events.TryGetValue(publishid, out existingEvent))
                {
                    foreach (var item in existingEvent.ToArray())
                    {
                        item.InvokePublish(TPayload);
                    }

                }
            }
        }

        #endregion


    }
}
