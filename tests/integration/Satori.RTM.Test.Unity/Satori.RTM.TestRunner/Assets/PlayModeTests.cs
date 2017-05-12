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
		int initialized = Interlocked.CompareExchange (ref UnityTestBase.initialized, value: 1, comparand: 0);
		if (initialized == 0) {
			Init ();
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

    [UnityTest]
    public IEnumerator AuthenticateWithSuccess()
    {
        return new AuthTests().AuthenticateWithSuccess().Await();
    }

    [UnityTest]
    public IEnumerator FailToAuthenticateWithBadKey()
    {
        return new AuthTests().FailToAuthenticateWithBadKey().Await();
    }

    [UnityTest]
    public IEnumerator FailToAuthenticateWithBadRole()
    {
        return new AuthTests().FailToAuthenticateWithBadRole().Await();
    }

    [UnityTest]
    public IEnumerator SubscribeToRestrictedChannelWhenAuthorized()
    {
        return new AuthTests().SubscribeToRestrictedChannelWhenAuthorized().Await();
    }

    [UnityTest]
    public IEnumerator PublishToRestrictedChannelWhenNotAuthorized()
    {
        return new AuthTests().PublishToRestrictedChannelWhenNotAuthorized().Await();
    }
}

public class ClientTestsWrapper : UnityTestBase
{
    [UnityTest]
    public IEnumerator NoStateTransitionsInCallbacks()
    {
        return new ClientTests().NoStateTransitionsInCallbacks().Await();
    }

    [UnityTest]
    public IEnumerator RestartOnSubscriptionError()
    {
        return new ClientTests().RestartOnSubscriptionError().Await();
    }

    [UnityTest]
    public IEnumerator StartStopAndAtomicity()
    {
        return new ClientTests().StartStopAndAtomicity().Await();
    }

    [UnityTest]
    public IEnumerator StopOnConnected()
    {
        return new ClientTests().StopOnConnected().Await();
    }

    [UnityTest]
    public IEnumerator TwoClients()
    {
        return new ClientTests().TwoClients().Await();
    }

    [UnityTest]
    public IEnumerator ThrowWhenTooManyRequests()
    {
        return new ClientTests().ThrowWhenTooManyRequests().Await();
    }

    [UnityTest]
    public IEnumerator ThrowWhenDisconnectedAndMaxPendingQueueLengthIsZero()
    {
        return new ClientTests().ThrowWhenDisconnectedAndMaxPendingQueueLengthIsZero().Await();
    }
}

public class ConnectionTestsWrapper : UnityTestBase
{
    [UnityTest]
    public IEnumerator ReconnectWhenConnectionDropped()
    {
        return new ConnectionTests().ReconnectWhenConnectionDropped().Await();
    }

    [UnityTest]
    public IEnumerator ReconnectIfConnectionCannotBeEstablished()
    {
        return new ConnectionTests().ReconnectIfConnectionCannotBeEstablished().Await();
    }

    [Test]
    public void ReconnectIntervals()
    {
        new ConnectionTests().ReconnectIntervals();
    }
}

public class FilterTestsWrapper : UnityTestBase
{
	[UnityTest]
    public IEnumerator TwoSubscriptionsWithDifferentNames()
    {
        return new FilterTests().TwoSubscriptionsWithDifferentNames().Await();
    }

	[UnityTest]
    public IEnumerator BothChannelAndFilterSpecified()
    {
        return new FilterTests().BothChannelAndFilterSpecified().Await();
    }
}

public class OutOfSyncTestsWrapper : UnityTestBase
{
    [UnityTest]
    public IEnumerator FastForwardOnOutOfSyncWhenSimpleMode()
    {
        return new OutOfSyncTests().FastForwardOnOutOfSyncWhenSimpleMode().Await();
    }

    [UnityTest]
    public IEnumerator FailOnOutOfSyncWhenAdvancedMode()
    {
        return new OutOfSyncTests().FailOnOutOfSyncWhenAdvancedMode().Await();
    }
}

public class ReadWriteStorageTestsWrapper : UnityTestBase
{
    [UnityTest]
    public IEnumerator ReadAfterWrite()
    {
        return new ReadWriteStorageTests().ReadAfterWrite().Await();
    }

    [UnityTest]
    public IEnumerator WriteTwiceAndRead()
    {
        return new ReadWriteStorageTests().WriteTwiceAndRead().Await();
    }

    [UnityTest]
    public IEnumerator WriteDeleteRead()
    {
        return new ReadWriteStorageTests().WriteDeleteRead().Await();
    }

    [UnityTest]
    public IEnumerator ReadOldMessage()
    {
        return new ReadWriteStorageTests().ReadOldMessage().Await();
    }

    [UnityTest]
    public IEnumerator WriteNullShouldBeOk()
    {
        return new ReadWriteStorageTests().WriteNullShouldBeOk().Await();
    }
}

public class SubscriptionTestsWrapper : UnityTestBase
{
    [UnityTest]
    public IEnumerator AutoDeleteSubscriptionOnDispose()
    {
        return new SubscriptionTests().AutoDeleteSubscriptionOnDispose().Await();
    }

    [UnityTest]
    public IEnumerator AutoResubscribe()
    {
        return new SubscriptionTests().AutoResubscribe().Await();
    }

    [UnityTest]
    public IEnumerator DelayedPublish()
    {
        return new SubscriptionTests().DelayedPublish().Await();
    }

    [UnityTest]
    public IEnumerator ManualResubscribe()
    {
        return new SubscriptionTests().ManualResubscribe().Await();
    }

    [UnityTest]
    public IEnumerator RepeatFirstMessage()
    {
        return new SubscriptionTests().RepeatFirstMessage().Await();
    }

    [UnityTest]
    public IEnumerator RepeatSecondMessage()
    {
        return new SubscriptionTests().RepeatSecondMessage().Await();
    }

    [UnityTest]
    public IEnumerator SubscribeWithSimpleModeAndPosition()
    {
        return new SubscriptionTests().SubscribeWithSimpleModeAndPosition().Await();
    }

    [UnityTest]
    public IEnumerator SubscribeWithAdvancedModeAndPosition()
    {
        return new SubscriptionTests().SubscribeWithAdvancedModeAndPosition().Await();
    }

    [UnityTest]
    public IEnumerator RestartOnUnsubscribeError()
    {
        return new SubscriptionTests().RestartOnUnsubscribeError().Await();
    }
}
	