using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        //TODO Load credentials from external place. For now, 
        // just set endpoint, appkey and other properties here
        static RtmConfig()
        {
            Instance = new RtmConfig
            {
            };
        }
    }
}
