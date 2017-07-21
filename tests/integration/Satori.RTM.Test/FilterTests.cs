using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class FilterTests : IntegrationTestsBase
    {
        [Test]
        public async Task TwoSubscriptionsWithDifferentNames()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();

            var channel = GenerateRandomChannelName();
            var subId1 = "s1_" + channel;
            var subId2 = "s2_" + channel;

            await client.StartAndWaitConnected();

            var subCfg = new SubscriptionConfig(SubscriptionModes.Advanced)
            {
                Filter = $"select * FROM `{channel}`"
            };

            var queue1 = new TestSubscriptionObserverQueue();
            queue1.ObserveSubscriptionState();
            queue1.ObserveSubscriptionPdu();
            subCfg.Observer = queue1;
            client.CreateSubscription(subId1, subCfg);

            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:created"));
            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue1.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            var queue2 = new TestSubscriptionObserverQueue();
            queue2.ObserveSubscriptionState();
            queue2.ObserveSubscriptionPdu();
            subCfg.Observer = queue2;
            client.CreateSubscription(subId2, subCfg);

            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:created"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            var msg = new JObject(new JProperty("filed", "value"));

            await client.Publish(channel, msg, Ack.Yes);
            Assert.That(await queue1.Dequeue(), Is.EqualTo($"rtm:subscription-data:{msg}"));
            Assert.That(await queue2.Dequeue(), Is.EqualTo($"rtm:subscription-data:{msg}"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue1.TryDequeue(), Is.EqualTo(null));
            Assert.That(queue2.TryDequeue(), Is.EqualTo(null));

            await client.Dispose();
        }

        [Test]
        public async Task CreateFilterWIthExistingSubscriptionId()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.StartAndWaitConnected();

            var channel = GenerateRandomChannelName();

            client.CreateSubscription(channel, SubscriptionModes.Advanced, null);

            var queue = client.CreateStateQueue();
            client.SetClientErrorObserver(queue);
            var subObserver = new TestSubscriptionObserverQueue(queue);
            subObserver.ObserveAll();
            
            client.CreateSubscription(
                channel,
                new SubscriptionConfig(SubscriptionModes.Advanced, observer: subObserver)
                {
                    Filter = $"select * FROM `{channel}`"
                });

            await queue.AssertDequeue("rtm:subscribe-error:InvalidOperationException");
            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }
    }
}
