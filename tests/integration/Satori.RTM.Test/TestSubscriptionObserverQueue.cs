using System;
using System.Threading.Tasks;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    public class TestSubscriptionObserverQueue : SubscriptionObserver, IObservableSink<string>
    {
        private readonly QueueAsync<string> _queue;

        public TestSubscriptionObserverQueue() : this(new QueueAsync<string>())
        {
        }

        public TestSubscriptionObserverQueue(QueueAsync<string> queue)
        {
            _queue = queue;
        }

        void IObservableSink<string>.Next(string val)
        {
            _queue.Enqueue(val);
        }

        public void Enqueue(string item)
        {
            _queue.Enqueue(item);
        }

        public Task<string> Dequeue()
        {
            return _queue.Dequeue();
        }

        public bool TryDequeue(out string res)
        {
            return _queue.TryDequeue(out res);
        }

        public string TryDequeue()
        {
            return _queue.TryDequeue();
        }

        public void ObserveSubscriptionState()
        {
            this.SetSubscriptionStateObserver(_queue);
        }

        public void ObserveSubscriptionPdu()
        {
            this.SetSubscriptionPduObserver(_queue);
        }

        public void ObserveSubscribeUnsubscribeError()
        {
            this.SetSubscribeUnsubscribeErrorObserver(_queue);
        }

        public void ObserveAll()
        {
            ObserveSubscriptionState();
            ObserveSubscriptionPdu();
            ObserveSubscribeUnsubscribeError();
        }
    }
}
