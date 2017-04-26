using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        public static RtmConfig Instance { get; private set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("appkey")]
        public string AppKey { get; set; }

        [JsonProperty("superuser_role_secret")]
        public string SuperuserRoleSecret { get; set; }
    }
}
