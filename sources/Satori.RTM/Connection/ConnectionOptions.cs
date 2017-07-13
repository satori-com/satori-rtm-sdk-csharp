using System;
using System.Net;

namespace Satori.Rtm
{
    /// <summary>
    /// Options to use when creating an instance of the <see cref="IConnection"/> interface. 
    /// </summary>
    public class ConnectionOptions
    {
        /// <summary>
        /// Gets or sets the address of the HTTPS proxy server. 
        /// Default value is null (no proxy). 
        /// </summary>
        public Uri HttpsProxy { get; set; }

        /// <summary>
        /// Sets or gets the credentials to submit to the proxy server for authentication.
        /// </summary>
        public ICredentials ProxyCredentials { get; set; }
    }
}
