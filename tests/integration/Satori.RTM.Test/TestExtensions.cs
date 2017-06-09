using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    public static class TestExtensions
    {
        public static async Task AssertDequeue<T>(this QueueAsync<T> queue, params T[] items) 
        {
            foreach (var item in items) 
            {
                Assert.That(await queue.Dequeue(), Is.EqualTo(item));
            }
        }

        public static async Task AssertDequeue(this TestSubscriptionObserverQueue queue, params string[] items) 
        {
            foreach (var item in items) 
            {
                Assert.That(await queue.Dequeue(), Is.EqualTo(item));
            }
        }

        public static async Task AssertEmpty<T>(this QueueAsync<T> queue, IRtmClient client, int millis) 
        {
            await Task.Delay(millis);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));
        }

        public static async Task<QueueAsync<string>> StartAndWaitConnected(this IRtmClient client)
        {
            await client.Yield();
            var queue = client.CreateStateQueue();
            client.Start();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            return queue;
        }

        public static async Task<JToken> CreateSubscriptionAndWaitSubscribed(
            this IRtmClient client,
            string channel,
            string position,
            ISubscriptionObserver observer)
        {
            var tcs = new TaskCompletionSource<JToken>();
            var sobs = new SubscriptionObserver();
            sobs.OnEnterSubscribed += (s) =>
            {
                tcs.TrySetResult(s.Position);
            };
            sobs.OnEnterFailed += (_) =>
            {
                tcs.TrySetException(new Exception("subscription was removed"));
            };
            sobs.OnDeleted += (_) =>
            {
                tcs.TrySetException(new Exception("subscription was removed"));
            };
            var subCfg = new SubscriptionConfig(SubscriptionModes.Advanced)
            {
                Position = position,
                Observer = new SubscriptionCompoundObserver(sobs, observer)
            };
            client.CreateSubscription(channel, subCfg);
            return await tcs.Task.ConfigureAwait(false);
        }

        public static async Task<RtmSubscriptionData> DequeueAndVerify<T>(this QueueAsync<RtmSubscriptionData> subscriptionData, string subscriptionId, T message)
        {
            var d = await subscriptionData.Dequeue().ConfigureAwait(false);
            if (d.SubscriptionId != subscriptionId)
            {
                throw new Exception("wrong channel");
            }

            if (d.Messages.Length != 1)
            {
                throw new Exception("only one message was expected");
            }

            if (!d.Messages[0].ToObject<T>().Equals(message))
            {
                throw new Exception("not equals");
            }

            return d;
        }

        public static QueueAsync<string> CreateStateQueue(this IRtmClient client)
        {
            var queue = new QueueAsync<string>();
            client.SetStateObserver(queue);
            return queue;
        }

        public static void SetStateObserver(this IRtmClient client, IObservableSink<string> observer)
        {
            client.OnEnterStopped += () => { observer.Next("enter-stopped"); };
            client.OnLeaveStopped += () => { observer.Next("leave-stopped"); };
            client.OnEnterConnecting += () => { observer.Next("enter-connecting"); };
            client.OnLeaveConnecting += () => { observer.Next("leave-connecting"); };
            client.OnEnterConnected += (_) => { observer.Next("enter-connected"); };
            client.OnLeaveConnected += (_) => { observer.Next("leave-connected"); };
            client.OnEnterAwaiting += () => { observer.Next("enter-awaiting"); };
            client.OnLeaveAwaiting += () => { observer.Next("leave-awaiting"); };
            client.OnEnterDisposed += () => { observer.Next("enter-disposed"); };
        }

        public static void SetClientErrorObserver(this IRtmClient client, IObservableSink<string> observer)
        {
            client.OnError += ex => { observer.Next("error:" + ex.GetType().Name); };
        }

        public static void SetSubscriptionStateObserver(this ISubscriptionEventSource source, IObservableSink<string> observer)
        {
            source.OnCreated += (_) => { observer.Next("rtm:created"); };
            source.OnEnterUnsubscribed += (_) => { observer.Next("rtm:enter-unsubscribed"); };
            source.OnLeaveUnsubscribed += (_) => { observer.Next("rtm:leave-unsubscribed"); };
            source.OnEnterSubscribing += (_, r) => { observer.Next("rtm:enter-subscribing"); };
            source.OnLeaveSubscribing += (_) => { observer.Next("rtm:leave-subscribing"); };
            source.OnEnterSubscribed += (_) => { observer.Next("rtm:enter-subscribed"); };
            source.OnLeaveSubscribed += (_) => { observer.Next("rtm:leave-subscribed"); };
            source.OnEnterUnsubscribing += (_) => { observer.Next("rtm:enter-unsubscribing"); };
            source.OnLeaveUnsubscribing += (_) => { observer.Next("rtm:leave-unsubscribing"); };
            source.OnDeleted += (_) => { observer.Next("rtm:deleted"); };
            source.OnEnterFailed += (_) => { observer.Next("rtm:enter-failed"); };
            source.OnLeaveFailed += (_) => { observer.Next("rtm:leave-failed"); };
        }

        public static void SetSubscriptionPduObserver(this ISubscriptionEventSource source, IObservableSink<string> observer)
        {
            source.OnSubscriptionData += (_, pdu) =>
            {
                foreach (var m in pdu.Messages)
                {
                    observer.Next($"rtm:subscription-data:{m?.ToString()}");
                }
            };
            source.OnSubscriptionInfo += (_, pdu) =>
            {
                observer.Next($"rtm:subscription-info:{pdu.Info}");
            };
            source.OnSubscriptionError += (_, pdu) =>
            {
                observer.Next($"rtm:subscription-error:{pdu.Code}");
            };
        }

        public static QueueAsync<RtmSubscriptionData> CreateSubscriptionDataQueue(this ISubscriptionEventSource source)
        {
            var queue = new QueueAsync<RtmSubscriptionData>();
            source.SetSubscriptionDataQueue(queue);
            return queue;
        }

        public static void SetSubscriptionDataQueue(
            this ISubscriptionEventSource source,
            IObservableSink<RtmSubscriptionData> observer)
        {
            source.OnSubscriptionData += (_, data) =>
            {
                observer.Next(data);
            };
        }
    }
}
