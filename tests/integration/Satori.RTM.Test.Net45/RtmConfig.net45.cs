using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Satori.Rtm.Test
{
    public partial class RtmConfig
    {
        private const string RtmConfigEnvVar = "RTM_CONFIG";

        public static void Init()
        {
            var path = System.Environment.GetEnvironmentVariable(RtmConfigEnvVar);
            if (string.IsNullOrEmpty(path))
            {
                char s = Path.DirectorySeparatorChar;
                path = Assembly.GetExecutingAssembly().Location
                               + $"{s}..{s}..{s}..{s}..{s}..{s}..{s}..{s}credentials.json";
                path = Path.GetFullPath(path);
                if (!File.Exists(path))
                {
                    throw new InvalidOperationException(RtmConfigEnvVar +
                                                        " environment variable should be set to rtm config path. Alternatively, "
                                                        + " put the rtm config to " + path);
                }
            }

            using (var reader = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                var cfg = (RtmConfig)serializer.Deserialize(reader, typeof(RtmConfig));
                Instance = cfg;
            }
        }
    }
}
