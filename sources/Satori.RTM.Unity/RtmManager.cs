using Satori.Common;
using UnityEngine;

namespace Satori.Rtm.Client
{
    public class RtmManager
    {
        public static readonly RtmManager Instance = new RtmManager();

        private readonly SingleThreadDispatcher dispatcher = new SingleThreadDispatcher();
        private IRtmClient client;
        
        public IRtmClient Client => client;

        private static Logger Log => DefaultLoggers.Client;

        public RtmManager()
        {
            System.Diagnostics.Trace.Listeners.Add(UnityTraceListener.Instance);

            UnhandledExceptionWatcher.OnError += exn =>
            { 
                Log.E(exn, "Unhandled exception in event handler");
            };

            Log.V("Creating RTM manager behavior");
            var gameObj = new GameObject("Satori.Rtm.RtmManager.Behavior");
            gameObj.AddComponent<RtmManager.Behavior>();
        }

        public IRtmClient Register(IRtmClient client)
        {
            Log.V("Updating RTM client in RTM manager: {0}", client);
            this.client?.Dispose();
            this.client = client;
            this.client?.Start();
            return this.client;
        }

        public IRtmClient Register(string endpoint, string appKey)
        {
            var client = new RtmClientBuilder(endpoint, appKey)
                .SetDispatcher(dispatcher)
                .Build();
            return Register(client);
        }

        public IRtmClient Register(string endpoint, string appKey, string role, string roleSecret)
        {
            var client = new RtmClientBuilder(endpoint, appKey)
                .SetRoleSecretAuthenticator(role, roleSecret)
                .SetDispatcher(dispatcher)
                .Build();
            return Register(client);
        }

        public void Unregister()
        {
            Register(null);
        }

        private class Behavior : MonoBehaviour
        {
            private Logger Log => RtmManager.Log;

            private void Awake()
            {
                Log.V("RTM manager behavior is awakening");
                DontDestroyOnLoad(this);
            }

            private void Start()
            {
                Log.V("RTM manager behavior is starting");
                var client = RtmManager.Instance.Client;
                if (client != null)
                {
                    client.Start();
                }
            }

            private void Update()
            {
                RtmManager.Instance.dispatcher.ProcessQueue();
            }

            private void OnDestroy()
            {
                Log.V("Destroying RTM manager behavior");
                RtmManager.Instance.Unregister();
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                Log.V("RTM manager behavior is Handling app pause");

                var client = RtmManager.Instance.Client;
                if (client == null)
                {
                    return;
                }

                if (pauseStatus)
                {
                    client.Stop();
                }
                else
                {
                    client.Start();
                }
            }
        }
    }
}
