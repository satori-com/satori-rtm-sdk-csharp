#pragma warning disable 1591

using System;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// This exception occurs when the client's offline action queue exceeds 
    /// <see cref="RtmClientBuilder.PendingActionQueueLength"/> 
    /// </summary>
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(string message) : base(message)
        {
        }
    }
}
