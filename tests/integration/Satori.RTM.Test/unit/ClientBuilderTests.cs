using System;
using NUnit.Framework;
using Satori.Rtm.Client;

namespace Satori.Rtm.Test
{
    public class ClientBuilderTests
    {
        [Test]
        public void AppendVersion()
        {
            // legacy format
            Assert.That(
                new RtmClientBuilder("ws://xxx.api.com/v2", "appkey").Url,
                Is.EqualTo("ws://xxx.api.com/v2?appkey=appkey"));

            // legacy format without slash with additional path component
            Assert.That(
                new RtmClientBuilder("ws://xxx.api.com/foo/v2", "appkey").Url,
                Is.EqualTo("ws://xxx.api.com/foo/v2?appkey=appkey"));

            // new format with slash
            Assert.That(
                new RtmClientBuilder("ws://xxx.api.com/", "appkey").Url,
                Is.EqualTo("ws://xxx.api.com/v2?appkey=appkey"));

            // new format without slash
            Assert.That(
                new RtmClientBuilder("ws://xxx.api.com", "appkey").Url,
                Is.EqualTo("ws://xxx.api.com/v2?appkey=appkey"));

            // v1 not supported
            try
            {
                new RtmClientBuilder("ws//xxx.api.com/v1", "appkey");
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
