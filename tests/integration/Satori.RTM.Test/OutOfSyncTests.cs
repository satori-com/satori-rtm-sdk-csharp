using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class OutOfSyncTests : IntegrationTestsBase
    {
        [Test]
        public async Task FastForwardOnOutOfSyncWhenSimpleMode()
        {
            var channel = GenerateRandomChannelName();

            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetConnector(ConnectAndFastForwardOnData)
                .Build();

            var queue = client.CreateStateQueue();

            var obs = new TestSubscriptionObserverQueue(queue);
            obs.ObserveSubscriptionState();
            obs.ObserveSubscriptionPdu();

            client.OnError += ex =>
            {
                queue.Enqueue($"error:{ex.Message}");
            };

            RtmSubscriptionInfo subInfo = null;
            obs.OnSubscriptionInfo += (_, info) => subInfo = info; 

            await client.CreateSubscription(channel, SubscriptionModes.Simple, obs);

            await client.Start();

            await queue.AssertDequeue(
                "rtm:created",
                "rtm:enter-unsubscribed",
                "leave-stopped",
                "enter-connecting",
                "leave-connecting",
                "rtm:leave-unsubscribed",
                "rtm:enter-subscribing",
                "enter-connected",
                "rtm:leave-subscribing",
                "rtm:enter-subscribed");

            // trigger subscription/info
            await client.Publish(channel, "msg", Ack.Yes);

            // on connected info
            Assert.That(await queue.Dequeue(), Is.EqualTo($"rtm:subscription-info:{RtmSubscriptionInfo.FastForward}"));

            var sub = await client.GetSubscription(channel);
            Assert.That(sub.Position, Is.EqualTo(null));
            Assert.That(subInfo.Position, Is.Not.EqualTo(null));

            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }

        [Test]
        public async Task FailOnOutOfSyncWhenAdvancedMode()
        {
            var channel = GenerateRandomChannelName();

            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetConnector(ConnectAndOutOfSyncOnData)
                .Build();
            
            var queue = client.CreateStateQueue();

            var obs = new TestSubscriptionObserverQueue(queue);
            obs.ObserveSubscriptionState();
            obs.ObserveSubscriptionPdu();

            client.OnError += ex =>
            {
                queue.Enqueue($"error:{ex.Message}");
            };

            await client.CreateSubscription(channel, SubscriptionModes.Advanced, obs);

            RtmSubscriptionError subError = null;
            obs.OnSubscriptionError += (_, error) => subError = error;

            await client.Start();

            await queue.AssertDequeue(
                "rtm:created",
                "rtm:enter-unsubscribed", 
                "leave-stopped",
                "enter-connecting",
                "leave-connecting",
                "rtm:leave-unsubscribed",
                "rtm:enter-subscribing", 
                "enter-connected",
                "rtm:leave-subscribing",
                "rtm:enter-subscribed");

            // trigger subscription/info
            await client.Publish(channel, "msg", Ack.Yes);

            await queue.AssertDequeue(
                $"rtm:leave-subscribed",
                "rtm:enter-failed",
                $"rtm:subscription-error:{RtmSubscriptionError.OutOfSync}");

            var sub = await client.GetSubscription(channel);
            Assert.That(subError.Position, Is.EqualTo(sub.Position));

            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }

        private static Task<IConnection> ConnectAndFastForwardOnData(string url, CancellationToken ct) 
        { 
            return TestConnection.Connect(
                url, 
                ct, 
                transform: (ConnectionStepResult res) => 
                {
                    // replace subscription/data pdu with subscription/info
                    var ev = res.AsUnsolicitedEvent();
                    if (ev != null && ev.pdu.Action.StartsWith(RtmActions.SubscriptionData, StringComparison.InvariantCulture)) 
                    {
                        var oldPdu = ev.pdu.As<RtmSubscriptionData>();
                        var newPdu = Pdu.Create(
                            action: RtmActions.SubscriptionInfo,
                            id: null,
                            body: new RtmSubscriptionInfo {
                                Info = RtmSubscriptionInfo.FastForward,
                                SubscriptionId = oldPdu.Body.SubscriptionId,
                                Position = oldPdu.Body.Position
                            });
                        return new ConnectionStepResult.UnsolicitedEvent(newPdu);
                    }

                    return res;
                });
        }

        private static Task<IConnection> ConnectAndOutOfSyncOnData(string url, CancellationToken ct) 
        { 
            return TestConnection.Connect(
                url, 
                ct, 
                transform: (ConnectionStepResult res) => 
            {
                // replace subscription/data pdu with subscription/info
                var ev = res.AsUnsolicitedEvent();
                if (ev != null && ev.pdu.Action.StartsWith(RtmActions.SubscriptionData, StringComparison.InvariantCulture)) 
                {
                    var oldPdu = ev.pdu.As<RtmSubscriptionData>();
                    var newPdu = Pdu.Create(
                        action: RtmActions.SubscriptionError,
                        id: null,
                        body: new RtmSubscriptionError {
                            Code = RtmSubscriptionError.OutOfSync,
                            SubscriptionId = oldPdu.Body.SubscriptionId,
                            Position = oldPdu.Body.Position
                        });
                    return new ConnectionStepResult.UnsolicitedEvent(newPdu);
                }

                return res;
            });
        }
    }
}
