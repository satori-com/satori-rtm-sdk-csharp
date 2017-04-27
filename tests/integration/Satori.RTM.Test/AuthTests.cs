using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class AuthTests : IntegrationTestsBase
    {
        [Test]
        public void GenerateHash()
        {
            var hash = AuthRoleSecret.ComputeHash("B37Ab888CAB4343434bAE98AAAAAABC1", "nonce");
            Assert.That(hash, Is.EqualTo("B510MG+AsMpvUDlm7oFsRg=="));
        }

        [Test]
        public async Task AuthenticateWithSuccess()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetRoleSecretAuthenticator(Config.AuthRoleName, Config.AuthRoleSecretKey)
                .Build();

            var queue = client.CreateStateQueue();
            await client.Start();

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));

            await client.Stop();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connected"));

            await client.Dispose();
        }

        [Test]
        public async Task FailToAuthenticateWithBadKey()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetRoleSecretAuthenticator(Config.AuthRoleName, "bad_secret_key")
                .Build();

            Exception error = null;
            client.OnError += ex => error = ex is AggregateException ? ex.InnerException : ex;

            var queue = client.CreateStateQueue();
            await client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));

            Assert.That(error, Is.TypeOf<AuthException>());

            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));

            await client.Stop();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-awaiting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-stopped"));

            await client.Dispose();
        }

        [Test]
        public async Task FailToAuthenticateWithBadRole()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetRoleSecretAuthenticator("bad_role_name", "bad_secret_key")
                .Build();

            Exception error = null;
            client.OnError += ex => error = ex is AggregateException ? ex.InnerException : ex;

            var queue = client.CreateStateQueue();
            await client.Start();
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));

            Assert.That(error, Is.TypeOf<AuthException>());

            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-awaiting"));

            await client.Dispose();
        }
        
        [Test]
        public async Task SubscribeToRestrictedChannelWhenAuthorized()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey)
                .SetRoleSecretAuthenticator(Config.AuthRoleName, Config.AuthRoleSecretKey)
                .Build();

            var queue = client.CreateStateQueue();
            await client.Start();

            var channel = GenerateRandomChannelName();

            var subObs = new TestSubscriptionObserverQueue(queue);
            subObs.ObserveSubscriptionState();
            subObs.ObserveSubscriptionPdu();
            await client.CreateSubscription(channel, SubscriptionModes.Advanced, subObs);

            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-stopped"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:created"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("leave-connecting"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-unsubscribed"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("enter-connected"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:leave-subscribing"));
            Assert.That(await queue.Dequeue(), Is.EqualTo("rtm:enter-subscribed"));

            await client.Dispose();
        }

        [Test]
        public async Task PublishToRestrictedChannelWhenNotAuthorized()
        {
            var client = new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();

            await client.Yield();
            await client.Start();

            try
            {
                var restrictedChannel = Config.AuthRestrictedChannel;
                await client.Publish(restrictedChannel, "foo", Ack.Yes);
            }
            catch (PduException ex)
            {
                Assert.That(ex.Error.Code, Is.EqualTo(AuthErrorCodes.AuthorizationDenied));
            }

            await client.Dispose();
        }
    }
}
