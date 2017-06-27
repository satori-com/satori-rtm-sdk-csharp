#pragma warning disable 1591

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Satori.Rtm
{
    public static partial class Rtm
    {
        public const string Service = "rtm";
    }

    public static class RtmOutcomes
    {
        public const string Ok = "ok";
        public const string Error = "error";
        public const string Unknown = "unknown";

        public const string Data = "data";
        public const string Info = "info";
    }

    public static class RtmOperations
    {
        public const string Publish = "publish";
        public const string Subscribe = "subscribe";
        public const string Unsubscribe = "unsubscribe";
        public const string Subscription = "subscription";
        public const string Read = "read";
        public const string Write = "write";
        public const string Delete = "delete";
    }

    public static class RtmActions
    {
        public static readonly string CoreError = $"/{RtmOutcomes.Error}";

        public static readonly string Publish = $"{Rtm.Service}/{RtmOperations.Publish}";
        public static readonly string Subscribe = $"{Rtm.Service}/{RtmOperations.Subscribe}";
        public static readonly string Unsubscribe = $"{Rtm.Service}/{RtmOperations.Unsubscribe}";

        public static readonly string Subscription = $"{Rtm.Service}/{RtmOperations.Subscription}";
        public static readonly string SubscriptionData = $"{Subscription}/{RtmOutcomes.Data}";
        public static readonly string SubscriptionError = $"{Subscription}/{RtmOutcomes.Error}";
        public static readonly string SubscriptionInfo = $"{Subscription}/{RtmOutcomes.Info}";

        public static readonly string Read = $"{Rtm.Service}/{RtmOperations.Read}";
        public static readonly string Write = $"{Rtm.Service}/{RtmOperations.Write}";
        public static readonly string Delete = $"{Rtm.Service}/{RtmOperations.Delete}";
    }

    /// <summary>
    /// Communication between client and servers involve exchanging units of structured data. 
    /// A single of such unit, a Protocol Data Unit (PDU) is a JSON-encoded message 
    /// sent in a separate WebSocket Frame, containing all of the system specific information 
    /// as well as the user specified payload.
    /// </summary>
    public class Pdu<TPayload>
    {
        /// <summary>
        /// Gets or sets the action field of PDU. Action can be executing a command, retrieving 
        /// a piece of information, etc. 
        /// Analogous to a "call" in RPC (remote procedure call) lingo.
        /// </summary>
        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the id field of PDU. The optional "id" field is used to match the server side response with 
        /// the client request. 
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the body field of PDU. The mandatory "body" field is used to convey "action"-specific contents, 
        /// such as a name of the channel in case of "rtm/publish" action. 
        /// </summary>
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public TPayload Body { get; set; }

        public override string ToString()
        {
            return $"[Pdu: Action={Action}, Id={Id}, Body={Body}]";
        }
    }

    /// <summary>
    /// See the <see cref="Pdu{TPayload}"/> class. 
    /// </summary>
    public class Pdu : Pdu<JToken>
    {
        public static Pdu Create<TPayload>(string action, string id, TPayload body)
        {
            return Create(action, id, body != null ? JToken.FromObject(body) : null);
        }

        public static Pdu Create(string action, string id, JToken body)
        {
            return new Pdu()
            {
                Action = action,
                Id = id,
                Body = body,
            };
        }

        public Pdu Reply<TPayload>(string code, TPayload body)
        {
            return Reply(code, body != null ? JToken.FromObject(body) : null);
        }

        public Pdu<T> As<T>()
        {
            return new Pdu<T>
            {
                Action = Action,
                Id = Id,
                Body = Body.ToObject<T>()
            };
        }

        public Pdu Reply(string code, JToken body)
        {
            return new Pdu()
            {
                Action = $"{Action}/{code}",
                Id = Id,
                Body = body,
            };
        }
    }

    /// <summary>
    /// Common base class for error outcomes. 
    /// </summary>
    public class RtmError
    {
        [JsonProperty("error")]
        public string Code { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }

    public class CommonError : RtmError
    {
        /// <summary>
        /// Invalid JSON
        /// </summary>
        public const string JsonParseError = "json_parse_error";

        /// <summary>
        /// Invalid action (the port up to the initial "/")
        /// </summary>
        public const string InvalidService = "invalid_service"; 

        /// <summary>
        /// Invalid action (the part after the initial "/")
        /// </summary>
        public const string InvalidOperation = "invalid_operation";

        /// <summary>
        /// an invalid field was provided, is missing or has invalid value
        /// and also "generic error": any unrecognized error code
        /// </summary>
        public const string InvalidFormat = "invalid_format"; 
    }

    /// <summary>
    /// Describes a subscribe request. 
    /// A subscribe request creates a long term relationship between the endpoint 
    /// and a specified channel. All messages which other endpoints send into 
    /// the channel will be received by all endpoints which requested a subscription. 
    /// The subscription may fail for many reasons, including authentication. 
    /// A positive subscription confirmation will appear prior to any messages from 
    /// the channel sent to the endpoint. At any point of time the channel may get closed. 
    /// No channel specific data will be sent to the endpoint after the channel is 
    /// closed either during subscription action or after sporadic closure.
    /// </summary>
    public class RtmSubscribeRequest
    {
        #region Subscribe to channel

        /// <summary>
        /// Gets or sets the channel. This property is mutually exclusive with 
        /// the <see cref="SubscriptionId"/>, <see cref="Filter"/>,  and
        /// <see cref="Period"/>  properties.  
        /// </summary>
        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public string Channel { get; set; }

        #endregion

        #region Subscirbe to filter

        /// <summary>
        /// Gets or sets the subscription identifier. This property is mutually 
        /// exclusive with the <see cref="Channel"/> property. 
        /// </summary>
        [JsonProperty("subscription_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the filter. This property is mutually exclusive with 
        /// the <see cref="Channel"/> property. 
        /// </summary>
        /// <remarks>
        /// The filter service supports a subset of SQL2003 syntax that makes sense in the 
        /// context of filtering streaming data. It does not support operations that create 
        /// or modify data. It does not support JOIN operations as only one row of data is 
        /// from a single channel is considered at any given moment. 
        /// Read about filters on <a href="https://www.satori.com/docs/using-satori/filters">satori.com</a>. 
        /// </remarks>
        [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the period. This property is mutually exclusive with 
        /// the <see cref="Channel"/> property. The RTM service sends the accumulated 
        /// messages once every "period" seconds. It can be a number 1-60 in seconds.
        /// </summary>
        [JsonProperty("period", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? Period { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the force flag. The subscribe request will be rejected 
        /// if there already is a subscription with the provided <see cref="Channel"/> name 
        /// or <see cref="SubscriptionId"/> unless "force" is set to true in which case the 
        /// subscription is left as is and the request will have a positive reply.
        /// </summary>
        [JsonProperty("force", NullValueHandling = NullValueHandling.Ignore)]
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets the fast_forward flag. 
        /// The RTM service does not maintain messages forever, in case the subscriber is 
        /// reading the messages from the connection too slow it may start falling behind. 
        /// If the subscriber falls behind far enough so that it will miss some messages, 
        /// then RTM will unsubscribe the client (default) unless the "fast_forward" flag 
        /// was set to true in which case the RTM service will fast-forward the client.
        /// </summary>
        [JsonProperty("fast_forward", NullValueHandling = NullValueHandling.Ignore)]
        public bool FastForward { get; set; }

        /// <summary>
        /// Gets or sets the position. Position sets the first message to receive, default 
        /// is current position. See the <see cref="History"/> property.  
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the history. History" means we want messages older than the one 
        /// specified by the <see cref="Position"/> property, default is no history.
        /// </summary>
        [JsonProperty("history", NullValueHandling = NullValueHandling.Ignore)]
        public RtmSubscribeHistory History { get; set; }
    }

    /// <summary>
    /// History" means we want messages older than the one specified by "position", 
    /// default is no history. 
    /// See the <see cref="RtmSubscribeRequest"/>.<see cref="RtmSubscribeRequest.History"/> property. 
    /// </summary>
    public class RtmSubscribeHistory
    {
        /// <summary>
        /// Gets or sets the count. Count means start the subscription this number of 
        /// messages older than what the 
        /// <see cref="RtmSubscribeRequest"/>.<see cref="RtmSubscribeRequest.Position"/>  property says.
        /// <see cref="Count"/> and <see cref="Age"/> are mutually exclusive.   
        /// </summary>
        [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? Count { get; set; }

        /// <summary>
        /// Gets or sets the age. Age means start the subscription with messages these many 
        /// seconds older than what the 
        /// <see cref="RtmSubscribeRequest"/>.<see cref="RtmSubscribeRequest.Position"/>  property says.
        /// <see cref="Count"/> and <see cref="Age"/> are mutually exclusive.   
        /// </summary>
        [JsonProperty("age", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? Age { get; set; }
    }

    /// <summary>
    /// Describes a positive reply on a <see cref="RtmSubscribeRequest"/> request.
    /// </summary>
    public class RtmSubscribeReply
    {
        /// <summary>
        /// Gets or sets the position. 
        /// Position is the position where this subscription is starting (or the 
        /// subscriptions' current position if the <see cref="RtmSubscribeRequest.Force"/> flag  was used).
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier. 
        /// All future references to this subscription should use the SubscriptionId 
        /// provided in the response (it'll be the same as the one specified in the request 
        /// provided <see cref="RtmSubscribeRequest.Filter"/>  was provided).
        /// </summary>
        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }
    }

    /// <summary>
    /// Describes an error on a <see cref="RtmSubscribeRequest"/> request. 
    /// </summary>
    public class RtmSubscribeError : RtmError
    {
        public const string InvalidFormat = "invalid_format";

        /// <summary>
        /// The specified subscription is already established.
        /// </summary>
        public const string AlreadySubscribed = "already_subscribed"; 

        /// <summary>
        /// The start position requested in "rtm/subscribe" no longer exists.
        /// </summary>
        public const string ExpiredPosition = "expired_position";

        [JsonProperty("subscription_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionId { get; set; }
    }

    /// <summary>
    /// Describes a subscription data. 
    /// Subscription data are sent as a result of an active subscription to send 
    /// published messages to the subscriber.
    /// </summary>
    public class RtmSubscriptionData<TPayload>
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// Position describes the position of the next message just after the 
        /// provided "messages", it is explicitly intended for re-subscription 
        /// in case of connection drop.
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the messages. 
        /// Messages is an array of messages that someone has published to this channel. 
        /// Each subscriber will receive them in the same order as each publisher sent them. 
        /// All subscribers will see the same order of messages. 
        /// </summary>
        [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
        public TPayload[] Messages { get; set; }
    }

    /// <summary>
    /// See the <see cref="RtmSubscriptionData"/> class.  
    /// </summary>
    public class RtmSubscriptionData : RtmSubscriptionData<JToken>
    {
    }

    /// <summary>
    /// Describes a subscription info. 
    /// </summary>
    /// <remarks>
    /// The RTM service does not maintain messages forever, in case the subscriber 
    /// is reading the messages from the connection too slow it may start falling 
    /// behind. If the subscriber falls behind far enough so that it will miss 
    /// some messages, then the RTM service will unsubscribe the client (default) 
    /// unless the <see cref="RtmSubscribeRequest.FastForward"/> flag was set to true 
    /// in which case the RTM service will fast-forward the client.
    /// <para/>
    /// If a new subscription specifies a position + history for a message 
    /// the RTM service no longer has and the "fast_forward" flag was set to true, 
    /// in which case the subscription will be fast-forwarded immediately
    /// </remarks>
    public class RtmSubscriptionInfo
    {
        public const string FastForward = "fast_forward";

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// Position describes the position of the next message just after 
        /// fast-forwarding, it is explicitly intended for re-subscription 
        /// in case of connection drop.
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }

        [JsonProperty("missed_message_count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? MissedMessageCount { get; set; }
    }

    public class RtmSubscriptionError : RtmError
    {
        /// <summary>
        /// Client's endpoint is too slow to process incoming channel messages. 
        /// The client is lagging behind the oldest available position: data loss 
        /// is imminent so the server pushes this error and forces unsubscription.
        /// <para/>
        /// Note that the client should not re-subscribe with its last recorded position 
        /// as it would be expired. Client can recover by re-subscribing without 
        /// position or, if available from external sources, with a more current position 
        /// (client would be losing channel data in between). 
        /// <para/>
        /// Note that a client experiencing this behavior consistently is also 
        /// a subject to uneven increasing latencies.
        /// </summary>
        public const string OutOfSync = "out_of_sync";

        public const string InternalError = "internal_error";

        public const string ChannelDeleted = "channel_deleted";

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the missed message count.
        /// missed_message_count" may be present in case "error":"out_of_sync".
        /// </summary>
        [JsonProperty("missed_message_count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? MissedMessageCount { get; set; } 
    }

    /// <summary>
    /// Describes an unsubscribe request. 
    /// An unsubscribe request will unsubscribe the client from the specified subscription.
    /// </summary>
    public class RtmUnsubscribeRequest
    {
        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }
    }

    /// <summary>
    /// Describes a reply on a <see cref="RtmUnsubscribeRequest"/> request. 
    /// </summary>
    public class RtmUnsubscribeReply
    {
        /// <summary>
        /// Gets or sets the position.
        /// Position describes the position of the next message just after the provided 
        /// "messages", it is explicitly intended for re-subscription in case of connection drop.
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("subscription_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionId { get; set; }
    }

    /// <summary>
    /// Describes an error on a <see cref="RtmUnsubscribeRequest"/> request.  
    /// </summary>
    public class RtmUnsubscribeError : RtmError
    {
        /// <summary>
        /// The specified subscription was not established
        /// </summary>
        public const string NotSubscribed = "not_subscribed"; 

        /// <summary>
        /// Value for field is invalid
        /// </summary>
        public const string InvalidFormat = "invalid_format"; 

        [JsonProperty("subscription_id", NullValueHandling = NullValueHandling.Ignore)]
        public string SubscriptionId { get; set; }
    }

    /// <summary>
    /// Describes a publish request.
    /// A publish request will publish the given <see cref="Message"/>  
    /// to the specified <see cref="Channel"/>.
    /// </summary>
    public class RtmPublishRequest<TPayload>
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the message. Message could be null.
        /// </summary>
        [JsonProperty("message")]
        public TPayload Message { get; set; }
    }

    /// <summary>
    /// See the <see cref="RtmPublishRequest"/> class.  
    /// </summary>
    public class RtmPublishRequest : RtmPublishRequest<JToken>
    {
    }

    /// <summary>
    /// Describes a reply on a <see cref="RtmPublishRequest"/> request. 
    /// </summary>
    /// <remarks>
    /// Note: both publish reply and subscription data PDUs contain "position" field 
    /// but semantics are slightly different. Publisher receives a position 
    /// corresponding to the message itself, while subscribers get the position 
    /// next after the message.
    /// </remarks>
    public class RtmPublishReply
    {
        /// <summary>
        /// Gets or sets the position.
        /// Position uniquely identifies placement of the published message 
        /// within a channel.
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }
    }

    /// <summary>
    /// Describes a read request. 
    /// A read request will read the latest publish message by default. 
    /// </summary>
    public class RtmReadRequest
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the position. 
        /// Position can be used to specify an older message to read.
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public string Position { get; set; }
    }

    /// <summary>
    /// Describes a reply on a <see cref="RtmReadRequest"/> request.  
    /// </summary>
    public class RtmReadReply<TPayload>
    {
        /// <summary>
        /// Gets or sets the position.
        /// Position is the position of the returned message. 
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the message. 
        /// Message will be null if the specified <see cref="RtmReadRequest.Channel"/>
        /// does not have any published messages.
        /// </summary>
        [JsonProperty("message")]
        public TPayload Message { get; set; }
    }

    /// <summary>
    /// See the <see cref="RtmReadReply"/> class.  
    /// </summary>
    public class RtmReadReply : RtmReadReply<JToken>
    {
    }

    /// <summary>
    /// Describes a write request. Same as <see cref="RtmPublishRequest"/>.
    /// </summary>
    public class RtmWriteRequest<TPayload>
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("message")]
        public TPayload Message { get; set; }
    }

    /// <summary>
    /// See the <see cref="RtmWriteRequest"/> class.  
    /// </summary>
    public class RtmWriteRequest : RtmWriteRequest<JToken>
    {
    }

    /// <summary>
    /// Describes a reply on a <see cref="RtmWriteRequest"/> request.  
    /// </summary>
    public class RtmWriteReply
    {
        /// <summary>
        /// Gets or sets the position.
        /// Position means position of the written message within a channel.
        /// </summary>
        [JsonProperty("position")]
        public string Position { get; set; }
    }

    /// <summary>
    /// Describes a delete request. 
    /// Same as <see cref="RtmPublishRequest"/> with 
    /// <see cref="RtmPublishRequest{TPayload}.Message"/> set to null, unless 
    /// <see cref="RtmDeleteRequest.Purge"/> is set to true in which case all 
    /// already published messages are also removed. 
    /// </summary>
    public class RtmDeleteRequest
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("purge", NullValueHandling = NullValueHandling.Ignore)]
        public bool Purge { get; set; }
    }

    /// <summary>
    /// Describes a reply on a <see cref="RtmDeleteRequest"/> request.  
    /// </summary>
    public class RtmDeleteReply
    {
        /// <summary>
        /// Gets or sets the position.
        /// Position means position of the deleted message and is returned 
        /// unless <see cref="RtmDeleteRequest"/>.<see cref="RtmDeleteRequest.Purge"/>
        ///  is set to true.
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public string Position { get; set; }
    }
}