using System;
using System.Diagnostics;
using System.Threading;
using Satori.Rtm.Client;

namespace TestInstallation
{
    class Program
    {
        const string endpoint = "YOUR_ENDPOINT";
        const string appkey = "YOUR_APPKEY";

        static void Main()
        {
            // Log messages from SDK to the console
            Trace.Listeners.Add(new ConsoleTraceListener());

            // This event will be signalled when the client is connected to RTM
            var connectedEvent = new ManualResetEvent(initialState: false);

            IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

            client.OnEnterConnected += cn => 
            {
                Console.WriteLine("Connected to RTM!");
                connectedEvent.Set();
            };

            client.OnError += ex => 
                Console.Error.WriteLine("Failed to connect: " + ex.Message);

            client.Start();

            connectedEvent.WaitOne(TimeSpan.FromSeconds(30));

            // Dispose the client before exiting the app
            client.Dispose().Wait();
        }
    }
}
