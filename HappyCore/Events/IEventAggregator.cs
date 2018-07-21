using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCore.Events
{
    public interface IEventAggregator
    {
        void Subscribe(string subscribeid, Action action);

        void Subscribe<T>(string subscribeid, Action<T> action);

        void Subscribe(string subscribeid, Action action, ThreadOption ThreadOption);

        void Subscribe<T>(string subscribeid, Action<T> action, ThreadOption ThreadOption);

        void Subscribe(string subscribeid, Action action, bool keepSubscriberReferenceAlive);

        void Subscribe<T>(string subscribeid, Action<T> action, bool keepSubscriberReferenceAlive);

        void Subscribe(string subscribeid, Action action, ThreadOption threadOption, bool keepSubscriberReferenceAlive);

        void Subscribe<T>(string subscribeid, Action<T> action, Predicate<T> filter);

        void Subscribe<T>(string subscribeid, Action<T> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive);

        void Subscribe<T>(string subscribeid, Action<T> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<T> filter);

        void UnSubscribe(string subscribeid);

        void UnSubscribe(string subscribeid, Action action);

        void UnSubscribe<T>(string subscribeid, Action<T> action);

        void Publish(string publishid);

        void Publish<T>(string publishid, T TPayload);
    }
}
