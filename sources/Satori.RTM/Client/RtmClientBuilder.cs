#pragma warning disable 1591

using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Use the RtmClientBuilder to create an instance of the <see cref="IRtmClient"/>  interface and set the client properties.
    /// </summary>
    /// <remarks>After you create the client, use the client instance to publish messages and subscribe to channels.</remarks>
    public class RtmClientBuilder
    {
        public const string RtmVersion = "v2";
        public const int DefaultPendingQueueLength = 1024;

        /// <summary>
        /// Creates an instance of a client builder for specific endpoint and application key.
        /// </summary>
        /// <remarks>Use the Developer Portal to obtain the appropriate application keys.</remarks>
        public RtmClientBuilder(string endpoint, string appKey)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("endpoint can't be null or empty");
            }

            if (string.IsNullOrEmpty(appKey))
            {
                throw new ArgumentException("appKey can't be null or empty");
            }

            string fullEndpoint = endpoint;

            var pattern = @"/(v\d+)$";
            var match = Regex.Match(endpoint, pattern);
            if (match.Success)
            {
                var ver = match.Groups[1].Value;
                if (ver != RtmVersion)
                {
                    throw new ArgumentException($"{ver} is not supported");
                }

                Log.W("Specifying a version as a part of the endpoint is deprecated. "
                      + "Please remove the {0} from {1}", ver, endpoint);
            }
            else
            {
                if (!endpoint.EndsWith("/", StringComparison.Ordinal))
                {
                    fullEndpoint += "/";
                }

                fullEndpoint += RtmVersion;
            }

            fullEndpoint += $"?appkey={appKey}";

            Url = new Uri(fullEndpoint).ToString();
        }

        /// <summary>
        /// Gets or sets the connector which will be used by the client to connect to RTM endpoint. 
        /// </summary>
        /// <remarks>Connector should provide a connection which is already connected.</remarks>
        public Func<string, ConnectionOptions, CancellationToken, Task<IConnection>> Connector { get; set; }

        /// <summary>
        /// Gets or sets an authentication provider for the client.
        /// </summary>
        /// <remarks>
        /// Use this method if you want to authenticate an application user 
        /// when you establish the connection.
        /// </remarks>
        public Func<IConnection, Task<JToken>> Authenticator { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher. All transport events and user callbacks are executed 
        /// in dispatcher. Dispatcher must execute tasks in order. 
        /// </summary>
        /// <remarks>If not specified, it will be created automatically.</remarks>
        public IDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Gets URL to RTM endpoint.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Options to use when establishing a connection
        /// </summary>
        public ConnectionOptions ConnectionOptions { get; } = new ConnectionOptions();

        /// <summary>
        /// Gets or sets the minimum time period to wait between reconnection attempts. 
        /// </summary>
        /// <remarks>
        /// The following formula is used to calculate the reconnect interval 
        /// after the client disconnects from the RTM service for any reason:
        /// <code>
        /// Min(MinReconnectInterval * (2 ^ (attempt_number), MaxReconnectInterval)
        /// </code>
        /// The timeout period between each successive connection attempt increases, 
        /// but starts with this value.
        /// </remarks>
        public TimeSpan MinReconnectInterval { get; set; } = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// Gets or sets the maximum time period to wait between reconnection attempts. 
        /// </summary>
        /// <remarks>
        /// The following formula is used to calculate the reconnect interval 
        /// after the client disconnects from the RTM service for any reason:
        /// <code>
        /// Min(MinReconnectInterval * (2 ^ (attempt_number), MaxReconnectInterval)
        /// </code>
        /// The timeout period between each successive connection attempt increases, 
        /// but never exceeds this value.
        /// </remarks>
        public TimeSpan MaxReconnectInterval { get; set; } = TimeSpan.FromMilliseconds(120000);

        /// <summary>
        /// Gets or sets the length of pending action queue. 
        /// When a client is disconnected, the pending action queue temporarily
        /// stores messages until the client reconnects.
        /// </summary>
        /// <remarks>Default value is <see cref="DefaultPendingQueueLength"/> </remarks>
        public int PendingActionQueueLength { get; set; } = DefaultPendingQueueLength;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public Logger Log { get; } = DefaultLoggers.Client;

        /// <summary>
        /// See <see cref="Url"/>. 
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetUrl(string url)
        {
            Url = url;
            return this;
        }

        /// <summary>
        /// Sets the address and credentials of the HTTPS proxy server.
        /// </summary>
        /// <remarks>
        /// This functionality is available when running on .NET Framework. 
        /// Proxy options are ignored on Mono (including Xamarin and Unity).
        /// </remarks>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetHttpsProxy(Uri address, ICredentials credentials = null)
        {
            ConnectionOptions.HttpsProxy = address;
            ConnectionOptions.ProxyCredentials = credentials;
            return this;
        }

        /// <summary>
        /// See <see cref="Connector"/>.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetConnector(Func<string, ConnectionOptions, CancellationToken, Task<IConnection>> connector)
        {
            Connector = connector;
            return this;
        }

        /// <summary>
        /// See <see cref="Authenticator"/>.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetAuthenticator(Func<IConnection, Task<JToken>> authenticator)
        {
            Authenticator = authenticator;
            return this;
        }

        /// <summary>
        /// See <see cref="Dispatcher"/>.
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetDispatcher(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            return this;
        }

        /// <summary>
        /// See <see cref="Authenticator"/>. 
        /// </summary>
        /// <returns>The instance of the builder.</returns>
        public RtmClientBuilder SetRoleSecretAuthenticator(string role, string secret)
        {
            Authenticator = con =>
            {
                return AuthRoleSecret.Authenticate(con, role, secret);
            };
            return this;
        }

        /// <summary>
        ///  Builds an instance of the <see cref="IRtmClient"/> interface. Use this method after you
        /// set the client properties. 
        /// </summary>
        /// <returns>The instance of the client.</returns>
        public IRtmClient Build()
        {
            return new RtmClient(
                Connector, 
                ConnectionOptions,
                Authenticator, 
                Dispatcher, 
                Url,
                MinReconnectInterval, 
                MaxReconnectInterval, 
                PendingActionQueueLength);
        }
    }
}