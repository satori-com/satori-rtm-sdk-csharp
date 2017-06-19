using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Satori.Rtm;
using Satori.Rtm.Client;

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKEY";

    const string channel = "animals";

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

        client.OnEnterConnected += cn => Console.WriteLine("Connected to RTM");
        client.OnError += ex => Console.WriteLine("Error occurred: " + ex.Message);

        client.Start();

        try 
        {
            var message = new Animal 
            {
                Who = "zebra",
                Where =  new float[] {34.134358f, -118.321506f}
            };

            RtmPublishReply reply = await client.Publish(channel, message, Ack.Yes);
        }
        catch(PduException ex) 
        {
            Console.WriteLine("Publishing failed because RTM replied with the error {0}: {1}", ex.Error.Code, ex.Error.Reason);
        }
        catch(DisconnectedException ex)
        {
            Console.WriteLine("Publishing failed because the client was disconnected: " + ex.Message);
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Publishing failed: " + ex.Message);
        }

        // Stop and clean up the client before exiting the program
        await client.Dispose();
    }
}