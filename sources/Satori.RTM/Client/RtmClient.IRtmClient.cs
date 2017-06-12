#pragma warning disable 1591

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient : IRtmClient
    {
        private readonly TimeSpan _minReconnectInterval;
        private readonly TimeSpan _maxReconnectInterval;
        private readonly TimeSpan _jitter;
        private readonly BlockingCollection<TaskCompletionSource<IConnection>> _pendingRequests;

        private long _failCount;

        public event Action OnEnterStopped;

        public event Action OnLeaveStopped;

        public event Action OnEnterConnecting;

        public event Action OnLeaveConnecting;

        public event Action<IConnection> OnEnterConnected;

        public event Action<IConnection> OnLeaveConnected;

        public event Action OnEnterAwaiting;

        public event Action OnLeaveAwaiting;

        public event Action OnEnterDisposed;

        public event Action<Exception> OnError;

        private event Action<Pdu> OnUnsolicitedEvent;

        public IDispatcher Dispatcher { get; private set; }

        public async void Start()
        {
            await StartImpl().ConfigureAwait(false);
        }

        public async void Stop()
        {
            await StopImpl().ConfigureAwait(false);
        }

        public async void Restart()
        {
            await RestartImpl().ConfigureAwait(false);
        }

        public async Task Dispose()
        {
            Log.V("Dispose method is dispatched");
            await this.Yield();
            Log.V("Dispose method is executing");
            if (_state != null)
            {
                _state.Dispose();
                Cleanup();
            } 
            else 
            {
                Log.W("Dispose method is ignored because client is disposed");
            }

            Log.V("Dispose method is completed");
        }

        public async Task<IConnection> GetConnection()
        {
            Log.V("GetConnection method is dispatched");
            await this.Yield();
            Log.V("GetConnection method is executing");
            var conn = _state.GetConnection();
            if (conn != null)
            {
                Log.V("GetConnection is completed");
                return conn;
            }

            Log.V("GetConnection is pending because client is not connected");
            var tcs = new TaskCompletionSource<IConnection>();
            if (_pendingRequests == null || !_pendingRequests.TryAdd(tcs))
            {
                throw new TooManyRequestsException($"Too many requests are queued: {_pendingRequests?.Count() ?? 0}");
            }

            return await tcs.Task;
        }

        #region RTM

        public void CreateSubscription(
            string channel, 
            SubscriptionModes mode,
            ISubscriptionObserver observer)
        {
            _rtmModule.CreateSubscription(
                channel, 
                new SubscriptionConfig(mode)
                {
                    Observer = observer
                });
        }

        public void CreateSubscription(string channel, ISubscriptionObserver observer)
        {
            CreateSubscription(channel, SubscriptionModes.Simple, observer);
        }
        
        public void CreateSubscription(string channelOrSubId, SubscriptionConfig subscriptionConfig)
        {
            _rtmModule.CreateSubscription(channelOrSubId, subscriptionConfig);
        }

        public void RemoveSubscription(string channelOrSubId)
        {
            _rtmModule.RemoveSubscription(channelOrSubId);
        }

        public Task<ISubscription> GetSubscription(string channelOrSubId)
        {
            return _rtmModule.GetSubscription(channelOrSubId);
        }

        public Task<RtmPublishReply> Publish<T>(string channel, T message, Ack ack)
        {
            return _rtmModule.Publish(channel, message, ack);
        }

        public Task<RtmPublishReply> Publish<T>(string channel, T message)
        {
            return Publish(channel, message, Ack.Yes);
        }

        public Task<RtmReadReply<T>> Read<T>(string channel)
        {
            return _rtmModule.Read<T>(channel);
        }

        public Task<RtmReadReply<T>> Read<T>(RtmReadRequest request)
        {
            return _rtmModule.Read<T>(request);
        }

        public Task<RtmWriteReply> Write<T>(string channel, T message, Ack ack)
        {
            return _rtmModule.Write(channel, message, ack);
        }

        public Task<RtmWriteReply> Write<T>(string channel, T message)
        {
            return Write(channel, message, Ack.Yes);
        }

        public Task<RtmWriteReply> Write<T>(RtmWriteRequest<T> request, Ack ack)
        {
            return _rtmModule.Write(request, ack);
        }

        public Task<RtmDeleteReply> Delete(string channel, Ack ack)
        {
            return _rtmModule.Delete(channel, ack);
        }

        public Task<RtmDeleteReply> Delete(string channel)
        {
            return Delete(channel, Ack.Yes);
        }

        #endregion

        internal async Task StartImpl()
        {
            Log.V("Start method is dispatched");
            await this.Yield();
            Log.V("Start method is executing");
            if (_state == null)
            {
                Log.V("Start method is ignored because client is disposed");
            }
            else
            {
                var appliedState = _state.Start();
                Log.V("Start method is completed, applied state: {0}", appliedState);
            }
        }

        internal async Task StopImpl()
        {
            Log.V("Stop method is dispatched");
            await this.Yield();
            Log.V("Stop() method is executing");
            if (_state == null)
            {
                Log.V("Stop method is ignored because client is disposed");
            }
            else
            {
                var appliedState = _state.Stop();
                Log.V("Stop method is completed, applied state: {0}", appliedState);
            }
        }

        internal async Task RestartImpl()
        {
            await StopImpl();
            await StartImpl();
        }

        internal ISuccessfulAwaiter<State> GetStateAsync()
        {
            return Dispatcher.Yield().Map(_ => _state);
        }

        internal State GetState() => _state;

        internal TimeSpan NextReconnectInterval()
        {
            const int MaxPow = 30;
            long count = Math.Min(_failCount, MaxPow);
            double min = _minReconnectInterval.TotalMilliseconds;
            double max = _maxReconnectInterval.TotalMilliseconds;
            double jtr = _jitter.TotalMilliseconds;

            double offset = Math.Min(max, jtr + (min * Math.Pow(2, count)));
            _failCount += 1;
            Log.V("Reconnect interval is changed to {0} ms, number of attempts: {1}", offset, _failCount);
            return TimeSpan.FromMilliseconds(offset);
        }

        internal void ResetReconnectInterval()
        {
            _failCount = 0;
            Log.V("Number of reconnect attempts is set to 0");
        }
    }
}