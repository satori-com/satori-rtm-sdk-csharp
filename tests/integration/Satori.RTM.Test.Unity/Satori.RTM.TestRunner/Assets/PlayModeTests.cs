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
