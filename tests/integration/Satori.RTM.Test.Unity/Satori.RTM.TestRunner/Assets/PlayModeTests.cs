using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Satori.Rtm.Test;
using System.Threading;
using Satori.Rtm;
using Satori.Rtm.Client;
using Satori.Common;
using Logger = Satori.Rtm.Logger;

public class UnityTestBase
{
    private static int initialized;

    protected UnityTestBase()
    {
        int initialized = Interlocked.CompareExchange(ref UnityTestBase.initialized, value: 1, comparand: 0);
        if (initialized == 0)
        {
            Init();
        }
    }

    static void Init()
    {
        Debug.Log("Initializing logging...");

        System.Diagnostics.Trace.Listeners.Add(UnityTraceListener.Instance);

        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

        UnhandledExceptionWatcher.OnError += exn =>
        {
            Debug.LogError("Unhandled exception in event handler: " + exn);
        };

    }

}

public class ClientBuilderTestsWrapper : UnityTestBase
{
    [Test]
    public void AppendVersion()
    {
        new ClientBuilderTests().AppendVersion();
    }
}

public class AuthTestsWrapper : UnityTestBase
{
    [Test]
    public void GenerateHash()
    {
        new AuthTests().GenerateHash();
    }

    [Test]
    public void AuthenticateWithSuccess()
    {
        new AuthTests().AuthenticateWithSuccess().Wait();
    }

    [Test]
    public void FailToAuthenticateWithBadKey()
    {
        new AuthTests().FailToAuthenticateWithBadKey().Wait();
    }

    [Test]
    public void FailToAuthenticateWithBadRole()
    {
        new AuthTests().FailToAuthenticateWithBadRole().Wait();
    }

    [Test]
    public void SubscribeToRestrictedChannelWhenAuthorized()
    {
        new AuthTests().SubscribeToRestrictedChannelWhenAuthorized().Wait();
    }

    [Test]
    public void PublishToRestrictedChannelWhenNotAuthorized()
    {
        new AuthTests().PublishToRestrictedChannelWhenNotAuthorized().Wait();
    }
}

public class ClientTestsWrapper : UnityTestBase
{
    [Test]
    public void NoStateTransitionsInCallbacks()
    {
        new ClientTests().NoStateTransitionsInCallbacks().Wait();
    }

    [Test]
    public void RestartOnSubscriptionError()
    {
        new ClientTests().RestartOnSubscriptionError().Wait();
    }

    [Test]
    public void StartStopAndAtomicity()
    {
        new ClientTests().StartStopAndAtomicity().Wait();
    }

    [Test]
    public void StopOnConnected()
    {
        new ClientTests().StopOnConnected().Wait();
    }

    [Test]
    public void TwoClients()
    {
        new ClientTests().TwoClients().Wait();
    }

    [Test]
    public void ThrowWhenTooManyRequests()
    {
        new ClientTests().ThrowWhenTooManyRequests().Wait();
    }

    [Test]
    public void ThrowWhenDisconnectedAndMaxPendingQueueLengthIsZero()
    {
        new ClientTests().ThrowWhenDisconnectedAndMaxPendingQueueLengthIsZero().Wait();
    }
}

public class ConnectionTestsWrapper : UnityTestBase
{
    [Test]
    public void ReconnectWhenConnectionDropped()
    {
        new ConnectionTests().ReconnectWhenConnectionDropped().Wait();
    }

    [Test]
    public void ReconnectIfConnectionCannotBeEstablished()
    {
        new ConnectionTests().ReconnectIfConnectionCannotBeEstablished().Wait();
    }

    [Test]
    public void ReconnectIntervals()
    {
        new ConnectionTests().ReconnectIntervals();
    }
}

public class FilterTestsWrapper : UnityTestBase
{
	[Test]
    public void TwoSubscriptionsWithDifferentNames()
    {
        new FilterTests().TwoSubscriptionsWithDifferentNames().Wait();
    }

