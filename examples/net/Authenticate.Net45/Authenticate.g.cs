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
    const string appkey = "YOUR_APPKeY";
    const string role = "YOUR_ROLE";
    const string roleSecretKey = "YOUR_SECRET";

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        IRtmClient client = new RtmClientBuilder(endpoint, appkey)
            .SetRoleSecretAuthenticator(role, roleSecretKey)
            .Build();

        client.OnEnterConnected += cn => 
            Console.WriteLine("Connected to Satori RTM and authenticated as " + role);

        client.OnError += ex => 
        {
            var authEx = ex as AuthException;
            if (authEx != null) 
                Console.WriteLine("Failed to authenticate: " + ex.Message);
            else 
                Console.WriteLine("Error occurred: " + ex.Message);
        };

        client.Start();

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}