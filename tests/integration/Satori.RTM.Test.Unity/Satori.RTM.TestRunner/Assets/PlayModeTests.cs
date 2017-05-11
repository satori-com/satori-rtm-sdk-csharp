using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Satori.Rtm.Test;

public class ClientBuilderTestsWrapper
{
    [Test]
    public void AppendVersion()
    {
        new ClientBuilderTests().AppendVersion();
    }
}

public class AuthTestsWrapper
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

public class ClientTestsWrapper
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

public class ConnectionTestsWrapper
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

public class FilterTestsWrapper
{
    [Test]
    public void TwoSubscriptionsWithDifferentNames()
    {
        new FilterTests().TwoSubscriptionsWithDifferentNames().Wait();
    }

    [Test]
    public void BothChannelAndFilterSpecified()
    {
        new FilterTests().BothChannelAndFilterSpecified().Wait();
    }
}

public class OutOfSyncTestsWrapper
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

public class ReadWriteStorageTestsWrapper
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

public class SubscriptionTestsWrapper
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
}

