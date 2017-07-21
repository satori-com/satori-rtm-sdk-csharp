using System;

namespace Satori.Rtm.Test
{
    public class IntegrationTestsBase
    {
        protected const int DefaultTestCaseTimeoutMsec = 60 * 1000;

        protected readonly RtmConfig Config = RtmConfig.Instance;

        protected IntegrationTestsBase()
        {
            if (Config == null)
            {
                throw new InvalidOperationException("RTM config is null");
            }
        }

        public static string GenerateRandomChannelName(string prefix = "")
        {
            return $"{prefix}{Guid.NewGuid():N}";
        }
    }
}
