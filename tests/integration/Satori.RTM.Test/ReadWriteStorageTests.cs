using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class ReadWriteStorageTests : IntegrationTestsBase
    {
        [Test]
        public async Task ReadAfterWrite()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            client.Start();

            var msg = "foo";
            await client.Write(channel, msg, Ack.Yes);
            var reply = await client.Read<string>(channel);
            Assert.That(reply.Message, Is.EqualTo(msg));

            await client.Dispose();
        }

        [Test]
        public async Task WriteTwiceAndRead()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            client.Start();

            var msg1 = "foo";
            var msg2 = "bar";
            await client.Write(channel, msg1, Ack.Yes);
            await client.Write(channel, msg2, Ack.Yes);
            var reply = await client.Read<string>(channel);
            Assert.That(reply.Message, Is.EqualTo(msg2));

            await client.Dispose();
        }

        [Test]
        public async Task WriteDeleteRead()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            client.Start();

            var msg = "foo";
            await client.Write(channel, msg, Ack.Yes);
            await client.Delete(channel, Ack.Yes);
            var reply = await client.Read<string>(channel);
            Assert.That(reply.Message, Is.EqualTo(null));

            await client.Dispose();
        }

        [Test]
        public async Task ReadOldMessage()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            client.Start();

            var msg1 = "foo";
            var msg2 = "bar";
            var writeReply = await client.Write(channel, msg1, Ack.Yes);
            await client.Write(channel, msg2, Ack.Yes);
            var readReply = await client.Read<string>(new RtmReadRequest
            {
                Channel = channel,
                Position = writeReply.Position
            });
            Assert.That(readReply.Message, Is.EqualTo(msg1));

            await client.Dispose();
        }

        [Test]
        public async Task WriteNullShouldBeOk()
        {
            var channel = GenerateRandomChannelName();
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            client.Start();

            var queue = new TestSubscriptionObserverQueue();
            await client.CreateSubscriptionAndWaitSubscribed(channel, null, queue);
            await client.Yield();
            queue.ObserveSubscriptionState();
            queue.ObserveSubscriptionPdu();

            await client.Write(channel, (string)null, Ack.Yes);
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:subscription-data:"));

            await Task.Delay(200);
            await client.Yield();
            Assert.That(queue.TryDequeue() == null);

            await client.Dispose();
        }
    }
}
