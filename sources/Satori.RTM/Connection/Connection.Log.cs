#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    public partial class DefaultLoggers
    {
        public static Logger Connection { get; set; } = new DefaultLogger(
            "Satori.Rtm.Connection");
    }
}