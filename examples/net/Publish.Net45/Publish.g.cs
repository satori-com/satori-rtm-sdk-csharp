using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Satori.Rtm;
using Satori.Rtm.Client;

// Objects of the Animal type are used in the SDK examples to show
// how to send or recieve custom types over RTM 
class Animal
{
    [JsonProperty("who")]
    public string Who { get; set; }

    [JsonProperty("where")]
    public float[] Where { get; set; }
}

class Program
{
    const string endpoint = "YOUR_ENDPOINT";
    const string appkey = "YOUR_APPKEY";

    static void Main()
    {
        // Log messages from SDK to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        IRtmClient client = new RtmClientBuilder(endpoint, appkey).Build();

        client.OnEnterConnected += cn => Console.WriteLine("Connected to Satori RTM!");

        client.OnError += ex => 
            Console.WriteLine("Failed to connect: " + ex.Message);

        client.Start();

        try 
        {
            var message = new Animal 
            {
                Who = "zebra",
                Where =  new float[] { 34.134358f, -118.321506f }
            };
        
            RtmPublishReply reply = await client.Publish("animals", message, Ack.Yes);
            Console.WriteLine("Publish confirmed");
        }
        catch(PduException ex) 
        {
            Console.WriteLine("Failed to publish. RTM replied with the error {0}: {1}", ex.Error.Code, ex.Error.Reason);
        }
        catch (Exception ex) 
        {
            Console.WriteLine("Failed to publish: " + ex.Message);
        }

        Console.ReadKey();

        // Stop and clean up the client before exiting the program
        client.Dispose().Wait();
    }
}