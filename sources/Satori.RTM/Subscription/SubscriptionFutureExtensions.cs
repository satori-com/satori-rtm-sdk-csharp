#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    internal static class SubscriptionFutureExtensions
    {
        public static void NotifyCreated(this SubscriptionFuture future)
        {
            future?.Observer.NotifyCreated(future);
        }

        public static void NotifyDeleted(this SubscriptionFuture future)
        {
            future?.Observer.NotifyDeleted(future);
        }

        public static void NotifyEnterUnsubscribed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyEnterUnsubscribed(future);
        }

        public static void NotifyLeaveUnsubscribed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyLeaveUnsubscribed(future);
        }

        public static void NotifyEnterSubscribing(this SubscriptionFuture future, RtmSubscribeRequest request)
        {
            future?.Observer.NotifyEnterSubscribing(future, request);
        }

        public static void NotifyLeaveSubscribing(this SubscriptionFuture future)
        {
            future?.Observer.NotifyLeaveSubscribing(future);
        }

        public static void NotifyEnterSubscribed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyEnterSubscribed(future);
        }

        public static void NotifyLeaveSubscribed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyLeaveSubscribed(future);
        }

        public static void NotifyEnterUnsubscribing(this SubscriptionFuture future)
        {
            future?.Observer.NotifyEnterUnsubscribing(future);
        }

        public static void NotifyLeaveUnsubscribing(this SubscriptionFuture future)
        {
            future?.Observer.NotifyLeaveUnsubscribing(future);
        }

        public static void NotifyEnterFailed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyEnterFailed(future);
        }

        public static void NotifyLeaveFailed(this SubscriptionFuture future)
        {
            future?.Observer.NotifyLeaveFailed(future);
        }

        public static void NotifySubscriptionData(this SubscriptionFuture future, RtmSubscriptionData data)
        {
            future?.Observer.NotifySubscriptionData(future, data);
        }

        public static void NotifySubscriptionInfo(this SubscriptionFuture future, RtmSubscriptionInfo info)
        {
            future?.Observer.NotifySubscriptionInfo(future, info);
        }

        public static void NotifySubscriptionError(this SubscriptionFuture future, RtmSubscriptionError error)
        {
            future?.Observer.NotifySubscriptionError(future, error);
        }
    }
}