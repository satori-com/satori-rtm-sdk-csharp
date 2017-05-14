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

        System.Diagnostics.Trace.Listeners.Add(UnityTraceListener.Instance);

        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

        UnhandledExceptionWatcher.OnError += exn =>
        {
            Debug.LogError("Unhandled exception in event handler: " + exn);
        };

        UnityMainThreadDispatcher.Init();

        try
        {
            string endpoint = "<YOUR RTM ENDPOINT>";
            string appKey = "<YOUR APP KEY>";
            client = new RtmClientBuilder(endpoint, appKey).Build();

            var observer = new SubscriptionObserver();

            observer.OnEnterSubscribed += sub =>
            {
                Debug.Log("Client subscribed to " + sub.SubscriptionId);

                var msg = new Event
                {
                    Who = "zebra",
                    Where = new float[] { 34.134358f, -118.321506f }
                };

                client.Publish(channel, msg, Ack.Yes)
                    .ContinueWith(t =>
                    {
                        if (t.Exception == null)
                            Debug.Log("Published successfully!");
                        else
                            Debug.LogError("Publishing failed: " + t.Exception);
                    });
            };

            observer.OnLeaveSubscribed += sub => Debug.Log("Unsubscribed from " + sub.SubscriptionId);

            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                Debug.Log("Data received");
                JToken jToken = data.Messages[0];
                Event msg = jToken.ToObject<Event>();
                string greeting = string.Format("Hello {0}!", msg.Who);

                // call on main thread
                UnityMainThreadDispatcher.Enqueue(() => UpdateText(greeting));
            };

            client.CreateSubscription(channel, SubscriptionModes.Simple, observer);

            client.Start();
        } catch (Exception ex) {
            Debug.LogError("Setting up RTM client failed. " + ex);
        }
    }

    void UpdateText(string msg)
    {
        var textObj = GameObject.Find("MyText");
        TextMesh mesh = (TextMesh)textObj.GetComponent(typeof(TextMesh));
        mesh.text = msg;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (client == null) {
            return;
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
