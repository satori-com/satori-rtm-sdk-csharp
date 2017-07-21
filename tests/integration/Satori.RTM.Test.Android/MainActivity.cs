using System.Reflection;
using Android.App;
using Android.OS;
using NUnit.Framework;
using Satori.Common;
using Satori.Rtm.Client;
using Xamarin.Android.NUnitLite;

namespace Satori.Rtm.Test
{
    // IMPORTANT: In order the app finds rtm config, setup Run Configuration:
    // - Go to Options > Run > Configurations > Default 
    // - Select Main Activity as activity in Explicit Intent
    // - Go to Intent Extras tab and add "RTM_CONFIG"="<JSON RTM config>"
    // NOTE: Special characters, including ':', are not supported. See RtmConfig.android.cs
    [Activity(Label = "Satori.Rtm.Test", MainLauncher = true)]
    public class MainActivity : TestSuiteActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            UnhandledExceptionWatcher.OnError += exn =>
            {
                Assert.Fail("Unhandled exception in event handler: " + exn.Message);
            };

            DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
            DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);
            
            // tests can be inside the main assembly
            AddTest(Assembly.GetExecutingAssembly());

            // Once you called base.OnCreate(), you cannot add more assemblies.
            base.OnCreate(bundle);
        }
    }
}
