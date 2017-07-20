#pragma warning disable 1591

using System;
using System.Threading.Tasks;

namespace Satori.Rtm.Client
{
    /// <summary>
    /// Defines how a particular subscription handles reconnects and slow connection. 
    /// </summary>
    [Flags]
    public enum SubscriptionModes
    {
        /// <summary>
        /// SDK tracks the stream position from responses and tries to restore
        /// subscription from the latest known position on reconnects. 
        /// Consider using one of the following modes instead: 
        /// <see cref="Simple"/>, <see cref="Reliable"/>, <see cref="Advanced"/>
        /// </summary>
        TrackPosition = 1,

        /// <summary>
        /// RTM forwards the subscription to the earliest possible
        /// position if a client has slow connection. 
        /// Consider using one of the following modes instead: 
        /// <see cref="Simple"/>, <see cref="Reliable"/>, <see cref="Advanced"/>
        /// </summary>
        FastForward = 2,

        /// <summary>
        /// May lose data during reconnect and on slow connections.
        /// <para>
        /// SDK doesn't track the stream position and restores subscription from it's
        /// actual position. RTM forwards the subscription to the earliest possible
        /// position if a client has slow connection. </para>
        /// </summary>
        Simple = FastForward,

        /// <summary>
        /// Tries to avoid data loss during reconnect but may lose data on slow connections.
        /// <para>
        /// SDK tracks the stream position from responses and tries to restore 
        /// subscription from the latest known position on reconnects. 
        /// RTM forwards the subscription to the earliest possible position 
        /// if the stream position is expired on reconnect or a client has slow connection. </para>
        /// </summary>
        Reliable = TrackPosition | FastForward,

        /// <summary>
        /// Tries to avoid any data loss, could get out_of_sync and expired_position errors
        /// on reconnect and slow connections.
        /// <para>
        /// SDK tracks the stream position from responses and tries to restore
        /// subscription from the latest known position on reconnects. If the stream
        /// position is expired then an expired_position error is thrown. If connection is
        /// slow then out_of_sync error is thrown. </para>
        /// </summary>
        Advanced = TrackPosition
    }

    /// <summary>
    /// The <see cref="IRtmClient"/>  interface is the main entry point for accessing 
    /// the RTM service, including publish and subscribe operations.
    /// </summary>
    /// <remarks>
    /// Create an instance of the client with <see cref="RtmClientBuilder"/> 
    /// and use the <see cref="IRtmClient"/>  methods to start, stop, and restart 
    /// the client WebSocket connection, and publish and subscribe to channels.
    /// </remarks>
    public interface IRtmClient : IDispatchObject
    {
        /// <summary>
        /// Occurs when a client enters a stopped state. 
        /// Client enters a stopped state after <see cref="Stop"/> method is called. 
        /// </summary>
        event Action OnEnterStopped;

        /// <summary>
        /// Occurs when a client leaves a stopped state. Client leaves 
        /// a stopped state after <see cref="Start"/> method is called 
        /// </summary>
        event Action OnLeaveStopped;

        /// <summary>
        /// Occurs when a client enters a connecting state. 
        /// Client enters a connecting state after it leaves a stopped state
        /// or awaiting state. 
        /// </summary>
        event Action OnEnterConnecting;

        /// <summary>
        /// Occurs when a client leaves a connecting state. 
        /// Client leaves a connecting state after successful or failed
        /// attempt to connect to RTM service. 
        /// <para/>
        /// Note, that authentication happens in a connecting state, just 
        /// after a connection to RTM service is established.
        /// </summary>
        event Action OnLeaveConnecting;

        /// <summary>
        /// Occurs when a client enters a connected state.
        /// Client enters a connected state after a connection 
        /// to RTM service is established and authentication succeed. 
        /// </summary>
        event Action<IConnection> OnEnterConnected;

        /// <summary>
        /// Occurs when a client leaves a connected state. 
        /// Client leaves a connected state after <see cref="Stop"/> method
        /// is called or a connection is dropped. 
        /// </summary>
        event Action<IConnection> OnLeaveConnected;

        /// <summary>
        /// Occurs when a client enters an awaiting state. 
        /// Client enters an awaiting state after it leaves
        /// a connecting or connected state because connection has been dropped.  
        /// </summary>
        event Action OnEnterAwaiting;

