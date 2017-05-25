using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

namespace PublishSubscribe
{
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

            client.OnError += ex => Console.Error.WriteLine("Error occurred: " + ex.Message);

            client.Start();

            // This event will be signalled when the client subscribes to the channel
            var subscribedEvent = new ManualResetEvent(initialState: false);

            // This event will be signalled when the client receives data
            var dataReceivedEvent = new ManualResetEvent(initialState: false);

            // Create subscription observer to observe channel subscription events 
            var observer = new SubscriptionObserver();

            observer.OnEnterSubscribed += sub => subscribedEvent.Set();

            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) 
                => Console.Error.WriteLine("Subscription error " + err.Code + ": " + err.Reason);

            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                foreach(JToken token in data.Messages) 
                {
                    Animal msg = token.ToObject<Animal>();
                    Console.WriteLine("Hello {0} at ({1})!", msg.Who, string.Join(", ", msg.Where));
                }

                dataReceivedEvent.Set();
            };

            client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

            subscribedEvent.WaitOne(TimeSpan.FromSeconds(30));

            var message = new Animal
            {
                Who = "zebra",
                Where = new float[] { 34.134358f, -118.321506f }
            };

            // Publish message to the subscribed channel
            client.Publish(channel, message, Ack.Yes)
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                        Console.Error.WriteLine("Publishing failed: " + t.Exception);
                });

            dataReceivedEvent.WaitOne(TimeSpan.FromSeconds(30));

            // Dispose the client before exiting the app
            client.Dispose().Wait();
        }
    }
}