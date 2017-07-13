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
        public async Task Expired()
        {
            // NOTE: You must use async/await keywords here instead of returning Task directly, 
            // because otherwise test fails with message: 
            // "Method has non-void return value, but no result is expected"
           await ConnectToInsecureUrl("wss://expired.badssl.com/");
        }

        [Test]
        public async Task WrongHost()
        {
            // NOTE: You must use async/await keywords here instead of returning Task directly, 
            // because otherwise test fails with message: 
            // "Method has non-void return value, but no result is expected"
            await ConnectToInsecureUrl("wss://wrong.host.badssl.com/");
        }

        [Test]
        public async Task SelfSigned()
        {
            // NOTE: You must use async/await keywords here instead of returning Task directly, 
            // because otherwise test fails with message: 
            // "Method has non-void return value, but no result is expected"
            await ConnectToInsecureUrl("wss://self-signed.badssl.com/");
        }

        [Test]
        public async Task UntrustedRoot()
        {
            // NOTE: You must use async/await keywords here instead of returning Task directly, 
            // because otherwise test fails with message: 
            // "Method has non-void return value, but no result is expected"
            await ConnectToInsecureUrl("wss://untrusted-root.badssl.com/");
        }

        private async Task ConnectToInsecureUrl(string url)
        {
            var client = (RtmClient)new RtmClientBuilder(Config.Endpoint, Config.AppKey).Build();
            var connector = client.Connector;
            try 
            {
                await connector(url, new ConnectionOptions(), CancellationToken.None);
                Assert.Fail();
            } 
            catch (Exception ex)
            {
                client.Log.V(ex, "Expected connection exception because connection is insecure");
                var inner = GetMostInnerException(ex);

                var monoException = "Mono.Security.Protocol.Tls.TlsException";
                var netException = "System.Security.Authentication.AuthenticationException";
                var iosException = "Mono.Security.Interface.TlsException";
                Assert.That(
                    inner.GetType().FullName, 
                    Is.EqualTo(monoException).Or.EqualTo(netException).Or.EqualTo(iosException));
            }
        }

        private Exception GetMostInnerException(Exception ex) 
        {
            return ex.InnerException == null ? ex : GetMostInnerException(ex.InnerException);
        }
    }
}