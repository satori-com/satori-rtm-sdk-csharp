#pragma warning disable 1591

using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Satori.Rtm.Client;

namespace Satori.Rtm
{
    public static partial class ConnectionExtensions
    {
        public static async Task<TReply> DoOperation<TRequest, TReply>(
            this IConnection conn, 
            string action, 
            TRequest reqBody, 
            Ack ack)
            where TRequest : class
            where TReply : class
        {
            if (ack == Ack.Yes)
            {
                var res = await conn.DoOperationAsync(action, reqBody)
                                    .ConfigureAwait(false);

                return res.Match(
                    disconnected: _ =>
                    {
                        throw new DisconnectedException();
                    },
                    failure: f =>
                    {
                        throw f.error;
                    },
                    reply: r => r.Match(
                        positive: p =>
                        {
                            return p.pdu.Body?.ToObject<TReply>();
                        },
                        negative: p =>
                        {
                            throw new PduException(p.pdu);
                        },
                        unknown: p =>
                        {
                            throw new PduException(p.pdu);
                        }));
            }
            else
            {
                await conn.DoOperationNoAckAsync(action, reqBody);
                return null;
            }
        }

        public static async Task<RtmSubscribeReply> RtmSubscribe(this IConnection conn, RtmSubscribeRequest reqBody)
        {
            var res = await conn.DoOperationAsync(
                RtmActions.Subscribe, 
                reqBody).ConfigureAwait(false);
            return res.Match(
                disconnected: _ =>
                {
                    throw new DisconnectedException();
                },
                failure: f =>
                {
                    throw f.error;
                },
                reply: r => r.Match(
                    positive: p =>
                    {
                        return p.pdu.Body?.ToObject<RtmSubscribeReply>();
                    },
                    negative: p =>
                    {
                        throw new SubscribeException(p.pdu);
                    },
                    unknown: p =>
                    {
                        throw new SubscribeException(p.pdu);
                    }));
        }

        public static async Task<RtmUnsubscribeReply> RtmUnsubscribe(this IConnection conn, string subscriptionId)
        {
            var reqBody = new RtmUnsubscribeRequest
            {
                SubscriptionId = subscriptionId
            };
            var res = await conn.DoOperationAsync(
                RtmActions.Unsubscribe, reqBody).ConfigureAwait(false);
            return res.Match(
                disconnected: _ =>
                {
                    throw new DisconnectedException();
                },
                failure: f =>
                {
                    throw f.error;
                },
                reply: r => r.Match(
                    positive: p =>
                    {
                        return p.pdu.Body?.ToObject<RtmUnsubscribeReply>();
                    },
                    negative: p =>
                    {
                        throw new UnsubscribeException(p.pdu);
                    },
                    unknown: p =>
                    {
                        throw new UnsubscribeException(p.pdu);
                    }));
        }

        public static Task<RtmPublishReply> RtmPublish<T>(this IConnection conn, string channel, T message, Ack ack)
        {
            var reqBody = new RtmPublishRequest<T>
            {
                Channel = channel,
                Message = message
            };
            return conn.DoOperation<RtmPublishRequest<T>, RtmPublishReply>(RtmActions.Publish, reqBody, ack);
        }

        public static Task<RtmReadReply<T>> RtmRead<T>(this IConnection conn, RtmReadRequest reqBody)
        {
            return conn.DoOperation<RtmReadRequest, RtmReadReply<T>>(RtmActions.Read, reqBody, Ack.Yes);
        }

        public static Task<RtmWriteReply> RtmWrite<T>(this IConnection conn, RtmWriteRequest<T> reqBody, Ack ack)
        {
            return conn.DoOperation<RtmWriteRequest<T>, RtmWriteReply>(RtmActions.Write, reqBody, ack);
        }

        public static Task<RtmDeleteReply> RtmDelete(this IConnection conn, RtmDeleteRequest reqBody, Ack ack)
        {
            return conn.DoOperation<RtmDeleteRequest, RtmDeleteReply>(RtmActions.Delete, reqBody, ack);
        }

        #region Authentication

        public static async Task<JToken> Authenticate(this IConnection conn, string method, JToken credentials)
        {
            var reqBody = new AuthenticateRequest
            {
                Method = method,
                Credentials = credentials
            };
            var res = await conn.DoOperationAsync(
                AuthActions.Authenticate, reqBody).ConfigureAwait(false);
            return res.Match(
                disconnected: _ =>
                {
                    throw new DisconnectedException();
                },
                failure: f =>
                {
                    throw f.error;
                },
                reply: r => r.Match(
                    positive: p =>
                    {
                        return p.pdu.Body;
                    },
                    negative: p =>
                    {
                        throw new AuthException(p.pdu);
                    },
                    unknown: p =>
                    {
                        throw new AuthException(p.pdu);
                    }));
        }

        public static Task<JToken> Authenticate<T>(this IConnection conn, string method, T credentials)
        {
            return Authenticate(conn, method, JToken.FromObject(credentials));
        }

        public static async Task<AuthHandshakeReply> AuthHandshake(this IConnection conn, string method, JToken data)
        {
            var reqBody = new AuthHandshakeRequest
            {
                Method = method,
                Data = data
            };
            var res = await conn.DoOperationAsync(
                AuthActions.Handshake, reqBody).ConfigureAwait(false);
            return res.Match(
                disconnected: _ =>
                {
                    throw new DisconnectedException();
                },
                failure: f =>
                {
                    throw f.error;
                },
                reply: r => r.Match(
                    positive: p =>
                    {
                        return p.pdu.Body?.ToObject<AuthHandshakeReply>();
                    },
                    negative: p =>
                    {
                        throw new AuthException(p.pdu);
                    },
                    unknown: p =>
                    {
                        throw new AuthException(p.pdu);
                    }));
        }

        public static Task<AuthHandshakeReply> AuthHandshake<T>(this IConnection conn, string method, T data)
        {
            return AuthHandshake(conn, method, JToken.FromObject(data));
        }

        public static Task<JToken> RoleSecretAuthenticate(this IConnection conn, string secret, string nonce)
        {
            return Authenticate(
                conn, 
                AuthenticateMethods.RoleSecret,
                new AuthRoleSecretCredentials
                {
                    Hash = AuthRoleSecret.ComputeHash(secret, nonce),
                });
        }

        public static async Task<AuthRoleSecretHandshakeReply> RoleSecretHandshake(this IConnection conn, string role)
        {
            var res = await AuthHandshake(
                conn, 
                AuthenticateMethods.RoleSecret,
                new AuthRoleSecretHandshakeData
                {
                    Role = role,
                }).ConfigureAwait(false);
            return new AuthRoleSecretHandshakeReply(res);
        }

        #endregion
    }
}