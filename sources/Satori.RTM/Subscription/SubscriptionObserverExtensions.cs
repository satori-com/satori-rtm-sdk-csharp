#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal static class SubscriptionObserverExtensions
    {
        public static void NotifyCreated(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnCreated(subscription);
            }
        }

        public static void NotifyDeleted(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnDeleted(subscription);
            }
        }

        public static void NotifyEnterUnsubscribed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnEnterUnsubscribed(subscription);
            }
        }

        public static void NotifyLeaveUnsubscribed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnLeaveUnsubscribed(subscription);
            }
        }

        public static void NotifyEnterSubscribing(this ISubscriptionObserver observer, ISubscription subscription, RtmSubscribeRequest request)
        {
            if (observer != null)
            {
                observer.OnEnterSubscribing(subscription, request);
            }
        }

        public static void NotifyLeaveSubscribing(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnLeaveSubscribing(subscription);
            }
        }

        public static void NotifyEnterSubscribed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnEnterSubscribed(subscription);
            }
        }

        public static void NotifyLeaveSubscribed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnLeaveSubscribed(subscription);
            }
        }

        public static void NotifyEnterUnsubscribing(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnEnterUnsubscribing(subscription);
            }
        }

        public static void NotifyLeaveUnsubscribing(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnLeaveUnsubscribing(subscription);
            }
        }

        public static void NotifyEnterFailed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnEnterFailed(subscription);
            }
        }

        public static void NotifyLeaveFailed(this ISubscriptionObserver observer, ISubscription subscription)
        {
            if (observer != null)
            {
                observer.OnLeaveFailed(subscription);
            }
        }

        public static void NotifySubscriptionData(this ISubscriptionObserver observer, ISubscription subscription, RtmSubscriptionData data)
        {
            if (observer != null)
            {
                observer.OnSubscriptionData(subscription, data);
            }
        }

        public static void NotifySubscriptionInfo(this ISubscriptionObserver observer, ISubscription subscription, RtmSubscriptionInfo info)
        {
            if (observer != null)
            {
                observer.OnSubscriptionInfo(subscription, info);
            }
        }

        public static void NotifySubscriptionError(this ISubscriptionObserver observer, ISubscription subscription, RtmSubscriptionError error)
        {
            if (observer != null)
            {
                observer.OnSubscriptionError(subscription, error);
            }
        }

        public static void NotifySubscribeError(this ISubscriptionObserver observer, ISubscription subscription, Exception error)
        {
            if (observer != null)
            {
                observer.OnSubscribeError(subscription, error);
            }
        }

        public static void NotifyUnsubscribeError(this ISubscriptionObserver observer, ISubscription subscription, Exception error)
        {
            if (observer != null)
            {
                observer.OnUnsubscribeError(subscription, error);
            }
        }

        public static void NotifyCreated(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnCreated(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyDeleted(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnDeleted(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyEnterUnsubscribed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnEnterUnsubscribed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyLeaveUnsubscribed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnLeaveUnsubscribed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyEnterSubscribing(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, RtmSubscribeRequest request)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnEnterSubscribing(subscription, request);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyLeaveSubscribing(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnLeaveSubscribing(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyEnterSubscribed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnEnterSubscribed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyLeaveSubscribed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnLeaveSubscribed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyEnterUnsubscribing(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnEnterUnsubscribing(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyLeaveUnsubscribing(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnLeaveUnsubscribing(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyEnterFailed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnEnterFailed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyLeaveFailed(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnLeaveFailed(subscription);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifySubscriptionData(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, RtmSubscriptionData data)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnSubscriptionData(subscription, data);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifySubscriptionInfo(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, RtmSubscriptionInfo info)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnSubscriptionInfo(subscription, info);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifySubscriptionError(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, RtmSubscriptionError error)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnSubscriptionError(subscription, error);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifySubscribeError(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, Exception error)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnSubscribeError(subscription, error);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }

        public static void NotifyUnsubscribeError(this IEnumerable<ISubscriptionObserver> observers, ISubscription subscription, Exception error)
        {
            if (observers == null)
            {
                return;
            }

            foreach (var observer in observers)
            {
                try
                {
                    observer.OnUnsubscribeError(subscription, error);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionWatcher.Swallow(ex);
                }
            }
        }
    }
}