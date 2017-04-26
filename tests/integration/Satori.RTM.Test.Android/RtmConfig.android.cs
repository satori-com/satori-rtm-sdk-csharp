using System;
using Android.Content;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        private const string RtmConfigKey = "RTM_CONFIG";

        public static void Init(Intent intent)
        {
            if (intent == null) 
            {
                throw new InvalidOperationException("Intent with RTM_CONFIG in extras should be passed to the MainActivity activity. ");
            }

            var jsonCfg = intent.Extras.GetString(RtmConfigKey);
            if (string.IsNullOrEmpty(jsonCfg))
            {
                throw new InvalidOperationException("RTM_CONFIG in the intent extras must be populated");    
            }

            // colons must be escaped becuase they are not supported by Run Configuration in Xamarin Studio
            jsonCfg = jsonCfg.Replace("%3A", ":");
            var cfg = JsonConvert.DeserializeObject<RtmConfig>(jsonCfg);
            Instance = cfg;
        }
    }
}
