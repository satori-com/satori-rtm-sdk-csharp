#pragma warning disable 1591

using System;
using Satori.Common;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Describes subscription events. Provide the this observer to <see cref="IRtmClient"/> 
    /// when creating a subscription. 
    /// </summary>
    public class SubscriptionObserver : ISubscriptionObserver, ISubscriptionEventSource
    {
        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnCreated"/>. 
        /// </summary>
        public event Action<ISubscription> OnCreated;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnDeleted"/>. 
        /// </summary>
        public event Action<ISubscription> OnDeleted;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnEnterUnsubscribed"/>. 
        /// </summary>
        public event Action<ISubscription> OnEnterUnsubscribed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnLeaveUnsubscribed"/>. 
        /// </summary>
        public event Action<ISubscription> OnLeaveUnsubscribed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnEnterSubscribing"/>. 
        /// </summary>
        public event Action<ISubscription, RtmSubscribeRequest> OnEnterSubscribing;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnLeaveSubscribing"/>. 
        /// </summary>
        public event Action<ISubscription> OnLeaveSubscribing;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnEnterSubscribed"/>. 
        /// </summary>
        public event Action<ISubscription> OnEnterSubscribed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnLeaveSubscribed"/>. 
        /// </summary>
        public event Action<ISubscription> OnLeaveSubscribed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnEnterUnsubscribing"/>. 
        /// </summary>
        public event Action<ISubscription> OnEnterUnsubscribing;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnLeaveUnsubscribing"/>. 
        /// </summary>
        public event Action<ISubscription> OnLeaveUnsubscribing;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnEnterFailed"/>. 
        /// </summary>
        public event Action<ISubscription> OnEnterFailed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnLeaveFailed"/>. 
        /// </summary>
        public event Action<ISubscription> OnLeaveFailed;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnSubscriptionData"/>. 
        /// </summary>
        public event Action<ISubscription, RtmSubscriptionData> OnSubscriptionData;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnSubscriptionInfo"/>. 
        /// </summary>
        public event Action<ISubscription, RtmSubscriptionInfo> OnSubscriptionInfo;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnSubscriptionError"/>. 
        /// </summary>
        public event Action<ISubscription, RtmSubscriptionError> OnSubscriptionError;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnSubscribeError"/>. 
        /// </summary>
        public event Action<ISubscription, Exception> OnSubscribeError;

        /// <summary>
        /// See <see cref="ISubscriptionEventSource.OnUnsubscribeError"/>. 
        /// </summary>
        public event Action<ISubscription, Exception> OnUnsubscribeError;

        void ISubscriptionObserver.OnDeleted(ISubscription subscription)
        {
            OnDeleted.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnEnterFailed(ISubscription subscription)
        {
            OnEnterFailed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnEnterSubscribed(ISubscription subscription)
        {
            OnEnterSubscribed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnEnterSubscribing(ISubscription subscription, RtmSubscribeRequest request)
        {
            OnEnterSubscribing.InvokeSafe(subscription, request);
        }

        void ISubscriptionObserver.OnEnterUnsubscribed(ISubscription subscription)
        {
            OnEnterUnsubscribed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnEnterUnsubscribing(ISubscription subscription)
        {
            OnEnterUnsubscribing.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnCreated(ISubscription subscription)
        {
            OnCreated.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnLeaveFailed(ISubscription subscription)
        {
            OnLeaveFailed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnLeaveSubscribed(ISubscription subscription)
        {
            OnLeaveSubscribed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnLeaveSubscribing(ISubscription subscription)
        {
            OnLeaveSubscribing.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnLeaveUnsubscribed(ISubscription subscription)
        {
            OnLeaveUnsubscribed.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnLeaveUnsubscribing(ISubscription subscription)
        {
            OnLeaveUnsubscribing.InvokeSafe(subscription);
        }

        void ISubscriptionObserver.OnSubscriptionData(ISubscription subscription, RtmSubscriptionData data)
        {
            OnSubscriptionData.InvokeSafe(subscription, data);
        }

        void ISubscriptionObserver.OnSubscriptionInfo(ISubscription subscription, RtmSubscriptionInfo info)
        {
            OnSubscriptionInfo.InvokeSafe(subscription, info);
        }

        void ISubscriptionObserver.OnSubscriptionError(ISubscription subscription, RtmSubscriptionError error)
        {
            OnSubscriptionError.InvokeSafe(subscription, error);
        }

        void ISubscriptionObserver.OnSubscribeError(ISubscription subscription, Exception error)
        {
            OnSubscribeError.InvokeSafe(subscription, error);
        }

        void ISubscriptionObserver.OnUnsubscribeError(ISubscription subscription, Exception error)
        {
            OnUnsubscribeError.InvokeSafe(subscription, error);
        }
    }
}