using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public class RtmConfig
    {
        public static readonly RtmConfig Instance = new RtmConfig();

        public readonly string Endpoint = "YOUR_ENDPOINT";
        public readonly string AppKey = "YOUR_APPKeY";
        public readonly string Role = "YOUR_ROLE";
        public readonly string RoleSecretKey = "YOUR_SECRET";
        public readonly string RestrictedChannel = "YOUR_CHANNEL";
    }
}
