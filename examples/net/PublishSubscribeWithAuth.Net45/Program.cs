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
    const string appkey = "YOUR_APPKEY";
    const string role = "YOUR_ROLE";
    const string roleSecret = "YOUR_SECRET";

    const string channel = "YOUR_CHANNEL";

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
            .SetRoleSecretAuthenticator(role, roleSecret)
            .Build();

        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");
        client.OnError += ex => Console.WriteLine("Error occurred: " + ex.Message);
            
        client.Start();

        var observer = new SubscriptionObserver();

        observer.OnEnterSubscribed += (ISubscription sub) => 
            Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

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
                    Console.WriteLine("Failed to parse incoming message: {0}", ex);
                }
            }
        };

        observer.OnSubscribeError += (ISubscription sub, Exception err) => 
        {
            var rtmEx = err as SubscribeException;
            if (rtmEx != null) 
                Console.WriteLine("Failed to subscribe because RTM replied with the error {0}: {1}", rtmEx.Error.Code, rtmEx.Error.Reason);
            else 
                Console.WriteLine("Failed to subscribe: " + err.Message);
        };

        observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
            Console.WriteLine("Subscription failed because RTM sent the unsolicited error {0}: {1}", err.Code, err.Reason);

        client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

        PublishLoop(client).Wait();
    }

    private static async Task PublishLoop(IRtmClient client)
    {
        var random = new Random();

        while(true)
        {
            try 
            {
                var message = new Animal 
                {
                    Who = "zebra",
                    Where =  new float[] {
                        34.134358f + (float)random.NextDouble(), 
                        -118.321506f + (float)random.NextDouble()
                    }
                };

                RtmPublishReply reply = await client.Publish(channel, message, Ack.Yes);
                Console.WriteLine("Published successfully");
            }
            catch(PduException ex) 
            {
                Console.WriteLine("Failed to publish because RTM replied with the error {0}: {1}", ex.Error.Code, ex.Error.Reason);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to publish: " + ex.Message);
            }

            await Task.Delay(millisecondsDelay: 2000);
        }
    }
}