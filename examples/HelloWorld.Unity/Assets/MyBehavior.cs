// Unity Primer (c# SDK)
// For tutorial purposes, the app subscribes to the same channel that it publishes 
// a message to. So it receives its own published message. This allows end-to-end 
// illustration of data flow with just a single client. 
// In the real world, publisher and subscriber are typically different clients. 
// 
// The app displays received message on the screen. 

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;
using System;
using UnityEngine;
using Logger = Satori.Rtm.Logger;

// Sample model to publish to RTM
// This class represents the following raw json structure:
// {
//   "who": "zebra",
//   "where": [34.134358,-118.321506]
// }
class Animal
{
    [JsonProperty("who")]
    public string who { get; set; }

    [JsonProperty("where")]
    public float[] where { get; set; }
}

public class MyBehavior : MonoBehaviour
{
    // Replace these values with your project's credentials
    // from Dev Portal (https://developer.satori.com/#/projects).
    string endpoint = "YOUR_ENDPOINT";
    string appKey = "YOUR_APPKEY";

    //role and secret are optional. Setting these to null means no authentication.
    string role = "YOUR_ROLE";
    string secret = "YOUR_SECRET";

    string channel = "animals";

    // Use this for initialization
    void Start()
    {
        Debug.Log("My behavior started");

        // Change logging levels. Default level is Warning. 
        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

        try
        {
            // Communication with RTM is done via RTM client which implements IRtmClient 
            // interface. RTM client can be created by calling RtmClientBuilder.Build() 
            // or RtmManager.Register() methods. 
            // RtmManager.Register() method simplifies initialization of the client: 
            // 1. Creates the client 
            // 2. Configures the client to work on the main thread, so 
            // that all user callbacks are called on the main thread
            // 3. Starts the client 
            // RtmManager stops the client when the app is paused, and 
            // starts it again when the app becomes active.  
            IRtmClient client;
            if (!string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(secret))
                client = RtmManager.Instance.Register(endpoint, appKey, role, secret);
            else
                client = RtmManager.Instance.Register(endpoint, appKey);
            
            // Hook up to client lifecycle state transitions 
            client.OnEnterConnecting += () => UpdateText("Connecting...");
            client.OnEnterConnected += cn => UpdateText("Connected");
            client.OnLeaveConnected += cn => UpdateText("Disconnected");
            client.OnError += ex =>
            {
                Debug.LogError("Client error occurred: " + ex);
                UpdateText("Error occurred");
            };

            // We create a subscription observer object in order to receive callbacks
            // for incoming data, state changes and errors. 
            // The same observer can be shared between several subscriptions. 
            var observer = new SubscriptionObserver();

            observer.OnEnterSubscribed += sub =>
            {
                Debug.Log("Client subscribed to " + sub.SubscriptionId);

                var animal = new Animal
                {
                    who = "zebra",
                    where = new float[] { 34.134358f, -118.321506f }
                };

                // Publish message to the channel. 
                // We must publish the message *only after* we get confirmation that 
                // subscription is established. This is not a general principle: 
                // most applications doesn't care to receive the messages they publish.
                // 
                // In case of publishing, there's no observer object involved because
                // the process is simpler: we're guaranteed to receive exactly one
                // reply callback and need only to inspect it to see if it succeeded or
                // failed.
                client.Publish(channel, animal)
                    .ContinueWith(t =>
                        {
                            if (t.Exception == null)
                                Debug.Log("Published successfully!");
                            else
                                Debug.LogError("Publishing failed: " + t.Exception);
                    });
            };

            // This callback is called when the subscription ends 
            observer.OnLeaveSubscribed += sub
                => Debug.Log("Unsubscribed from " + sub.SubscriptionId);

            // This callback is called when a subscription error occurrs 
            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err)
                => Debug.LogError("Subscription error " + err.Code + ": " + err.Reason);

            // This callback allows us to observe incoming messages
            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                // Note: sub.SubscriptionId equals to the channel name used for publishing the messages
                Debug.Log("Message received from subscription " + sub.SubscriptionId);
                
                // Messages arrive in an array:
                // we only expect one (first) message
                JToken jToken = data.Messages[0];
                Animal msg = jToken.ToObject<Animal>();
                string greeting = string.Format("Hello {0}!", msg.who);

                UpdateText(greeting);
            };

            // At this moment, the client may not be connected yet to Satori RTM. 
            // If the client is not connected, the subscription request will be queued. 
            // This request will be sent when the client becomes connected. 
            client.CreateSubscription(channel, observer);
        }
        catch (Exception ex)
        {
            Debug.LogError("Setting up RTM client failed. " + ex);
        }
    }

    // Update displayed text
    void UpdateText(string msg)
    {
        Debug.Log("Updating displayed text to '" + msg + "'");

        var textObj = GameObject.Find("MyText");
        var mesh = (TextMesh)textObj.GetComponent(typeof(TextMesh));
        mesh.text = msg;
    }
}
