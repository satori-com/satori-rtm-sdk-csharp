#pragma warning disable 1591

namespace System.Net
{
    /// <summary>
    /// Simple implementation of IWebProxy. 
    /// It is a substitute for the standard implementation which is not available 
    /// in .NETStandard 1.3
    /// </summary>
    public class WebProxy : IWebProxy
    {
        public Uri Address { get; set; }

        public WebProxy(Uri address)
        {
            Address = address;
        }
        public Uri GetProxy(Uri destination)
        {
            return Address;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}
