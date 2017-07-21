using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class SubscriptionTests : IntegrationTestsBase
    {
        [Test]
        public async Task AutoDeleteSubscriptionOnDispose()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.Yield();

            var queue = new TestSubscriptionObserverQueue();
            queue.ObserveSubscriptionState();

            client.Start();
            client.CreateSubscription(channel, SubscriptionModes.Advanced, queue);

            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:created"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            await client.Dispose();

            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:deleted"));

            try
            {
                await client.GetSubscription(channel);
                Assert.Fail("Subscription wasn't removed");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public async Task AutoResubscribe()
        {
            var channel = GenerateRandomChannelName();
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var queue = client.CreateStateQueue();

            client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            var subData = new TestSubscriptionObserverQueue();
            subData.ObserveSubscriptionPdu();
            await client.CreateSubscriptionAndWaitSubscribed(channel, null, subData).ConfigureAwait(false);
            await client.Publish(channel, "message-1", Ack.No).ConfigureAwait(false);
            await client.Yield();
            Assert.That(await subData.Dequeue(), Is.EqualTo("rtm:subscription-data:message-1"));

            var cn = await client.GetConnection();
            await cn.Close();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));
            await client.Publish(channel, "message-2", Ack.No).ConfigureAwait(false);
            await client.Yield();
            Assert.That(await subData.Dequeue(), Is.EqualTo("rtm:subscription-data:message-2"));
            await client.Dispose();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-disposed"));
            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue() == null);
            Assert.That(subData.TryDequeue() == null);
        }

        [Test]
        public async Task DelayedPublish()
        {
            var channel = GenerateRandomChannelName();

            var subClient = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await subClient.Yield();
            subClient.Start();
            var subData = new TestSubscriptionObserverQueue();
            subData.ObserveSubscriptionPdu();
            await subClient.CreateSubscriptionAndWaitSubscribed(channel, null, subData);

            var pubClient = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await pubClient.Yield();
            var noAckTask = pubClient.Publish(channel, "no-ack", Ack.No);
            var withAckTask = pubClient.Publish(channel, "with-ack", Ack.Yes);
            await Task.Delay(100);
            Assert.That(noAckTask.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            Assert.That(withAckTask.Status, Is.EqualTo(TaskStatus.WaitingForActivation));

            pubClient.Start();
            await noAckTask.ConfigureAwait(false);
            await withAckTask.ConfigureAwait(false);

            Assert.That(await subData.Dequeue(), Is.EqualTo("rtm:subscription-data:no-ack"));
            Assert.That(await subData.Dequeue(), Is.EqualTo("rtm:subscription-data:with-ack"));

            await subClient.Dispose();
            await pubClient.Dispose();
        }

        [Test]
        public async Task ManualResubscribe()
        {
            var channel = GenerateRandomChannelName();
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var queue = client.CreateStateQueue();
            var subObs = new TestSubscriptionObserverQueue(queue);
            subObs.ObserveSubscriptionState();
            subObs.ObserveSubscriptionPdu();

            await client.Yield();
            client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));
            client.CreateSubscription(channel, SubscriptionModes.Advanced, subObs);
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:created"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            await client.Publish(channel, "message-1", Ack.No).ConfigureAwait(false);
            await client.Yield();
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:subscription-data:message-1"));
            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));

            var cn = await client.GetConnection();
            await cn.Close();

            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));
            client.Start();
            var sw = new Stopwatch();
            sw.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            sw.Stop();
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(50));

            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            await client.Publish(channel, "message-2", Ack.No).ConfigureAwait(false);
            await client.Yield();
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:subscription-data:message-2"));
            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));

            await client.Dispose();
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-disposed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:deleted"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));
        }

        [Test]
        public async Task RepeatFirstMessage()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.Yield();
            var subObs = new SubscriptionObserver();
            var subData = subObs.CreateSubscriptionDataQueue();

            var subscription = new QueueAsync<string>();
            subObs.OnEnterSubscribed += s =>
            {
                subscription.Enqueue(s.Position);
            };

            client.Start();
            client.CreateSubscription(channel, SubscriptionModes.Advanced, subObs);
            var pos = await subscription.Dequeue().ConfigureAwait(false);

            await client.Publish(channel, "message-1", Ack.Yes).ConfigureAwait(false);
            await subData.DequeueAndVerify(channel, "message-1").ConfigureAwait(false);
            await client.Yield();

            client.RemoveSubscription(channel);
            client.CreateSubscription(
                channel, 
                new SubscriptionConfig(SubscriptionModes.Advanced, position: pos, observer: subObs));
            await subData.DequeueAndVerify(channel, "message-1").ConfigureAwait(false);
            await client.Dispose();
        }

        [Test]
        public async Task RepeatSecondMessage()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();

            var obs = new SubscriptionObserver();
            var subData = obs.CreateSubscriptionDataQueue();
            client.Start();

            await client.CreateSubscriptionAndWaitSubscribed(channel, null, obs);
            await client.Publish(channel, "first-message", Ack.Yes).ConfigureAwait(false);
            var pos = (await subData.DequeueAndVerify(channel, "first-message").ConfigureAwait(false)).Position;
            await client.Publish(channel, "second-message", Ack.Yes).ConfigureAwait(false);
            await subData.DequeueAndVerify(channel, "second-message").ConfigureAwait(false);
            client.RemoveSubscription(channel);
            await client.CreateSubscriptionAndWaitSubscribed(channel, pos, obs).ConfigureAwait(false);
            await subData.DequeueAndVerify(channel, "second-message").ConfigureAwait(false);
            client.RemoveSubscription(channel);
            client.Stop();
            await client.Dispose();
        }

        [Test]
        public async Task SubscribeWithSimpleModeAndPosition()
        {
            var channel = GenerateRandomChannelName();
            var reqs = new QueueAsync<Pdu<object>>();
            var reps = new QueueAsync<Pdu>();
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetConnector((url, opts, ct) => TestConnection.Connect(url, opts, ct, reqs, reps))
                .Build();

            client.Start();

            // get position
            client.CreateSubscription(
                channel, 
                new SubscriptionConfig(SubscriptionModes.Simple));

            Assert.That((await reqs.Dequeue()).Action, Is.EqualTo("rtm/subscribe"));

            var rep1 = await reps.Dequeue();
            Assert.That(rep1.Action, Is.EqualTo("rtm/subscribe/ok"));
            var pos = rep1.As<RtmSubscribeReply>().Body.Position;

            client.RemoveSubscription(channel);
            Assert.That((await reqs.Dequeue()).Action, Is.EqualTo("rtm/unsubscribe"));
            Assert.That((await reps.Dequeue()).Action, Is.EqualTo("rtm/unsubscribe/ok"));

            await client.Yield();
            var sub = await client.GetSubscription(channel);
            Assert.That(sub, Is.Null);

            client.CreateSubscription(
                channel, 
                new SubscriptionConfig(SubscriptionModes.Simple, position: pos));

            var req3 = await reqs.Dequeue();
            var body3 = (RtmSubscribeRequest)req3.Body;
            Assert.That(req3.Action, Is.EqualTo("rtm/subscribe"));
            Assert.That(body3.Position, Is.EqualTo(pos));
            Assert.That(body3.FastForward, Is.True);

            var rep3 = await reps.Dequeue();
            Assert.That(rep3.Action, Is.EqualTo("rtm/subscribe/ok"));

            // cause resubsubscription
            var conn = await client.GetConnection();
            await conn.Close();

            var req4 = await reqs.Dequeue();
            var body4 = (RtmSubscribeRequest)req4.Body;
            Assert.That(req4.Action, Is.EqualTo("rtm/subscribe"));
            Assert.That(body4.Position, Is.EqualTo(null));
            Assert.That(body4.FastForward, Is.True);

            var rep4 = await reps.Dequeue();
            Assert.That(rep4.Action, Is.EqualTo("rtm/subscribe/ok"));

            await client.Dispose();
        }

        [Test]
        public async Task SubscribeWithAdvancedModeAndPosition()
        {
            var channel = GenerateRandomChannelName();
            var reqs = new QueueAsync<Pdu<object>>();
            var reps = new QueueAsync<Pdu>();
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetConnector((url, opts, ct) => TestConnection.Connect(url, opts, ct, reqs, reps))
                .Build();

            client.Start();

            // get position
            client.CreateSubscription(
                channel, 
                new SubscriptionConfig(SubscriptionModes.Simple));

            Assert.That((await reqs.Dequeue()).Action, Is.EqualTo("rtm/subscribe"));

            var rep1 = await reps.Dequeue();
            Assert.That(rep1.Action, Is.EqualTo("rtm/subscribe/ok"));
            var pos1 = rep1.As<RtmSubscribeReply>().Body.Position;

            client.RemoveSubscription(channel);
            Assert.That((await reqs.Dequeue()).Action, Is.EqualTo("rtm/unsubscribe"));
            Assert.That((await reps.Dequeue()).Action, Is.EqualTo("rtm/unsubscribe/ok"));

            await client.Yield();
            var sub = await client.GetSubscription(channel);
            Assert.That(sub, Is.Null); // subscription is removed

            client.CreateSubscription(
                channel, 
                new SubscriptionConfig(SubscriptionModes.Advanced, position: pos1));

            var req3 = await reqs.Dequeue();
            var body3 = (RtmSubscribeRequest)req3.Body;
            Assert.That(req3.Action, Is.EqualTo("rtm/subscribe"));
            Assert.That(body3.Position, Is.EqualTo(pos1));
            Assert.That(body3.FastForward, Is.False);

            var rep3 = await reps.Dequeue();
            Assert.That(rep3.Action, Is.EqualTo("rtm/subscribe/ok"));
            var pos2 = rep3.As<RtmSubscribeReply>().Body.Position;

            // cause resubsubscription
            var conn = await client.GetConnection();
            await conn.Close();

            var req4 = await reqs.Dequeue();
            var body4 = (RtmSubscribeRequest)req4.Body;
            Assert.That(req4.Action, Is.EqualTo("rtm/subscribe"));
            Assert.That(body4.Position, Is.EqualTo(pos2));
            Assert.That(body4.FastForward, Is.False);

            var rep4 = await reps.Dequeue();
            Assert.That(rep4.Action, Is.EqualTo("rtm/subscribe/ok"));

            await client.Dispose();
        }

        [Test]
        public async Task RestartOnUnsubscribeError() 
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetConnector(
                    (url, opts, ct) => TestConnection.Connect(
                        url, 
                        opts,
                        ct, 
                        transform: res => 
                        {
                            var reply = res.AsReply()?.AsPositive();
                            if (reply != null && reply.pdu.Action.StartsWith(RtmActions.Unsubscribe, StringComparison.InvariantCulture)) 
                            {
                                var oldPdu = reply.pdu.As<RtmUnsubscribeReply>();
                                var newPdu = Pdu.Create<RtmUnsubscribeError>(
                                    action: $"{RtmActions.Unsubscribe}/{RtmOutcomes.Error}",
                                    id: oldPdu.Id,
                                    body: new RtmUnsubscribeError {
                                    SubscriptionId = oldPdu.Body.SubscriptionId
                                });
                                return new ConnectionOperationResult.Reply.Negative(newPdu);
                            }

                            return res;
                        })).Build();

            var queue = client.CreateStateQueue();
            var subObs = new TestSubscriptionObserverQueue(queue);
            subObs.ObserveSubscriptionState();

            client.Start();
            await queue.AssertDequeue(
                "leave-stopped", 
                "enter-connecting", 
                "leave-connecting", 
                "enter-connected");

            client.CreateSubscription(channel, SubscriptionModes.Advanced, subObs);
            await queue.AssertDequeue(
                "rtm:created", 
                "rtm:enter-unsubscribed", 
                "rtm:leave-unsubscribed", 
                "rtm:enter-subscribing", 
                "rtm:leave-subscribing", 
                "rtm:enter-subscribed");

            client.RemoveSubscription(channel);

            await queue.AssertDequeue(
                "rtm:leave-subscribed", 
                "rtm:enter-unsubscribing", 
                "rtm:leave-unsubscribing", 
                "rtm:enter-unsubscribed", 
                "rtm:leave-unsubscribed", 
                "rtm:deleted", 
                "leave-connected", 
                "enter-awaiting", 
                "leave-awaiting", 
                "enter-connecting", 
                "leave-connecting", 
                "enter-connected");

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));

            await client.Dispose();
        }

        [Test]
        public async Task CreateSubscriptionThatAlreadyExists()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.StartAndWaitConnected();

            var channel = GenerateRandomChannelName();

            client.CreateSubscription(channel, SubscriptionModes.Advanced, null);

            var queue = client.CreateStateQueue();
            client.SetClientErrorObserver(queue);
            var subObserver = new TestSubscriptionObserverQueue(queue);
            subObserver.ObserveAll();
            
            client.CreateSubscription(channel, SubscriptionModes.Advanced, subObserver);

            await queue.AssertDequeue("rtm:subscribe-error:InvalidOperationException");
            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }

        [Test]
        public async Task CreateSubscriptionToRestrictedChannel()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.StartAndWaitConnected();

            var channel = Config.RestrictedChannel;

            var queue = client.CreateStateQueue();
            client.SetClientErrorObserver(queue);
            var subObserver = new TestSubscriptionObserverQueue(queue);
            subObserver.ObserveAll();

            client.CreateSubscription(channel, SubscriptionModes.Advanced, subObserver);

            await queue.AssertDequeue(
                "rtm:created",
                "rtm:enter-unsubscribed",
                "rtm:leave-unsubscribed",
                "rtm:enter-subscribing",
                "rtm:subscribe-error:SubscribeException:authorization_denied",
                "rtm:leave-subscribing",
                "rtm:enter-failed");
            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }

        [Test]
        public async Task RemoveSubscriptionThatDoesntExist()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.StartAndWaitConnected();

            var channel = GenerateRandomChannelName();

            var queue = client.CreateStateQueue();
            client.SetClientErrorObserver(queue);

            client.RemoveSubscription(channel);

            await queue.AssertDequeue("error:InvalidOperationException");
            await queue.AssertEmpty(client, millis: 200);

            await client.Dispose();
        }
    }
}