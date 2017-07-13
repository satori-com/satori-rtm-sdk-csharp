using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public class RtmConfigImpl : RtmConfig
    {
        //TODO Load credentials from external place. For now, 
        // just set endpoint, appkey and other properties here
        public static void Init()
        {
            Instance = new RtmConfig
            {
                Endpoint = "YOUR_ENDPOINT",
                AppKey = "YOUR_APPKEY",
                AuthRoleName = "YOUR_ROLE",
                AuthRoleSecretKey = "YOUR_SECRET",
                AuthRestrictedChannel = "YOUR_CHANNEL"
            };
        }
    }
}
