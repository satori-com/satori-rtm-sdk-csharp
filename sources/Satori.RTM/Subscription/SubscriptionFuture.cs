#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    internal class SubscriptionFuture : ISubscription
    {
        public readonly ISubscriptionObserver Observer;

        public SubscriptionFuture(
            string subscriptionId, 
            SubscriptionModes mode, 
            string filter, 
            uint? period, 
            string position, 
            RtmSubscribeHistory history, 
            ISubscriptionObserver observer)
        {
            SubscriptionId = subscriptionId;
            Mode = mode;
            Filter = filter;
            Period = period;
            Position = position;
            History = history;

            Observer = observer ?? new SubscriptionObserver();
        }

        public string SubscriptionId { get; set; }

        public SubscriptionModes Mode { get; set; }

        public bool IsFastForward => (Mode & SubscriptionModes.FastForward) == SubscriptionModes.FastForward;

        public bool IsTrackPosition => (Mode & SubscriptionModes.TrackPosition) == SubscriptionModes.TrackPosition;

        public string Filter { get; set; }

        public uint? Period { get; set; }

        public string Position { get; set; }

        public RtmSubscribeHistory History { get; set; }
    }
}