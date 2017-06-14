using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

namespace HelloWorld
{
    class App
    {

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
        }

        static void Main(string[] args)
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

            if (args.Length != 2)
            {
                Console.WriteLine("Enter parameters: <endpoint> <app key>");
                return;
            }

            var endpoint = args[0];
            var appKey = args[1];

            IRtmClient client = new RtmClientBuilder(endpoint, appKey).Build();

            // Hook up to client lifecycle events 
            client.OnEnterConnecting += () => Console.WriteLine("Connecting...");
            client.OnEnterConnected += cn => Console.WriteLine("Connected");
            client.OnLeaveConnected += cn => Console.WriteLine("Disconnected");
            client.OnError += ex => Console.Error.WriteLine("Error occurred: " + ex.Message);

            client.Start();

            // This event will be signalled once published message is received
            var ev = new ManualResetEvent(initialState: false);

            RunAsync(client, ev);

            ev.WaitOne(TimeSpan.FromSeconds(30));

            // Dispose the client before exiting the app
            client.Dispose().Wait();
        }

        private static async void RunAsync(IRtmClient client, ManualResetEvent ev)
        {
            var channel = "my_channel";

            // Create subscription observer to observe channel subscription events 
            var observer = new SubscriptionObserver();

            observer.OnEnterSubscribed += sub =>
            {
                Console.WriteLine("Client subscribed to " + sub.SubscriptionId);

                var msg = new Animal
                {
                    Who = "zebra",
                    Where = new float[] { 34.134358f, -118.321506f }
                };

                // Publish message to the subscribed channel
                client.Publish(channel, msg, Ack.Yes)
                    .ContinueWith(t =>
                    {
                        if (t.Exception == null)
                            Console.WriteLine("Published successfully!");
                        else
                            Console.Error.WriteLine("Publishing failed: " + t.Exception);
                    });
            };

            observer.OnLeaveSubscribed += sub 
                => Console.WriteLine("Unsubscribed from " + sub.SubscriptionId);

            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) 
                => Console.Error.WriteLine("Subscription error " + err.Code + ": " + err.Reason);

            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                Console.WriteLine("Message received");

                foreach(JToken token in data.Messages) 
                {
                    Animal msg = token.ToObject<Animal>();
                    Console.WriteLine("Hello {0} at ({1})!", msg.Who, string.Join(", ", msg.Where));
                }

                // Signal the manual reset event to exit the app
                ev.Set();
            };

            // Subscribe to the channel. Because client is not connected to Satori RTM yet, 
            // subscription request will be queued. This request will be sent when 
            // the client is connected. 
            client.CreateSubscription(channel, SubscriptionModes.Simple, observer);
        }
    }
}