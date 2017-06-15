using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKEY";

    const string channel = "animals";

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

        client.OnEnterConnected += cn => Console.WriteLine("Connected to RTM");
        client.OnError += ex => Console.WriteLine("Error occurred: " + ex.Message);

        client.Start();

        var observer = new SubscriptionObserver();

        observer.OnEnterSubscribed += (ISubscription sub) => 
            Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

        observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
        {
            foreach(JToken jToken in data.Messages)
            {
                Animal msg = jToken.ToObject<Animal>();
                string text = string.Format("Who? {0}. Where? At {1},{2}", msg.Who, msg.Where[0], msg.Where[1]);
                Console.WriteLine("Got message: " + text);
            }
        };

        observer.OnSubscribeError += (ISubscription sub, Exception err) => 
        {
            var rtmEx = err as SubscribeException;
            if (rtmEx != null) 
                Console.WriteLine("Subscribing failed because RTM replied with the error {0}: {1}", rtmEx.Error.Code, rtmEx.Error.Reason);
            else 
                Console.WriteLine("Subscribing failed: " + err.Message);
        };

        observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
            Console.WriteLine("Subscription failed because RTM sent the error {0}: {1}", err.Code, err.Reason);

        // Assume that someone already publishes animals to the channel 'animals'
        client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}