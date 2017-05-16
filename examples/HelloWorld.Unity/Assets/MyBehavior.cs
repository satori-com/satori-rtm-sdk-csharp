using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Common;
using Satori.Rtm;
using Satori.Rtm.Client;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Satori.Rtm.Logger;

// Sample model to publish to RTM
// This class represents the following raw json structure:
// {
//   "who": "zebra",
//   "where": [34.134358,-118.321506]
// }
class Event
{
    [JsonProperty("who")]
    public string Who { get; set; }

    [JsonProperty("where")]
    public float[] Where { get; set; }
}

public class MyBehavior : MonoBehaviour
{
    IRtmClient client;
    string channel = "my_channel";

    // Use this for initialization
    void Start ()
    {
        Debug.Log("My behavior started");

        // Log messages from SDK to the Unity Console
        System.Diagnostics.Trace.Listeners.Add(UnityTraceListener.Instance);

        // Change logging levels. Default level is Warning. 
        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

        // Unhandled exceptions in user callbacks can be observed here. 
        // Do not throw exceptions in user callbacks. 
        UnhandledExceptionWatcher.OnError += exn =>
        {
            Debug.LogError("Unhandled exception in event handler: " + exn);
        };

        // Initialize dispatcher that allows performing actions on the main thread. 
        // Initialization must be done in the main thread. 
        // This method can be called several times. 
        UnityMainThreadDispatcher.Init();

        try
        {
            // Replace placeholders for endpoint and app key with your values 
            // from the Dev Portal at https://developer.satori.com 
            string endpoint = "<YOUR_ENDPOINT>";
            string appKey = "<YOUR_APPKEY>";

            client = new RtmClientBuilder(endpoint, appKey).Build();

            // Hook up to client lifecycle events 
            client.OnEnterConnecting += () => UpdateText("Connecting...");
            client.OnEnterConnected += cn => UpdateText("Connected");
            client.OnLeaveConnected += cn => UpdateText("Disconnected");
            client.OnError += ex => UpdateText("Error occurred");

            // Create subscription observer to observe channel subscription events 
            var observer = new SubscriptionObserver();

            observer.OnEnterSubscribed += sub =>
            {
                Debug.Log("Client subscribed to " + sub.SubscriptionId);

                var msg = new Event
                {
                    Who = "zebra",
                    Where = new float[] { 34.134358f, -118.321506f }
                };

                // Publish message to the subscribed channel
                client.Publish(channel, msg, Ack.Yes)
                    .ContinueWith(t =>
                        {
                            if (t.Exception == null)
                                Debug.Log("Published successfully!");
                            else
                                Debug.LogError("Publishing failed: " + t.Exception);
                    });
            };

            observer.OnLeaveSubscribed += sub 
                => Debug.Log("Unsubscribed from " + sub.SubscriptionId);

            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err) 
                => Debug.LogError("Subscription error " + err.Code + ": " + err.Reason);

            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                Debug.Log("Message received");

                JToken jToken = data.Messages[0];
                Event msg = jToken.ToObject<Event>();
                string greeting = string.Format("Hello {0}!", msg.Who);

                UpdateText(greeting);
            };

            // Subscribe to the channel. Because client is not connected to Satori RTM, 
            // subscription request will be queued. This request will be sent when 
            // the client is connected. 
            client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

            // Connect to Satori RTM. If connection is dropped, the client will 
            // reconnect automatically. 
            client.Start();
        } catch (Exception ex) {
            Debug.LogError("Setting up RTM client failed. " + ex);
        }
    }

    // Update displayed text
    void UpdateText(string msg)
    {
        // call on main thread
        UnityMainThreadDispatcher.Enqueue (() => 
            {
                var textObj = GameObject.Find("MyText");
                var mesh = (TextMesh)textObj.GetComponent(typeof(TextMesh));
                mesh.text = msg; 
            });
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (client == null) {
            return;
        }

        // Disconnect client from Satori RTM when the app is not active
        if (pauseStatus) {
            client.Stop(); 
        } else {
            client.Start();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (client != null) {
            // execute dispatched actions
            client.Dispatch();
        }
    }

    void OnDestroy()
    {
        if (client != null) {
            client.Dispose();
        }
    }
}
