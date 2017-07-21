using System;
using System.Diagnostics;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;

// IMPORTANT: the namespace should be the same as rest of tests reside
namespace Satori.Rtm.Test
{
    [SetUpFixture]
    public class OneTimeSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            UnhandledExceptionWatcher.OnError += exn =>
            {
                Assert.Fail("Unhandled exception in event handler: " + exn.Message);
            };

            DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);
        }
    }
}