	[Test]
    public void CreateFilterWIthExistingSubscriptionId()
    {
        new FilterTests().CreateFilterWIthExistingSubscriptionId().Wait();
    }
}

public class OutOfSyncTestsWrapper : UnityTestBase
{
    [Test]
    public void FastForwardOnOutOfSyncWhenSimpleMode()
    {
        new OutOfSyncTests().FastForwardOnOutOfSyncWhenSimpleMode().Wait();
    }

    [Test]
    public void FailOnOutOfSyncWhenAdvancedMode()
    {
        new OutOfSyncTests().FailOnOutOfSyncWhenAdvancedMode().Wait();
    }
}

public class ReadWriteStorageTestsWrapper : UnityTestBase
{
    [Test]
    public void ReadAfterWrite()
    {
        new ReadWriteStorageTests().ReadAfterWrite().Wait();
    }

    [Test]
    public void WriteTwiceAndRead()
    {
        new ReadWriteStorageTests().WriteTwiceAndRead().Wait();
    }

    [Test]
    public void WriteDeleteRead()
    {
        new ReadWriteStorageTests().WriteDeleteRead().Wait();
    }

    [Test]
    public void ReadOldMessage()
    {
        new ReadWriteStorageTests().ReadOldMessage().Wait();
    }

    [Test]
    public void WriteNullShouldBeOk()
    {
        new ReadWriteStorageTests().WriteNullShouldBeOk().Wait();
    }
}

public class SecureConnectionTestsWrapper : UnityTestBase
{
    [Test]
    public void Expired()
    {
        new SecureConnectionTests().Expired().Wait();
    }

    [Test]
    public void WrongHost()
    {
        new SecureConnectionTests().WrongHost().Wait();
    }

    [Test]
    public void SelfSigned()
    {
        new SecureConnectionTests().SelfSigned().Wait();
    }

    [Test]
    public void UntrustedRoot()
    {
        new SecureConnectionTests().UntrustedRoot().Wait();
    }
}

public class SubscriptionTestsWrapper : UnityTestBase
{
    [Test]
    public void AutoDeleteSubscriptionOnDispose()
    {
        new SubscriptionTests().AutoDeleteSubscriptionOnDispose().Wait();
    }

    [Test]
    public void AutoResubscribe()
    {
        new SubscriptionTests().AutoResubscribe().Wait();
    }

    [Test]
    public void DelayedPublish()
    {
        new SubscriptionTests().DelayedPublish().Wait();
    }

    [Test]
    public void ManualResubscribe()
    {
        new SubscriptionTests().ManualResubscribe().Wait();
    }

    [Test]
    public void RepeatFirstMessage()
    {
        new SubscriptionTests().RepeatFirstMessage().Wait();
    }

    [Test]
    public void RepeatSecondMessage()
    {
        new SubscriptionTests().RepeatSecondMessage().Wait();
    }

    [Test]
    public void SubscribeWithSimpleModeAndPosition()
    {
        new SubscriptionTests().SubscribeWithSimpleModeAndPosition().Wait();
    }

    [Test]
    public void SubscribeWithAdvancedModeAndPosition()
    {
        new SubscriptionTests().SubscribeWithAdvancedModeAndPosition().Wait();
    }

    [Test]
    public void RestartOnUnsubscribeError()
    {
        new SubscriptionTests().RestartOnUnsubscribeError().Wait();
    }

    [Test]
    public void CreateSubscriptionThatAlreadyExists()
    {
        new SubscriptionTests().CreateSubscriptionThatAlreadyExists().Wait();
    }

    [Test]
    public void CreateSubscriptionToRestrictedChannel()
    {
        new SubscriptionTests().CreateSubscriptionToRestrictedChannel().Wait();
    }

    [Test]
    public void RemoveSubscriptionThatDoesntExist()
    {
        new SubscriptionTests().RemoveSubscriptionThatDoesntExist().Wait();
    }
}
