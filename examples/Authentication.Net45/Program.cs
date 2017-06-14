using System;
using System.Diagnostics;
using System.Threading;
using Satori.Rtm;
using Satori.Rtm.Client;

namespace Authentication
{
    class Program
    {
        const string endpoint = "YOUR_ENDPOINT";
        const string appkey = "YOUR_APPKEY";
        const string role = "YOUR_ROLE";
        const string roleSecret = "YOUR_SECRET";

        static void Main()
        {
            // Log messages from SDK to the console
            Trace.Listeners.Add(new ConsoleTraceListener());

            // This event will be signalled when the client is connected to RTM
            var connectedEvent = new ManualResetEvent(initialState: false);

            IRtmClient client = new RtmClientBuilder(endpoint, appkey)
                .SetRoleSecretAuthenticator(role, roleSecret)
                .Build();

            client.OnEnterConnected += cn => 
            {
                Console.WriteLine("Successfully connected and authenticated");
                connectedEvent.Set();
            };

            client.OnError += ex => 
            {
                var authEx = ex as AuthException;
                if (authEx != null) 
                {
                    Console.WriteLine("Authentication failed: " + ex.Message);
                } else 
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                }
            };

            client.Start();

            connectedEvent.WaitOne(TimeSpan.FromSeconds(30));

            // Dispose the client before exiting the program
            client.Dispose().Wait();
        }
    }
}