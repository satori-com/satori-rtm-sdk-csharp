#pragma warning disable 1591

using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class Subscription : SubscriptionFuture
    {
        private readonly RtmClient _client;
        private State _state;

        public Subscription(
            RtmClient client, 
            string subscriptionId, 
            SubscriptionModes mode, 
            string filter, 
            uint? period, 
            string position, 
            RtmSubscribeHistory history, 
            ISubscriptionObserver observer)
            : base(subscriptionId, mode, filter, period, position, history, observer)
        {
            _client = client;
            _state = new State.Unsubscribed(this);
        }

        public bool MarkAsDeleted { get; private set; } = false;

        public SubscriptionFuture Future { get; private set; } = null;

        public override string ToString()
        {
            return $"[Subscription: SubscriptionId={SubscriptionId}, MarkAsDeleted={MarkAsDeleted}]";
        }

        public ISuccessfulAwaiter<SubscriptionFuture> Process()
        {
            var process = new SuccessfulAwaiter<SubscriptionFuture>();
            DoTransitionLoop(process);
            return process;
        }

        public ISuccessfulAwaiter<State> Yield()
        {
            return _client.Yield().Map(_ => _state);
        }

        public SubscriptionFuture ProcessSubscribe(SubscriptionConfig config, ISubscriptionObserver observer)
        {
            Log.V("Processing subscribe, subscription: {0}", this);

            if (!MarkAsDeleted || Future != null)
            {
                return null;
            }

            Future = new SubscriptionFuture(
                subscriptionId: SubscriptionId,
                mode: config.Mode,
                filter: config.Filter,
                period: config.Period,
                position: config.Position,
                history: config.History,
                observer: observer);

            return Future;
        }

        public SubscriptionFuture ProcessUnsubscribe()
        {
            Log.V("Processing unsubscribe, subscription: {0}", this);

            if (!MarkAsDeleted)
            {
                MarkAsDeleted = true;
                _state.ProcessMarkedAsDeleted();
                return this;
            }

            if (Future != null)
            {
                Future.NotifyCreated();
                Future.NotifyDeleted();
                var ret = Future;
                Future = null;
                return ret;
            }

            return null;
        }

        public void ProcessConnected(IConnection conn)
        {
            Log.V("Processing connected, subscription: {0}", this);
            _state.ProcessConnected(conn);
        }

        public void ProcessDisconnected()
        {
            Log.V("Processing disconnected, subscription: {0}", this);
            _state.ProcessDisconnected();
        }

        public bool ProcessSubscriptionData(RtmSubscriptionData data)
        {
            Log.V("Processing subscription data, subscription: {0}, data: {1}", this, data);

            if (_state.ProcessSubscriptionData(data) != null)
            {
                this.NotifySubscriptionData(data);
                return true;
            }

            return false;
        }

        public bool ProcessSubscriptionInfo(RtmSubscriptionInfo info)
        {
            Log.V("Processing subscription info, subscription: {0}, info: {1}", this, info);

            if (_state.ProcessSubscriptionInfo(info) != null)
            {
                this.NotifySubscriptionInfo(info);
                return true;
            }

            return false;
        }

        public bool ProcessSubscriptionError(RtmSubscriptionError error)
        {
            Log.V("Processing subscription error, subscription: {0}, error: {1}", this, error);

            if (_state.ProcessSubscriptionError(error) != null)
            {
                this.NotifySubscriptionError(error);
                return true;
            }

            return false;
        }

        private async void DoTransitionLoop(ISuccessfulCompletionSink<SubscriptionFuture> cs)
        {
            Log.V("Subscription loop started, subscription: {0}", this);
            while (true)
            {
                var next = await _state.Process();
                var prev = _state;

                // do transition
                _state.OnLeave();

                Log.I("Transitioning from '{0}' to '{1}'", prev, next);

                _state = next;
                if (next == null)
                {
                    this.NotifyDeleted();
                    break;
                }

                _state.OnEnter(prev);
            }

            cs.Succeed(Future);
            Log.V("Subscription loop completed, subscription: {0}", this);
        }
    }
}