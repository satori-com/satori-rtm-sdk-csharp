using Satori.Common;
using Satori.Rtm;
using Satori.Rtm.Client;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Satori.Rtm.Logger;

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
        string endpoint = "<YOUR RTM ENDPOINT>";
        string appKey = "<YOUR APP KEY>";
        client = new RtmClientBuilder(endpoint, appKey).Build();

        var observer = new SubscriptionObserver();
        
        observer.OnEnterSubscribed += sub => 
        {
            Debug.Log("Client subscribed to " + sub.SubscriptionId);
            client.Publish(channel, "Hello World!", Ack.Yes)
                .ContinueWith(t => {
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
            string msg = data.Messages[0].ToString();

            // call on main thread
            UnityMainThreadDispatcher.Enqueue(delegate
            {
                var textObj = GameObject.Find("MyText");
                TextMesh mesh = (TextMesh)textObj.GetComponent(typeof(TextMesh));
                mesh.text = msg;
            });
        };
        
        client.CreateSubscription(channel, SubscriptionModes.Simple, observer);
    }

    void OnApplicationFocus(bool pauseStatus)
    {
        if (client == null) {
            return;
        }

        if (pauseStatus) {
            client.Stop();
        } else {
            client.Start();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // execute dispatched actions
        client.Dispatch();
    }

    void OnDestroy()
    {
        if (client != null) {
            client.Dispose();
        }
    }
}
