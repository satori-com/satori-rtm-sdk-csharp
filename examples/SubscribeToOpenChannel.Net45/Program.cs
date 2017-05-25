using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;

namespace SubscribeToOpenChannel
{
    class Program
    {
        const string endpoint = "wss://open-data.api.satori.com";
        const string appkey = "YOUR_APPKEY";
        const string channel = "OPEN_CHANNEL";

        static void Main()
        {
            // Log messages from SDK to the console
            Trace.Listeners.Add(new ConsoleTraceListener());

            IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

            client.OnEnterConnected += cn => Console.WriteLine("Connected to RTM");
            client.OnError += ex => Console.Error.WriteLine("Error occurred: " + ex.Message);

            // This event will be signalled when the client receives data
            var dataReceivedEvent = new ManualResetEvent(initialState: false);

            // Create subscription observer to observe channel subscription events 
            var observer = new SubscriptionObserver();

            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                foreach(JToken msg in data.Messages) 
                {
                    Console.WriteLine("Got message: " + msg);
                }

                dataReceivedEvent.Set();
            };

            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) 
                => Console.Error.WriteLine("Subscription error " + err.Code + ": " + err.Reason);
            
            client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

            client.Start();

            dataReceivedEvent.WaitOne(TimeSpan.FromSeconds(30));

            // Dispose the client before exiting the app
            client.Dispose().Wait();
        }
    }
}