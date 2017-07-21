using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appKey = "YOUR_APPKeY";
    // Role and secret are optional: replace only if you need to authenticate.
    const string role = "YOUR_ROLE";
    const string roleSecretKey = "YOUR_SECRET";

    const string channel = "animals";

    // Sample model to publish to RTM.
    // This class represents the following raw json structure:
    // {
    //   "who": "zebra",
    //  "where": [34.134358,-118.321506]
    // }
    class Animal
    {
        [JsonProperty("who")]
        public string Who { get; set; }

        [JsonProperty("where")]
        public float[] Where { get; set; }

        public override string ToString()
        {
            return string.Format("[Animal: Who={0}, Where={1}]"
                                 , Who, string.Join(",", Where ?? new float[0]));
        }
    }

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        // Change logging levels to increase verbosity. Default level is Warning. 
        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Warning);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Warning);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Warning);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Warning);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Warning);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Warning); 

        //check if the role is set to authenticate or not
        var toAuthenticate = !new string[]{"YOUR_ROLE", "", null}.Contains(role);

        Console.WriteLine("RTM connection config:\n" +
                          "\tendpoint='{0}'\n" +
                          "\tappkey='{1}'\n" +
                          "\tauthenticate?={2}", endpoint, appKey, toAuthenticate); 

        var builder = new RtmClientBuilder(endpoint, appKey);

        if (toAuthenticate)
        {
            builder.SetRoleSecretAuthenticator(role, roleSecretKey);
        }

        IRtmClient client = builder.Build();

        // Hook up to client lifecycle events 
        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");
        client.OnError += ex => Console.WriteLine("RTM client failed: " + ex.Message);

        client.Start();

        // We create a subscription observer object to receive callbacks
        // for incoming messages, subscription state changes and errors. 
        // The same observer can be shared between several subscriptions. 
        var observer = new SubscriptionObserver();

        // when subscription is established (confirmed by RTM)
        observer.OnEnterSubscribed += sub =>
            Console.WriteLine("Subscribed to the channel: " + sub.SubscriptionId);;

        observer.OnSubscribeError += (sub, ex) => 
            Console.WriteLine("Subscribing failed. " +
                              "Check channel subscribe permissions in Dev Portal. \n" + ex.Message); 

        observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) 
            => Console.WriteLine("Subscription error " + err.Code + ": " + err.Reason);

        observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
        {
            // Messages arrive in an array
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

        // At this point, the client may not yet be connected to Satori RTM. 
        // If the client is not connected, the SDK internally queues the subscription request and
        // will send it once the client connects
        client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

        PublishLoop(client).Wait();
    }

    private static async Task PublishLoop(IRtmClient client)
    {
        // Publish messages every 2 seconds

        var random = new Random();
        while (true)
        {
            try 
            {
                var message = new Animal 
                {
                    Who = "zebra",
                    Where =  new float[] {
                        34.134358f + (float)random.NextDouble() / 100, 
                        -118.321506f + (float)random.NextDouble() / 100
                    }
                };

                // At this point, the client may not yet be connected to Satori RTM.
                // If the client is not connected, the SDK internally queues the publish request and
                // will send it once the client connects
                RtmPublishReply reply = await client.Publish(channel, message, Ack.Yes);
                Console.WriteLine("Animal is published: {0}", message);
            }
            catch(PduException ex) 
            {
                Console.WriteLine("Failed to publish. RTM replied with the error {0}: {1}", ex.Error.Code, ex.Error.Reason);
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed to publish: " + ex.Message);
            }

            await Task.Delay(millisecondsDelay: 2000);
        }
    }
}