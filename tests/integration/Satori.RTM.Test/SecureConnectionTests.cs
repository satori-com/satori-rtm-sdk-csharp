using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    [TestFixture, Timeout(DefaultTestCaseTimeoutMsec)]
    public class SecureConnectionTests : IntegrationTestsBase
    {
        [Test]
        public Task Expired()
        {
            return ConnectToInsecureUrl("wss://expired.badssl.com/");
        }

        [Test]
        public Task WrongHost()
        {
            return ConnectToInsecureUrl("wss://wrong.host.badssl.com/");
        }

        [Test]
        public Task SelfSigned()
        {
            return ConnectToInsecureUrl("wss://self-signed.badssl.com/");
        }

        private async Task ConnectToInsecureUrl(string url)
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var connector = client.Connector;
            try 
            {
                await connector(url, CancellationToken.None);
                Assert.Fail();
            } 
            catch (Exception ex)
            {
                client.Log.V(ex, "Expected connection exception because connection is insecure");
                var inner = GetMostInnerException(ex);
                Assert.That(inner.GetType().Name, Is.EqualTo("TlsException"));
            }
        }

        private Exception GetMostInnerException(Exception ex) 
        {
            return ex.InnerException == null ? ex : GetMostInnerException(ex.InnerException);
        }
    }
}