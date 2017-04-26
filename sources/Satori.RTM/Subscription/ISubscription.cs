#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Describes a subscription. 
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Gets the subscription identifier.
        /// </summary>
        /// <value>The subscription identifier.</value>
        string SubscriptionId { get; }

        /// <summary>
        /// Gets the subscription mode.
        /// </summary>
        /// <value>The mode.</value>
        SubscriptionModes Mode { get; }

        /// <summary>
        /// Gets the filter with which a subscription was created. 
        /// </summary>
        /// <value>The filter.</value>
        string Filter { get; }

        /// <summary>
        /// Gets the filter period with which a subscription was created. 
        /// </summary>
        /// <value>The period.</value>
        uint? Period { get; }

        /// <summary>
        /// Gets the current position. Position is updated when subscription data is received
        /// </summary>
        /// <value>The position.</value>
        string Position { get; }

        /// <summary>
        /// Gets the history with which a subscription was created
        /// </summary>
        /// <value>The history.</value>
        RtmSubscribeHistory History { get; }
    }
}