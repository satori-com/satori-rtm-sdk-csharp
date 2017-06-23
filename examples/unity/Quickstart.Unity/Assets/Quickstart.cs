// Unity Quickstart (c# SDK)
// For tutorial purposes, the app subscribes to the same channel that it publishes 
// a message to. So it receives its own published message. This allows end-to-end 
// illustration of data flow with just a single client. 
// In the real world, publisher and subscriber are typically different clients. 
// 
// The app displays received message on the screen. 

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Rtm;
using Satori.Rtm.Client;
using UnityEngine;
using UnityEngine.UI;

using Logger = Satori.Rtm.Logger;
using Random = UnityEngine.Random;

// Animal class represents user message to publish to RTM
// Message example in json: 
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

public class Quickstart : MonoBehaviour
{
    // Replace these values with your project's credentials
    // from Dev Portal (https://developer.satori.com/#/projects).
    string endpoint = "YOUR_ENDPOINT";
    string appKey = "YOUR_APPKEY";

    // Role and secret are optional: replace only if you need to authenticate. 
    string role = "YOUR_ROLE";
    string secret = "YOUR_SECRET";

    string channel = "animals";

    // Communication with RTM is done via RTM client which implements IRtmClient 
    // interface. 
    IRtmClient client;
    
    // Animal is published in the Update method if this flag is set
    bool publishAnimal;
    
    // Time in seconds when the last animal was published
    float lastPublished = 0;

    Text mesh;

    // Use this for initialization
    void Start()
    {
        var textObj = GameObject.Find("Text"); 
        mesh = (Text)textObj.GetComponent(typeof(Text));
        mesh.text = ""; 

        // Change logging levels. Default level is Warning. 
        DefaultLoggers.Dispatcher.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Serialization.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Connection.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.Client.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtm.SetLevel(Logger.LogLevel.Verbose);
        DefaultLoggers.ClientRtmSubscription.SetLevel(Logger.LogLevel.Verbose);

        try
        {
            ShowText(string.Format("RTM connection config:\n\tendpoint='{0}'\n\tappkey='{1}'", endpoint, appKey)); 

            //check if the role is set to authenticate or not
            var toAuthenticate = !new string[]{"YOUR_ROLE", "", null}.Contains(role);
            ShowText(string.Format("Starting RTM client... Authenticate? {0} (role={1})", toAuthenticate, role)); 

            if (toAuthenticate)
            {
                client = RtmManager.Instance.Register(endpoint, appKey, role, secret);
            } else 
            {   //no authentication (default role)
                client = RtmManager.Instance.Register(endpoint, appKey);
            }
            
            // Hook up to client connectivity state transitions 
            client.OnEnterConnecting += () => ShowText("Connecting...");
            client.OnEnterConnected += cn => ShowText("Connected to Satori RTM");
            client.OnLeaveConnected += cn => ShowText("Disconnected");
            client.OnError += ex => ShowText("ERROR:\n" + ex);

            // We create a subscription observer object to receive callbacks
            // for incoming messages, subscription state changes and errors. 
            // The same observer can be shared between several subscriptions. 
            var observer = new SubscriptionObserver();

            observer.OnSubscribeError += (sub, ex) => 
            {
                ShowText("ERROR: subscribing failed. " +
                    "Check channel subscribe permissions in Dev Portal. \n" + ex); 
            };

            // when subscription is establshed (confirmed by RTM)
            observer.OnEnterSubscribed += sub =>
            {
                ShowText("Subscribed to " + sub.SubscriptionId);

                // Instruct the Update method to publish next message. 
                // We publish a message to the same channel we have subscribed to
                // and so will be receiving our own message. 
                // (This is a contrived example just for tutorial purposes)
                publishAnimal = true;
            };

            // when the subscription ends 
            observer.OnLeaveSubscribed += sub => ShowText("Unsubscribed from " + sub.SubscriptionId);

            // when a subscription error occurs 
            observer.OnSubscriptionError += (ISubscription sub, RtmSubscriptionError err)
                => ShowText("ERROR: subscription " + err.Code + ": " + err.Reason);

            // when messages arrive
            observer.OnSubscriptionData += (ISubscription sub, RtmSubscriptionData data) =>
            {
                // Note: sub.SubscriptionId is the channel name
                ShowText("Message received from channel " + sub.SubscriptionId);
                
                // Messages arrive in an array
                foreach(JToken jToken in data.Messages) {
                    ShowText(jToken.ToString()); 
                    Animal msg = jToken.ToObject<Animal>();
                    string text = string.Format("Who? {0}. Where? at {1},{2}", msg.who, msg.where[0], msg.where[1]);
                    ShowText(text);
                }
            };

            // At this point, the client may not yet be connected to Satori RTM. 
            // If the client is not connected, the SDK internally queues the subscription request and
            // will send it once the client connects
            client.CreateSubscription(channel, observer);
        }
        catch(System.UriFormatException uriEx)
        {
            ShowText("ERRROR: invalid connection credentials. Check endpoint and appkey"); 
        }
        catch (Exception ex)
        {
            ShowText("ERROR: setting up RTM client failed.\n" + ex);
        }
    }
    
    // This method is called by Unity on every frame 
    void Update()
    {
        // Publish animals every ~2 seconds 
        if (publishAnimal && Time.time > lastPublished + 2 /*sec*/) {
            publishAnimal = false;
            lastPublished = Time.time;
            PublishAnimal();
        }
    }

    void PublishAnimal()
    {
        var animal = new Animal
        {
            who = "zebra",
            where = new float[] {
                        34.134358f + Random.value/100,
                        -118.321506f + Random.value/100
                    }
        };

        ShowText("publishing " + animal.who);
        client.Publish(channel, animal,
            onSuccess: reply => {
                ShowText("Published successfully: " + animal.ToString());
                // Instruct the Update method to publish next message
                publishAnimal = true;
            },
            onFailure: ex => {
                ShowText("ERROR: Publishing failed:\n" + ex);
                // Instruct the Update method to publish next message
                publishAnimal = true;
            });
    }

    // log and show text on screen
    void ShowText(string msg)
    {
		Debug.Log(msg);
        mesh.text += msg + "\n";
    }
}
