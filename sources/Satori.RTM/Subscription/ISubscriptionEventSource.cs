#pragma warning disable 1591

using System;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// This interface describes subscription events. It is not meant to be 
    /// implemented in user code. The <see cref="SubscriptionObserver"/> class 
    /// is a default implementation of this interface.
    /// Use an instance of the <see cref="SubscriptionObserver"/> class
    /// to provide the 
    /// <see cref="IRtmClient"/>.<see cref="IRtmClient.CreateSubscription(string,SubscriptionModes,ISubscriptionObserver)"/> 
    /// method  with a subscription observer. 
    /// </summary>
    public interface ISubscriptionEventSource
    {
        /// <summary>
        /// Occurs when a subscription is created.
        /// </summary>
        event Action<ISubscription> OnCreated;

        /// <summary>
        /// Occurs when a subscription is deleted.
        /// </summary>
        event Action<ISubscription> OnDeleted;

        /// <summary>
        /// Occurs when when a subscription is unsubscribed.
        /// </summary>
        event Action<ISubscription> OnEnterUnsubscribed;

        /// <summary>
        /// Occurs when a subscription is no longer unsubscribed.
        /// </summary>
        event Action<ISubscription> OnLeaveUnsubscribed;

        /// <summary>
        /// Occurs when a subscription is subscribing.
        /// </summary>
        event Action<ISubscription, RtmSubscribeRequest> OnEnterSubscribing;

        /// <summary>
        /// Occurs when a subscription is no longer subscribing.
        /// </summary>
        event Action<ISubscription> OnLeaveSubscribing;

        /// <summary>
        /// Occurs when a subscription is subscribed.
        /// </summary>
        event Action<ISubscription> OnEnterSubscribed;

        /// <summary>
        /// Occurs when a subscription is no longer subscribed.
        /// </summary>
        event Action<ISubscription> OnLeaveSubscribed;

        /// <summary>
        /// Occurs when a subscription is unsubscribing.
        /// </summary>
        event Action<ISubscription> OnEnterUnsubscribing;

        /// <summary>
        /// Occurs when a subscription is no longer unsubscribing.
        /// </summary>
        event Action<ISubscription> OnLeaveUnsubscribing;

        /// <summary>
        /// Occurs when a subscription is failed.
        /// </summary>
        event Action<ISubscription> OnEnterFailed;

        /// <summary>
        /// Occurs when a subscription is no longer failed.
        /// </summary>
        event Action<ISubscription> OnLeaveFailed;

        /// <summary>
        /// Occurs when a subscription receives data.
        /// </summary>
        event Action<ISubscription, RtmSubscriptionData> OnSubscriptionData;

        /// <summary>
        /// Occurs when a subscription receives info.
        /// </summary>
        event Action<ISubscription, RtmSubscriptionInfo> OnSubscriptionInfo;

        /// <summary>
        /// Occurs when a subscription receives an error.
        /// </summary>
        event Action<ISubscription, RtmSubscriptionError> OnSubscriptionError;

        /// <summary>
        /// Occurs when a subscribe request fails.
        /// </summary>
        event Action<ISubscription, Exception> OnSubscribeError;

        /// <summary>
        /// Occurs when an unsubscribe request fails.
        /// </summary>
        event Action<ISubscription, Exception> OnUnsubscribeError;
    }
}