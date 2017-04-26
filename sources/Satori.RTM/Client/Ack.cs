#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Sets up the acknowledgement mode for an operation. 
    /// That means, if operation fails, the SDK won't trigger any error event.
    /// </summary>
    public enum Ack
    {
        /// <summary>
        /// Operation doesn't need the acknowledgement from the RTM service.
        /// </summary>
        No,

        /// <summary>
        /// Operation should get the acknowledgement from the RTM service.
        /// </summary>
        Yes
    }
}