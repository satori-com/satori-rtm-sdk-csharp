using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKeY";
    const string channel = "OPEN_CHANNEL";

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();
        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");
        client.Start();

        // Create subscription observer to observe channel subscription events 
        var observer = new SubscriptionObserver();

        observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
        {
            foreach(JToken jToken in data.Messages)
            {
                Console.WriteLine("Got message: " + jToken);
            }
        };
        
         client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}