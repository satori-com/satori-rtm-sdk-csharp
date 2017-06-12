#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Subscription compound observer. See <see cref="ISubscriptionObserver"/>. 
    /// </summary>
    public class SubscriptionCompoundObserver : ISubscriptionObserver
    {
        private ISubscriptionObserver[] _observers;

        public SubscriptionCompoundObserver(IEnumerable<ISubscriptionObserver> observers)
        {
            _observers = observers?.ToArray();
        }

        public SubscriptionCompoundObserver(params ISubscriptionObserver[] observers)
        {
            _observers = observers;
        }

        public void OnDeleted(ISubscription subscription)
        {
            _observers.NotifyDeleted(subscription);
        }

        public void OnEnterFailed(ISubscription subscription)
        {
            _observers.NotifyEnterFailed(subscription);
        }

        public void OnEnterSubscribed(ISubscription subscription)
        {
            _observers.NotifyEnterSubscribed(subscription);
        }

        public void OnEnterSubscribing(ISubscription subscription, RtmSubscribeRequest request)
        {
            _observers.NotifyEnterSubscribing(subscription, request);
        }

        public void OnEnterUnsubscribed(ISubscription subscription)
        {
            _observers.NotifyEnterUnsubscribed(subscription);
        }

        public void OnEnterUnsubscribing(ISubscription subscription)
        {
            _observers.NotifyEnterUnsubscribing(subscription);
        }

        public void OnCreated(ISubscription subscription)
        {
            _observers.NotifyCreated(subscription);
        }

        public void OnLeaveFailed(ISubscription subscription)
        {
            _observers.NotifyLeaveFailed(subscription);
        }

        public void OnLeaveSubscribed(ISubscription subscription)
        {
            _observers.NotifyLeaveSubscribed(subscription);
        }

        public void OnLeaveSubscribing(ISubscription subscription)
        {
            _observers.NotifyLeaveSubscribing(subscription);
        }

        public void OnLeaveUnsubscribed(ISubscription subscription)
        {
            _observers.NotifyLeaveUnsubscribed(subscription);
        }

        public void OnLeaveUnsubscribing(ISubscription subscription)
        {
            _observers.NotifyLeaveUnsubscribing(subscription);
        }

        public void OnSubscriptionData(ISubscription subscription, RtmSubscriptionData data)
        {
            _observers.NotifySubscriptionData(subscription, data);
        }

        public void OnSubscriptionInfo(ISubscription subscription, RtmSubscriptionInfo info)
        {
            _observers.NotifySubscriptionInfo(subscription, info);
        }

        public void OnSubscriptionError(ISubscription subscription, RtmSubscriptionError error)
        {
            _observers.NotifySubscriptionError(subscription, error);
        }

        public void OnSubscribeError(ISubscription subscription, Exception error)
        {
            _observers.NotifySubscribeError(subscription, error);
        }

        public void OnUnsubscribeError(ISubscription subscription, Exception error)
        {
            _observers.NotifyUnsubscribeError(subscription, error);
        }
    }
}