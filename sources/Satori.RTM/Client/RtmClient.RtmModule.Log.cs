#pragma warning disable 1591

using System.Diagnostics;

namespace Satori.Rtm.Client
{
    public partial class DefaultLoggers
    {
        public static Logger ClientRtm { get; set; } = new DefaultLogger(
            "Satori.Rtm.Client.RtmModule");
    }

    internal partial class RtmClient
    {
        public partial class RtmModule
        {
            public Logger Log { get; } = DefaultLoggers.ClientRtm;
        }
    }
}