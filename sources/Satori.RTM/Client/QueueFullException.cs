#pragma warning disable 1591

using System;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// This exception occurs when the client's offline action queue exceeds 
    /// <see cref="RtmClientBuilder.PendingActionQueueLength"/> 
    /// </summary>
    public class QueueFullException : Exception
    {
        public QueueFullException(string message) : base(message)
        {
        }
    }
}
