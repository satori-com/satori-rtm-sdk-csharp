#pragma warning disable 1591

using System.Threading.Tasks;

namespace System.Net
{
    public static class DnsEx 
    {        
        public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress)
        {
            #if NET_4_5
            return Task<IPAddress[]>.Factory.FromAsync (Dns.BeginGetHostAddresses, Dns.EndGetHostAddresses, hostNameOrAddress, null);
            #else
            return Dns.GetHostAddressesAsync(hostNameOrAddress);
            #endif
        }

        public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress)
        {
            #if NET_4_5
            return Task<IPHostEntry>.Factory.FromAsync (Dns.BeginGetHostEntry, Dns.EndGetHostEntry, hostNameOrAddress, null);
            #else
            return Dns.GetHostEntryAsync(hostNameOrAddress);
            #endif
        }
    }
}