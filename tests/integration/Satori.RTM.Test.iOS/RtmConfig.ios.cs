using System;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        private const string RtmConfigEnvVar = "RTM_CONFIG";

        public static void Init()
        {
            var jsonCfg = Environment.GetEnvironmentVariable(RtmConfigEnvVar);
            if (string.IsNullOrEmpty(jsonCfg))
            {
                throw new InvalidOperationException(RtmConfigEnvVar + " environment variable should be set to rtm config json");
            }

            var cfg = JsonConvert.DeserializeObject<RtmConfig>(jsonCfg);
            Instance = cfg;
        }
    }
}