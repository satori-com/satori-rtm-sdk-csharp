using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

// Objects of the Animal type are used in the SDK examples to show
// how to send or recieve custom types over RTM 
class Animal
{
    [JsonProperty("who")]
    public string Who { get; set; }

    [JsonProperty("where")]
    public float[] Where { get; set; }
}

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKeY";

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");

        client.OnError += ex => 
            Console.WriteLine("Failed to connect: " + ex.Message);

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
                    Console.WriteLine("Got animal {0}: {1}", msg.Who, jToken);
                } 
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to handle the incoming message: {0}", ex.Message);
                }
            }
        };
        
        observer.OnSubscribeError += (ISubscription sub, Exception err) => 
        {
            var rtmEx = err as SubscribeException;
            if (rtmEx != null) 
            {
                Console.WriteLine("Failed to subscribe. RTM replied with the error {0}: {1}", rtmEx.Error.Code, rtmEx.Error.Reason);
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