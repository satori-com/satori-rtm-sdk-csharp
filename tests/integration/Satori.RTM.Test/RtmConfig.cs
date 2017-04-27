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

        [JsonProperty("auth_role_name")]
        public string AuthRoleName { get; set; }

        [JsonProperty("auth_role_secret_key")]
         public string AuthRoleSecretKey { get; set; }

        [JsonProperty("auth_restricted_channel")]
        public string AuthRestrictedChannel { get; set; }
    }
}
