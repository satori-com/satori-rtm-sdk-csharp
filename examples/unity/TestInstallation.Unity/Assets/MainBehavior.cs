using Newtonsoft.Json;
using Satori.Rtm;
using Satori.Rtm.Client;
using UnityEngine;

public class MainBehavior : MonoBehaviour {

    string endpoint = "YOUR_ENDPOINT";
    string appKey = "YOUR_APPKeY";

    void Start () {
        IRtmClient client = RtmManager.Instance.Register(endpoint, appKey);

        client.OnEnterConnected += cn => 
            Debug.Log("Connected to Satori RTM!");

        client.OnError += ex =>
            Debug.Log("Failed to connect: " + ex.Message);

        client.Start();

        // Publish, subscribe, and perform other operations here

    }
}
