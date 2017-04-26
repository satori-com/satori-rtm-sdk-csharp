#pragma warning disable 1591

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Satori.Rtm
{
    public static partial class AuthRoleSecret
    {
        public static string ComputeHash(string secret, string nonce)
        {
            var secretBin = Encoding.UTF8.GetBytes(secret);
            var nonceBin = Encoding.UTF8.GetBytes(nonce);
            using (var hmac = new HMACMD5(secretBin))
            {
                var hashBin = hmac.ComputeHash(nonceBin);
                var hash = Convert.ToBase64String(hashBin);
                return hash;
            }
        }

        public static async Task<JToken> Authenticate(IConnection con, string role, string secret)
        {
            var handshake = await con.RoleSecretHandshake(role).ConfigureAwait(false);
            var nonce = handshake.Data.Nonce;
            return await con.RoleSecretAuthenticate(secret, nonce).ConfigureAwait(false);
        }
    }

    public class AuthRoleSecretHandshakeData
    {
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }
    }

    public class AuthRoleSecretHandshakeRequest : AuthHandshakeRequest<AuthRoleSecretHandshakeData>
    {
    }

    public class AuthRoleSecretHandshakeNonce
    {
        [JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
        public string Nonce { get; set; }
    }

    public class AuthRoleSecretHandshakeReply : AuthHandshakeReply<AuthRoleSecretHandshakeNonce>
    {
        public AuthRoleSecretHandshakeReply(AuthRoleSecretHandshakeNonce data)
        {
            Data = data;
        }

        public AuthRoleSecretHandshakeReply(AuthHandshakeReply<AuthRoleSecretHandshakeNonce> v) 
            : this(v.Data)
        {
        }

        public AuthRoleSecretHandshakeReply(AuthHandshakeReply<JToken> v) 
            : this(v.Data.ToObject<AuthRoleSecretHandshakeNonce>())
        {
        }
    }

    public class AuthRoleSecretCredentials
    {
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }
    }

    public class AuthRoleSecretAthenticateRequest : AuthenticateRequest<AuthRoleSecretCredentials>
    {
    }
}