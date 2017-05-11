using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        static RtmConfig()
        {
            Instance = new RtmConfig
            {
            };
        }
    }
}
