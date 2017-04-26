#pragma warning disable 1591

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Satori.Rtm
{
    public static partial class Auth
    {
        public const string Service = "auth";
    }

    public static class AuthOperations
    {
        public const string Handshake = "handshake";
        public const string Authenticate = "authenticate";
    }

    public static class AuthActions
    {
        public static readonly string Handshake = $"{Auth.Service}/{AuthOperations.Handshake}";
        public static readonly string Authenticate = $"{Auth.Service}/{AuthOperations.Authenticate}";
    }

    public static class AuthenticateMethods
    {
        public const string RoleSecret = "role_secret";
    }

    public static class AuthErrorCodes
    {
        public const string AuthorizationDenied = "authorization_denied";
    }

    public class AuthHandshakeRequest<TPayload>
    {
        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public TPayload Data { get; set; }
    }

    public class AuthHandshakeRequest : AuthHandshakeRequest<JToken>
    {
    }

    public class AuthenticateRequest<TPayload>
    {
        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty("credentials", NullValueHandling = NullValueHandling.Ignore)]
        public TPayload Credentials { get; set; }
    }

    public class AuthenticateRequest : AuthenticateRequest<JToken>
    {
    }

    public class AuthHandshakeReply<TPayload>
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public TPayload Data { get; set; }
    }

    public class AuthHandshakeReply : AuthHandshakeReply<JToken>
    {
        public AuthHandshakeReply<T> As<T>()
        {
            return new AuthHandshakeReply<T>
            {
                Data = Data.ToObject<T>()
            };
        }
    }
}