        /// <summary>
        /// Occurs when a client leaves an awaiting state.
        /// Client leaves an awaiting state after a reconnect interval has elapsed
        /// or <see cref="Stop"/> method has been called. In former case,
        /// a client will enter a connecting state. 
        /// </summary>
        event Action OnLeaveAwaiting;

        /// <summary>
        /// Occurs when a client is disposed.
        /// </summary>
        event Action OnEnterDisposed;

        /// <summary>
        /// This event is fired when a client error occurs.
        /// Client error is contrasted to operation and subscription errors. 
        /// Client error occurs, for example, when a connection attempt fails or 
        /// RTM reports about a connection-wide error like a malformed PDU. 
        /// </summary>
        event Action<Exception> OnError;

        /// <summary>
        /// Starts the client. Client tries to establish the WebSocket connection 
        /// to the RTM service asynchronously.
        /// </summary>
        /// <remarks>
        /// The SDK attempts to reconnect to the RTM service if the WebSocket
        ///  connection fails for any reason.
        /// If you make any publish or subscribe requests while the WebSocket 
        /// connection is not active, the SDK queues the requests and completes 
        /// them when the connection is established or re-established. 
        /// You can use the client events to define application functionality for when the application
        /// enters or leaves the connected state.
        /// </remarks>
        void Start();

        /// <summary>
        /// Stops the client. The SDK tries to close the WebSocket connection 
        /// asynchronously and does not start it again unless you call <see cref="Start"/> .
        /// </summary>
        /// <remarks>
        /// Use this method to explicitly stop all interaction with the RTM service.
        /// You can use the client events to define application functionality for when the application
        /// enters or leaves the connected state.
        /// </remarks>
        void Stop();

        /// <summary>
        /// Restarts the client.
        /// </summary>
        void Restart();

        /// <summary>
        /// Returns the current connection or <c>null</c> if the client is not connected.
        /// </summary>
        /// <returns>The task completes when this call is executed on the <see cref="RtmClientBuilder.Dispatcher"/>. </returns>
        Task<IConnection> GetConnection();

