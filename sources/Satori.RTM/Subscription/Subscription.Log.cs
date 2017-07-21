#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    public partial class DefaultLoggers
    {
        public static Logger ClientRtmSubscription { get; set; } = new DefaultLogger(
            "Satori.Rtm.Client.Subscription");
    }

    internal partial class Subscription
    {
        public Logger Log { get; } = DefaultLoggers.ClientRtmSubscription;
    }
}
