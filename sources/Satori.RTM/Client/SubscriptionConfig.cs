#pragma warning disable 1591

using System;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Defines the initial settings for a subscription. 
    /// </summary>
    /// <remarks>The SDK provides several strategies for resubscribing. See <see cref="SubscriptionModes"/>.</remarks>
    public class SubscriptionConfig
    {
        /// <summary>
        /// Creates a subscription configuration to use in subscription requests, with a specific
        /// subscription modes.
        /// </summary>
        public SubscriptionConfig(SubscriptionModes mode, string position = null, ISubscriptionObserver observer = null)
        {
            Mode = mode;
            Position = position;
            Observer = observer;
        }

        public SubscriptionModes Mode { get; set; }

        /// <summary>
        /// Gets or sets the <c>filter</c> for a subscription.
        /// </summary>
        /// <remarks>
        /// A filter is a statement created with fSQL that defines the filter query to
        /// run on the messages published to the channel. 
        /// </remarks>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the <c>period</c> to use with a filter in a subscription.
        /// </summary>
        /// <remarks>
        /// The <c>period</c> is the period of time, in seconds, that the RTM service runs the filter on
        /// channel messages before it sends the result to the client application.
        /// </remarks>
        public uint? Period { get; set; }

        /// <summary>
        /// Gets or sets the <c>position</c> to use in subscription requests.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the history to use in subscription requests. 
        /// To use this method, set history settings for the channel in the Developer Portal.
        /// </summary>
        public RtmSubscribeHistory History { get; set; }

        /// <summary>
        /// Gets or sets the observer to receive subscription events. 
        /// </summary>
        public ISubscriptionObserver Observer { get; set; }
    }
}