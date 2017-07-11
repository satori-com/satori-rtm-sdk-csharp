using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKEY";

    class Animal
    {
        [JsonProperty("who")]
        public string Who { get; set; }

        [JsonProperty("where")]
        public float[] Where { get; set; }
    }

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        IRtmClient client = new RtmClientBuilder(endpoint, appkey)
            .Build();

        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");
        client.OnError += ex => Console.WriteLine("Error occurred: " + ex.Message);

        client.Start();

        var observer = new SubscriptionObserver();

        observer.OnEnterSubscribed += (ISubscription sub) => 
            Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

        observer.OnLeaveSubscribed += (ISubscription sub) => 
            Console.WriteLine("Unsubscribed from: " + sub.SubscriptionId);
        
        observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
        {
            foreach(JToken jToken in data.Messages)
            {
                if (data.SubscriptionId == "zebras")
                    Console.WriteLine("Got a zebra: " + jToken);
                else 
                    Console.WriteLine("Got a count: " + jToken);
            }
        };

        observer.OnSubscribeError += (ISubscription sub, Exception err) => 
            Console.WriteLine("Failed to subscribe: " + err.Message);
        
        observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
            Console.WriteLine("Subscription failed. RTM sent the unsolicited error {0}: {1}", err.Code, err.Reason);

        var zebraCfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
        {
            Filter = "SELECT * FROM `animals` WHERE who = 'zebra'"
        };
        client.CreateSubscription("zebras", zebraCfg);

        var statsCfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
        {
            // Count of messages by animal kind 
            // RTM view aggregates and delivers counts every second (default period)
            Filter = "SELECT count(*) as count, who FROM `animals` GROUP BY who"
        };
        client.CreateSubscription("stats", statsCfg);

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}