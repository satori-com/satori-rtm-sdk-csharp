#pragma warning disable 1591

using System;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Describes an observer interface which should be implemented 
    /// in order to listen for subscription events. 
    /// Use an instance of the <see cref="SubscriptionObserver"/> class
    /// to provide the 
    /// <see cref="IRtmClient"/>.<see cref="IRtmClient.CreateSubscription(string,SubscriptionModes,ISubscriptionObserver)"/> 
    /// method with a subscription observer. 
    /// </summary>
    public interface ISubscriptionObserver
    {
        /// <summary>
        /// Called when the subscription is created.
        /// </summary>
        void OnCreated(ISubscription subscription);

        /// <summary>
        /// Called when the subscription is deleted.
        /// </summary>
        void OnDeleted(ISubscription subscription);

        /// <summary>
        /// Called when the subscription enters an unsubscribed state.
        /// </summary>
        /// <remarks>
        /// The subscription enters an unsubscribed state in the following cases: 
        /// <list type="bullte">
        ///   <item><description>a connection to the RTM service is dropped.</description></item>
        ///   <item><description>An unsubscribe request succeed. </description></item>
        ///   <item><description>A subscription error is received, for example, out of sync error</description></item>
        /// </list> 
        /// </remarks>
        void OnEnterUnsubscribed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription leaves an unsubscribed state. 
        /// </summary>
        /// <remarks>
        /// The subscription leaves an unsubscribed state in the following cases: 
        /// <list type="bullet">
        ///   <item><description>The subscription will enter a subscribing state because 
        ///     the <see cref="IRtmClient.CreateSubscription(string, SubscriptionConfig)"/> method has been called 
        ///     or the <see cref="IRtmClient"/> client has successfully reconnected to the RTM service. </description></item>
        ///   <item><description>The subscription will be deleted because 
        ///     the <see cref="IRtmClient.RemoveSubscription"/> method has been called. </description></item>
        /// </list>
        /// </remarks>
        void OnLeaveUnsubscribed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription enters a subscribing state. 
        /// </summary>
        /// <remarks>
        /// The subscription enters a subscribing state when the 
        /// <see cref="IRtmClient.CreateSubscription(string, SubscriptionConfig)"/> method 
        /// has been called or the <see cref="IRtmClient"/> client has successfully 
        /// reconnected to the RTM service. During this state a subscribe request is sent 
        /// to the RTM service. 
        /// </remarks>
        void OnEnterSubscribing(ISubscription subscription, RtmSubscribeRequest request);

        /// <summary>
        /// Called when the subscription leaves a subscribing state.
        /// </summary>
        /// <remarks>
        /// The subscription leaves a subscribing state in the following cases:
        /// <list type="bullet">
        ///   <item><description>A subscribe request to the RTM service succeed or failed. </description></item>
        ///   <item><description>The <see cref="IRtmClient"/>.<see cref="IRtmClient.RemoveSubscription"/> 
        ///     method has been called. </description></item>
        ///   <item><description>A connection is dropped due to a network issue or the 
        ///     <see cref="IRtmClient"/>.<see cref="IRtmClient.Stop"/> method has been called. </description></item>
        /// </list> 
        /// </remarks>
        void OnLeaveSubscribing(ISubscription subscription);

        /// <summary>
        /// Called when the subscription enters a subscribed state. 
        /// </summary>
        /// <remarks>
        /// The subscription enters a subscribed state when a subscribe request to
        /// the RTM services succeed. During this state the subscription can receive 
        /// data, info and error. See the <see cref="OnSubscriptionData"/>, <see cref="OnSubscriptionInfo"/>, 
        /// and <see cref="OnSubscriptionError"/>  methods.   
        /// </remarks>
        void OnEnterSubscribed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription leaves a subscribed state.
        /// </summary>
        /// <remarks>
        /// The subscription leaves a subscribed state in the following cases: 
        /// <list type="bullet">
        ///   <item><description>A subscription error has been received, for example, out of sync error. </description></item>
        ///   <item><description>The <see cref="IRtmClient"/>.<see cref="IRtmClient.RemoveSubscription"/> 
        ///     method has been called. </description></item>
        ///   <item><description>A connection is dropped due to a network issue or the 
        ///     <see cref="IRtmClient"/>.<see cref="IRtmClient.Stop"/> method has been called. </description></item>
        /// </list>
        /// </remarks>
        void OnLeaveSubscribed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription enters an unsubscribing state. 
        /// </summary>
        /// <remarks>
        /// The subscription enters an unsubscribing state when the 
        /// <see cref="IRtmClient"/>.<see cref="IRtmClient.RemoveSubscription"/> method has been called. 
        /// During this state an unsubscribe request in sent to the RTM service.  
        /// </remarks>
        void OnEnterUnsubscribing(ISubscription subscription);

        /// <summary>
        /// Called when a subscription is no longer unsubscribing. 
        /// </summary>
        /// <remarks>
        /// The subscription leaves an unsubscribing state in the following cases:
        /// <list type="bullet">
        ///   <item><description>An unsubscribe request to the RTM service succeed or failed. </description></item>
        ///   <item><description>The <see cref="IRtmClient"/>.<see cref="IRtmClient.RemoveSubscription"/> 
        ///     method has been called. </description></item>
        ///   <item><description>A connection is dropped due to a network issue or the 
        ///     <see cref="IRtmClient"/>.<see cref="IRtmClient.Stop"/> method has been called. </description></item>
        /// </list> 
        /// </remarks>
        void OnLeaveUnsubscribing(ISubscription subscription);

        /// <summary>
        /// Called when a subscription enters a failed state. 
        /// </summary>
        /// <remarks>
        /// The subscription enters a failed state in the following cases: 
        /// <list type="bullet">
        ///   <item><description>A subscription error has been received. See the <see cref="OnSubscriptionError"/> method.</description></item>
        ///   <item><description>A subscribe request failed.</description></item>
        /// </list>
        /// </remarks>
        void OnEnterFailed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription leaves a failed state. 
        /// </summary>
        /// <remarks>
        /// The subscription leaves a failed state in the following cases: 
        /// <list type="bullet">
        ///   <item><description>The connections will be removed because the 
        ///     <see cref="IRtmClient"/>.<see cref="IRtmClient.RemoveSubscription"/> method has been called. </description></item>
        ///   <item><description>A connection is dropped due to a network issue or the 
        ///     <see cref="IRtmClient"/>.<see cref="IRtmClient.Stop"/> method has been called. </description></item>
        /// </list>
        /// </remarks>
        void OnLeaveFailed(ISubscription subscription);

        /// <summary>
        /// Called when the subscription receives data. See <see cref="RtmSubscriptionData"/> class.
        /// </summary>
        void OnSubscriptionData(ISubscription subscription, RtmSubscriptionData data);

        /// <summary>
        /// Called when the subscription receives info. See <see cref="RtmSubscriptionInfo"/> class. 
        /// </summary>
        void OnSubscriptionInfo(ISubscription subscription, RtmSubscriptionInfo info);

        /// <summary>
        /// Called when the subscription receives an error. See <see cref="RtmSubscriptionError"/> class. 
        /// </summary>
        void OnSubscriptionError(ISubscription subscription, RtmSubscriptionError error);

        /// <summary>
        /// Called when the subscribe request fails. See <see cref="RtmSubscribeError"/> class. 
        /// </summary>
        void OnSubscribeError(ISubscription subscription, Exception error);

        /// <summary>
        /// Called when the unsubscribe request fails. See <see cref="RtmUnsubscribeError"/> class. 
        /// </summary>
        void OnUnsubscribeError(ISubscription subscription, Exception error);
    }
}
