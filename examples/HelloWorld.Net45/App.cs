using System;
using Satori.Rtm.Client;

namespace HelloWorld
{
    class App
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Enter parameters: <endpoint> <app key>");
                return;
            }

            var endpoint = args[0];
            var appKey = args[1];

            // create client
            var client = new RtmClientBuilder(endpoint, appKey).Build();

            Start(client);

            Console.ReadKey(true);
            client.Dispose().Wait();
        }

        public static async void Start(IRtmClient client)
        {
            var channel = $"{Guid.NewGuid():N}";

            // handler for subscription channel data
            var observer = new SubscriptionObserver();
            observer.OnSubscriptionData += (subscription, data) =>
            {
                // print messages
                foreach (var m in data.Messages)
                {
                    Console.WriteLine(m);
                }
            };

            // run client
            await client.Start();

            // publish message and remember its position
            var reply = await client.Publish(channel, "hello world!", Ack.Yes);

            // create subscription from remembered position
            await client.CreateSubscription(
                channel,
                new SubscriptionConfig(SubscriptionModes.Advanced)
                {
                    Position = reply.Position,
                    Observer = observer
                });
        }
    }
}