        /// <summary>
        /// See <see cref="GetConnection()"/>. 
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>. 
        /// </summary>
        void GetConnection(Action<IConnection> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Disposes the client.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="T:Satori.Rtm.Client.IRtmClient"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="T:Satori.Rtm.Client.IRtmClient"/> in an unusable state.
        /// After calling <see cref="Dispose()"/>, you must release all references to the
        /// <see cref="T:Satori.Rtm.Client.IRtmClient"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:Satori.Rtm.Client.IRtmClient"/> was occupying.</remarks>
        /// <returns>The task completes when this call is executed on the <see cref="RtmClientBuilder.Dispatcher"/>. </returns>
        Task Dispose();

        /// <summary>
        /// See <see cref="Dispose()"/>.
        /// The callback is invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Dispose(Action onCompleted);

        #region RTM 

        /// <summary>
        /// Creates subscription with the specific <paramref name="channel"/> name.
        /// </summary>
        /// <remarks>
        /// You can create subscription at any time. The SDK manages 
        /// the subscription and sends a subscribe request when the connection 
        /// to RTM service is established. 
        /// Subscription mode <see cref="SubscriptionModes.Simple"/> is used.
        /// </remarks>
        void CreateSubscription(
            string channel,
            ISubscriptionObserver observer);

        /// <summary>
        /// Creates subscription with the specific <paramref name="channel"/> name.
        /// </summary>
        /// <remarks>
        /// You can create subscription at any time. The SDK manages 
        /// the subscription and sends a subscribe request when the connection 
        /// to RTM service is established. 
        /// Use the <paramref name="mode"/>  parameter to define the behavior that 
        /// the SDK uses to handle dropped connections.
        /// </remarks>
        void CreateSubscription(
            string channel,
            SubscriptionModes mode,
            ISubscriptionObserver observer);

        /// <summary>
        /// Creates subscription with the specific channel name or subscription id.
        /// </summary>
        /// <remarks>
        /// You can create subscription at any time. The SDK manages 
        /// the subscription and sends a subscribe request when the WebSocket 
        /// connection is established. 
        /// Use the <paramref name="config"/>  parameter to define the behavior that 
        /// the SDK uses to handle dropped connections.
        /// </remarks>
        void CreateSubscription(
            string channelOrSubId,
            SubscriptionConfig config);

        /// <summary>
        /// Removes the subscription with the specific subscription id.
        /// </summary>
        void RemoveSubscription(string channelOrSubId);

        /// <summary>
        /// Gets the subscription with the specified channel name or subscription id.
        /// </summary>
        /// <returns>The task completes when this call is executed on the <see cref="RtmClientBuilder.Dispatcher"/>. 
        /// Task fails if a subscription with the specified <paramref name="channelOrSubId"/> doesn't exist. </returns>
        Task<ISubscription> GetSubscription(string channelOrSubId);

        /// <summary>
        /// See <see cref="GetSubscription(string)"/>. 
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void GetSubscription(string channelOrSubId, Action<ISubscription> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Publishes the <paramref name="message"/> to the <paramref name="channel"/>.
        /// </summary>
        /// <remarks>
        /// If client is not connected to the RTM service, the publish request 
        /// is queued. The SDK sends the message when the connection is established. 
        /// The length of this queue is limited by <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. 
        /// Task completes successfully when an acknowledgement from the server is received. </returns>
        /// <param name="channel">The channel to which the <paramref name="message"/>  is published. </param>
        /// <param name="message">The message which is published. </param>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for serialization. </typeparam>
        Task<RtmPublishReply> Publish<T>(string channel, T message);

        /// <summary>
        /// See <see cref="Publish{T}(string, T)"/>. 
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Publish<T>(string channel, T message, Action<RtmPublishReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Publishes the <paramref name="message"/> to the <paramref name="channel"/>.
        /// </summary>
        /// <remarks>
        /// If client is not connected to the RTM service, the publish request 
        /// is queued. The SDK sends the message when the connection is established. 
        /// The length of this queue is limited by <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. </returns>
        /// <param name="channel">The channel to which the <paramref name="message"/>  is published. </param>
        /// <param name="message">The message which is published. </param>
        /// <param name="ack">Specifies whether an acknowledgement from the RTM service is needed. 
        /// See <see cref="Ack"/> for more details. </param>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for serialization. </typeparam>
        Task<RtmPublishReply> Publish<T>(string channel, T message, Ack ack);

        /// <summary>
        /// See <see cref="Publish{T}(string, T, Ack)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Publish<T>(string channel, T message, Ack ack, Action<RtmPublishReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Reads the massage from the specified <paramref name="channel"/>. 
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics: 
        /// <paramref name="channel"/> name represents a key and the last (and the only used) message 
        /// the channel represents a value. 
        /// In other words, a channel serves as a dictionary entry.  
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. 
        /// Get the message from <see cref="RtmReadReply"/>.<see cref="RtmReadReply{TPayload}.Message"/>. </returns>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for deserialization. </typeparam>
        Task<RtmReadReply<T>> Read<T>(string channel);

        /// <summary>
        /// See <see cref="Read{T}(string)"/>. 
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Read<T>(string channel, Action<RtmReadReply<T>> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Reads the message from the <see cref="RtmReadRequest"/>.<see cref="RtmReadRequest.Channel"/>. 
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics: 
        /// <see cref="RtmReadRequest"/>.<see cref="RtmReadRequest.Channel"/> name represents a key and the last 
        /// (and the only used) message the channel represents a value. 
        /// In other words, a channel serves as a dictionary entry.  
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. 
        /// Get the message from <see cref="RtmReadReply"/>.<see cref="RtmReadReply{TPayload}.Message"/>. </returns>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for deserialization. </typeparam>
        Task<RtmReadReply<T>> Read<T>(RtmReadRequest request);

        /// <summary>
        /// See <see cref="Read{T}(RtmReadRequest)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Read<T>(RtmReadRequest request, Action<RtmReadReply<T>> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Writes a <paramref name="message"/> to the specified <paramref name="channel"/>. 
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics: <paramref name="channel"/> 
        /// name represents a key and the last (and the only used) message the channel represents a value. 
        /// In other words, a channel serves as a dictionary entry.
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. 
        /// Task completes successfully when an acknowledgement from the server is received. </returns>
        /// <param name="channel">The channel to which the <paramref name="message"/>  is written. </param>
        /// <param name="message">The message which is written. </param>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for serialization. </typeparam>
        Task<RtmWriteReply> Write<T>(string channel, T message);

        /// <summary>
        /// See <see cref="Write{T}(string, T)"/>. 
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Write<T>(string channel, T message, Action<RtmWriteReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Writes a <paramref name="message"/> to the specified <paramref name="channel"/>. 
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics: <paramref name="channel"/> 
        /// name represents a key and the last (and the only used) message the channel represents a value. 
        /// In other words, a channel serves as a dictionary entry.
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. </returns>
        /// <param name="channel">The channel to which the <paramref name="message"/>  is written. </param>
        /// <param name="message">The message which is written. </param>
        /// <param name="ack">Specifies whether an acknowledgement from the RTM service is needed. 
        /// See <see cref="Ack"/> for more details. </param>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for serialization. </typeparam>
        Task<RtmWriteReply> Write<T>(string channel, T message, Ack ack);

        /// <summary>
        /// See <see cref="Write{T}(string, T, Ack)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Write<T>(string channel, T message, Ack ack, Action<RtmWriteReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Writes a <see cref="RtmWriteRequest"/>.<see cref="RtmWriteRequest{TPayload}.Message"/> 
        /// to the specified <see cref="RtmWriteRequest"/>.<see cref="RtmWriteRequest{TPayload}.Channel"/>. 
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics: 
        /// <see cref="RtmWriteRequest"/>.<see cref="RtmWriteRequest{TPayload}.Channel"/>
        /// name represents a key and the last (and the only used) message the channel represents a value. 
        /// In other words, a channel serves as a dictionary entry.
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. </returns>
        /// <param name="request">The request which contains the channel and the message to be written</param>
        /// <param name="ack">Specifies whether an acknowledgement from the RTM service is needed. 
        /// See <see cref="Ack"/> for more details. </param>
        /// <typeparam name="T">It can be any reference or value type. The Newtonsoft.Json library is
        /// used for serialization. </typeparam>
        Task<RtmWriteReply> Write<T>(RtmWriteRequest<T> request, Ack ack);

        /// <summary>
        /// See <see cref="Write{T}(RtmWriteRequest{T}, Ack)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Write<T>(RtmWriteRequest<T> request, Ack ack, Action<RtmWriteReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Deletes a value from the specified <paramref name="channel"/>.
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics to erase a value 
        /// for a given key. Key is represented by a <paramref name="channel"/>, and only the last 
        /// message in the channel is relevant (represents the value). 
        /// Hence, publishing a null value, serves as deletion of the the previous value (if any).
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. 
        /// Task completes successfully when an acknowledgement from the server is received. </returns>
        /// <param name="channel">The channel represents a key</param>
        Task<RtmDeleteReply> Delete(string channel);

        /// <summary>
        /// See <see cref="Delete(string)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Delete(string channel, Action<RtmDeleteReply> onSuccess, Action<Exception> onFailure);

        /// <summary>
        /// Deletes a value from the specified <paramref name="channel"/>.
        /// </summary>
        /// <remarks>
        /// This method is provided for key-value (dictionary storage) semantics to erase a value 
        /// for a given key. Key is represented by a <paramref name="channel"/>, and only the last 
        /// message in the channel is relevant (represents the value). 
        /// Hence, publishing a null value, serves as deletion of the the previous value (if any).
        /// <para/>
        /// If client is not connected to the RTM service, the read request 
        /// is queued. The SDK reads the value when the connection is established. 
        /// The length of this queue is limited by 
        /// <see cref="RtmClientBuilder.PendingActionQueueLength"/>.
        /// If the queue is full, the <see cref="QueueFullException"/> exception is returned in the task. 
        /// </remarks>
        /// <returns>The task which contains the successful reply or exception. </returns>
        /// <param name="channel">The channel represents a key</param>
        /// <param name="ack">Specifies whether an acknowledgement from the RTM service is needed. 
        /// See <see cref="Ack"/> for more details. </param>
        Task<RtmDeleteReply> Delete(string channel, Ack ack);

        /// <summary>
        /// See <see cref="Delete(string, Ack)"/>.
        /// Callbacks are invoked on the <see cref="RtmClientBuilder.Dispatcher"/>.
        /// </summary>
        void Delete(string channel, Ack ack, Action<RtmDeleteReply> onSuccess, Action<Exception> onFailure);

        #endregion
    }
}