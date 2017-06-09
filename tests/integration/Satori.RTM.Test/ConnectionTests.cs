using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class ConnectionTests : IntegrationTestsBase
    {
        [Test]
        public async Task ReconnectWhenConnectionDropped()
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var queue = client.CreateStateQueue();
            client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));

            var cn = await client.GetConnection();
            await cn.Close();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));

            await client.Dispose();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-disposed"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue(), Is.EqualTo(null));
        }

        [Test]
        public async Task ReconnectIfConnectionCannotBeEstablished()
        {
            var client = new RtmClientBuilder("ws://bad_endpoint", "bad_key").Build();
            var queue = client.CreateStateQueue();
            client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));

            await client.Dispose();
        }

        [Test]
        public void ReconnectIntervals()
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey)
            {
                MinReconnectInterval = TimeSpan.FromMilliseconds(10),
                MaxReconnectInterval = TimeSpan.FromMilliseconds(100)
            }.Build();

            Assert.That(
                client.NextReconnectInterval().TotalMilliseconds,
                Is.GreaterThanOrEqualTo(10).And.LessThanOrEqualTo(20));
            Assert.That(
                client.NextReconnectInterval().TotalMilliseconds,
                Is.GreaterThanOrEqualTo(20).And.LessThanOrEqualTo(30));
            Assert.That(
                client.NextReconnectInterval().TotalMilliseconds,
                Is.GreaterThanOrEqualTo(40).And.LessThanOrEqualTo(50));
            Assert.That(
                client.NextReconnectInterval().TotalMilliseconds,
                Is.GreaterThanOrEqualTo(80).And.LessThanOrEqualTo(90));

            for (int i = 0; i < 5; i++)
            {
                client.NextReconnectInterval();
            }

            Assert.That(client.NextReconnectInterval().TotalMilliseconds, Is.EqualTo(100d));
        }
    }
}
