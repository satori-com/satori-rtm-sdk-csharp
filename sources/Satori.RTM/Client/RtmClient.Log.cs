#pragma warning disable 1591

using System.Diagnostics;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Default loggers. Default logger writes to <c>System.Diagnostics.Trace</c>. 
    /// </summary>
    /// <remarks>You can customize existing loggers or provide your own.</remarks>
    public partial class DefaultLoggers
    {
        public static Logger Client { get; set; } = new DefaultLogger(
            "Satori.Rtm.Client");
    }

    internal partial class RtmClient
    {
        public Logger Log { get; } = DefaultLoggers.Client;
    }
}
