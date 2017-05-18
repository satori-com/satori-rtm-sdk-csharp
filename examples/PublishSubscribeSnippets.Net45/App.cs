using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

namespace PublishSubscribeSnippets
{
    class App
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
            MainAsync().Wait();
        }

        static async Task MainAsync() 
        {
            IRtmClient client = new RtmClientBuilder(endpoint, appkey)
                .Build();

            #region Publishing
            {
                try 
                {
                    var message = new Animal 
                    {
                        Who = "zebra",
                        Where =  new float[] {34.134358f, -118.321506f}
                    };

                    RtmPublishReply reply = await client.Publish("my-channel", message, Ack.Yes);
                }
                catch(PduException ex) 
                {
                    Pdu pdu = ex.Pdu;
                    // the RTM returned reply with negative action
                }
                catch(DisconnectedException ex)
                {
                    // the call didn't succeed because client disconnected
                }
                catch(TooManyRequestsException ex)
                {
                    // the client's offline action queue exceeds RtmClientBuilder.PendingActionQueueLength
                }
            }
            #endregion

            #region Simple Subscription
            {
                var observer = new SubscriptionObserver();

                observer.OnEnterSubscribed += (ISubscription sub) => 
                    Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

                observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
                    Console.WriteLine("Subscription error {0}: {1}", err.Code, err.Reason);

                observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
                {
                    foreach(JToken msg in data.Messages)
                    {
                        Console.WriteLine("Got message: " + msg);
                    }
                };

                client.CreateSubscription("my-channel", SubscriptionModes.Simple, observer);
            }
            #endregion


            #region Subscription with a Filter
            {
                var observer = new SubscriptionObserver();

                observer.OnEnterSubscribed += (ISubscription sub) => 
                    Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

                observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
                    Console.WriteLine("Subscription error {0}: {1}", err.Code, err.Reason);

                observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
                {
                    foreach(JToken msg in data.Messages)
                    {
                        Console.WriteLine("Got message: " + msg);
                    }
                };

                var cfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
                {
                    Filter = "SELECT a, MAX(b) FROM my-channel GROUP BY a"
                };
                client.CreateSubscription("my-subscription-id", cfg);
            }
            #endregion

            #region Multiple Subscriptions to a Channel
            {
                var observer = new SubscriptionObserver();

                observer.OnEnterSubscribed += (ISubscription sub) => 
                    Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

                observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
                    Console.WriteLine("Subscription error {0}: {1}", err.Code, err.Reason);

                observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
                {
                    foreach(JToken msg in data.Messages)
                    {
                        Console.WriteLine("Got message: " + msg);
                    }
                };

                var groupCfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
                {
                    Filter = "SELECT a, MAX(b) FROM my-channel GROUP BY a"
                };
                client.CreateSubscription("my-subscription-id-1", groupCfg);

                var allCfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
                {
                    Filter = "SELECT * FROM my-channel"
                };
                client.CreateSubscription("my-subscription-id-2", allCfg);
            }
            #endregion

            #region Subscription with History (count)
            {
                var observer = new SubscriptionObserver();

                observer.OnEnterSubscribed += (ISubscription sub) => 
                    Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

                observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
                    Console.WriteLine("Subscription error {0}: {1}", err.Code, err.Reason);

                observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
                {
                    foreach(JToken msg in data.Messages)
                    {
                        Console.WriteLine("Got message: " + msg);
                    }
                };

                var cfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
                {
                    History = new RtmSubscribeHistory { Count = 10 }
                };
                client.CreateSubscription("my-channel", cfg);
            }
            #endregion

            #region Subscription with History (age)
            {
                var observer = new SubscriptionObserver();

                observer.OnEnterSubscribed += (ISubscription sub) => 
                    Console.WriteLine("Subscribed to: " + sub.SubscriptionId);

                observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) => 
                    Console.WriteLine("Subscription error {0}: {1}", err.Code, err.Reason);

                observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) => 
                {
                    foreach(JToken msg in data.Messages)
                    {
                        Console.WriteLine("Got message: " + msg);
                    }
                };

                var cfg = new SubscriptionConfig(SubscriptionModes.Simple, observer)
                {
                    History = new RtmSubscribeHistory { Age = 60 /* seconds */ }
                };
                client.CreateSubscription("my-channel", cfg);
            }
            #endregion
        }
    }
}