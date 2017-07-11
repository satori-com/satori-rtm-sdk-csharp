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
                try 
                {
                    Animal msg = jToken.ToObject<Animal>();
                    Console.WriteLine("Got message: Who? {0}. Where? At {1},{2}", msg.Who, msg.Where[0], msg.Where[1]);
                } 
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to deserialize the incoming message: {0}", ex.Message);
                }
            }
        };

        observer.OnSubscribeError += (ISubscription sub, Exception err) => 
        {
            var rtmEx = err as SubscribeException;
            if (rtmEx != null) 
            {
                Console.WriteLine("Failed to subscribe. RTM replied with the error {0}: {1}", rtmEx.Error.Code, rtmEx.Error.Reason);

                if (rtmEx.Error.Code == AuthErrorCodes.AuthorizationDenied)
                    Console.WriteLine("Authorization denied. Check channel permissions in Dev Portal.");
            }
            else
            {
                Console.WriteLine("Failed to subscribe: " + err.Message);
            }
        };

        observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
            Console.WriteLine("Subscription failed. RTM sent the unsolicited error {0}: {1}", err.Code, err.Reason);

        client.CreateSubscription("animals", SubscriptionModes.Simple, observer);

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}