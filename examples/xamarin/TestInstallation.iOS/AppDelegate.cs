using System;
using Foundation;
using UIKit;
using Satori.Rtm.Client;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKeY";

    public override UIWindow Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

        client.OnEnterConnected += cn
            => Console.WriteLine("Connected to Satori RTM!");

        client.OnError += ex =>
            Console.WriteLine("Failed to connect: " + ex.Message);

        client.Start();

        // Publish, subscribe, and perform other operations here

        return true;
    }
}