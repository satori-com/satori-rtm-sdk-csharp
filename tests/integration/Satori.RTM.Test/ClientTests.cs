#pragma warning disable 4014 // suppress not awaited

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class ClientTests : IntegrationTestsBase
    {
        [Test]
        public async Task NoStateTransitionsInCallbacks()
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var queue = client.CreateStateQueue();
            await client.Yield();
            client.Start();
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Uninitialized>());
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Uninitialized>());
            client.Stop();
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Connecting>());
            client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Connecting>());
            client.Dispose();
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-stopped"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Stopped>());
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Stopped>());
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Connecting>());
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.TypeOf<RtmClient.State.Connecting>());
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-disposed"));
            Thread.Sleep(10);
            Assert.That(client.GetStateAsync().GetResult(), Is.EqualTo(null));
            client.Start();
            Assert.That(client.GetStateAsync().GetResult(), Is.EqualTo(null));
            client.Stop();
            Assert.That(client.GetStateAsync().GetResult(), Is.EqualTo(null));
            client.Dispose();
            Assert.That(client.GetStateAsync().GetResult(), Is.EqualTo(null));
        }

        [Test]
        public async Task RestartOnSubscriptionError()
        {
            var trigger = new TaskCompletionSource<bool>();
            var counts = 3;
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            await client.Yield();
            var obs = new SubscriptionObserver();
            client.CreateSubscription(string.Empty, SubscriptionModes.Advanced, obs);
            obs.OnEnterFailed += (s) =>
            {
                if (counts == 0)
                {
                    trigger.TrySetResult(true);
                }
                else
                {
                    counts -= 1;
                    client.Restart();
                }
            };
            client.Start();
            await trigger.Task;
            client.Stop();
            await client.Dispose();
        }

        [Test]
        public async Task StartStopAndAtomicity()
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            bool isStopped = false;
            await client.Yield();
            var awaiter = client.StartImpl();
            client.OnLeaveStopped += () => isStopped = true;
            Thread.Sleep(100);
            Assert.That(isStopped == false);
            await awaiter;
            Assert.That(isStopped == true);
            client.OnEnterStopped += () => isStopped = false;
            awaiter = client.StopImpl();
            Thread.Sleep(100);
            Assert.That(isStopped == true);
            await awaiter;
            Assert.That(isStopped == false);
            await client.Dispose();
        }

        [Test]
        public async Task StopOnConnected()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var cs = new TaskCompletionSource<bool>();
            client.OnEnterConnected += _ =>
            {
                client.Stop();
            };
            client.OnEnterStopped += () =>
            {
                cs.TrySetResult(true);
            };
            client.Start();
            await cs.Task;
            await client.Dispose();
        }

        [Test]
        public async Task TwoClients()
        {
            var channel = GenerateRandomChannelName();
            int i = 0;
            var msg = $"message-{i++}";
            var subscriber = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();

            var publisher = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();

            publisher.Start();
            subscriber.Start();

            var subObs = new SubscriptionObserver();
            var subData = subObs.CreateSubscriptionDataQueue();
            await subscriber.CreateSubscriptionAndWaitSubscribed(channel, null, subObs).ConfigureAwait(false);

            await publisher.Publish(channel, msg, Ack.Yes).ConfigureAwait(false);

            await subData.DequeueAndVerify(channel, msg).ConfigureAwait(false);
            subscriber.Stop();
            if ((await subscriber.GetStateAsync()).GetConnection() != null)
            {
                throw new Exception("test failed");
            }

            do
            {
                msg = $"message-{i++}";
                await publisher.Publish(channel, msg, Ack.Yes).ConfigureAwait(false);
                await Task.Delay(500);
                RtmSubscriptionData data;
                if (subData.TryDequeue(out data))
                {
                    throw new Exception("test failed");
                }

                subscriber.Start();

                await subData.DequeueAndVerify(channel, msg).ConfigureAwait(false);

                subscriber.Stop();
                GC.Collect();
            } while (i < 3);

            await subscriber.Dispose();

            publisher.Stop();
            await publisher.Dispose();
        }

        [Test]
        public async Task ThrowWhenTooManyRequests()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
            {
                PendingActionQueueLength = 2
            }.Build();

            var channel = GenerateRandomChannelName();

            var t1 = client.Publish(channel, 1, Ack.Yes);
            var t2 = client.Publish(channel, 2, Ack.Yes);
            var t3 = client.Publish(channel, 3, Ack.Yes);

            await Task.WhenAny(t1, t2, t3);
            Assert.That(t1.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            Assert.That(t2.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            Assert.That(t3.Status, Is.EqualTo(TaskStatus.Faulted));
            Assert.That(t3.Exception.InnerException, Is.InstanceOf(typeof(QueueFullException)));

            client.Start();

            await t1;
            await t2;

            await client.Publish(channel, 4, Ack.Yes);

            var queue = client.CreateStateQueue();
            client.Stop();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));

            var t5 = client.Publish(channel, 5, Ack.Yes);
            var t6 = client.Publish(channel, 6, Ack.Yes);
            var t7 = client.Publish(channel, 7, Ack.Yes);

            await Task.WhenAny(t5, t6, t7);
            Assert.That(t5.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            Assert.That(t6.Status, Is.EqualTo(TaskStatus.WaitingForActivation));
            Assert.That(t7.Status, Is.EqualTo(TaskStatus.Faulted));
            Assert.That(t7.Exception.InnerException, Is.InstanceOf(typeof(QueueFullException)));

            client.Start();
            await t5;
            await t6;

            await client.Dispose();
        }

        [Test]
        public async Task ThrowWhenDisconnectedAndMaxPendingQueueLengthIsZero()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
            {
                PendingActionQueueLength = 0
            }.Build();

            var channel = GenerateRandomChannelName();

            var queue = client.CreateStateQueue();

            try
            {
                await client.Publish(channel, 1, Ack.Yes);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.InstanceOf(typeof(QueueFullException)));
            }

            client.Start();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            await client.Publish(channel, 2, Ack.Yes);

            client.Stop();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));

            try
            {
                await client.Publish(channel, 2, Ack.Yes);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.InstanceOf(typeof(QueueFullException)));
            }

            await client.Dispose();
        }
    }
}
