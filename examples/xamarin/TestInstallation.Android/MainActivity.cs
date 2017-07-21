using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Satori.Rtm.Client;

namespace TestInstallation
{

    [Activity(Label = "TestInstallation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        const string endpoint = "YOUR_ENDPOINT";
        const string appkey = "YOUR_APPKeY";

        const string tag = "TestInstallation";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

            client.OnEnterConnected += cn 
                => Log.Info(tag, "Connected to Satori RTM!");

            client.OnError += ex =>
                Log.Error(tag, "Failed to connect: " + ex.Message);

            client.Start();

            // Publish, subscribe, and perform other operations here

        }
    }

}