#pragma warning disable 1591

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        private readonly string _url;
        private readonly RtmModule _rtmModule;
        private State _state;

        private Func<string, CancellationToken, Task<IConnection>> _connector;
        private Func<IConnection, Task<JToken>> _authenticator;

        public RtmClient(
            Func<string, CancellationToken, Task<IConnection>> connector, 
            Func<IConnection, Task<JToken>> authenticator, 
            IDispatcher dispatcher, 
            string url, 
            TimeSpan minReconnectInterval, 
            TimeSpan maxReconnectInterval, 
            int pendingActionQueueLength)
        {
            if (minReconnectInterval <= TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(minReconnectInterval)} must be positive");
            }

            if (minReconnectInterval > maxReconnectInterval)
            {
                throw new ArgumentException($"{nameof(minReconnectInterval)} must be less or equal to {nameof(maxReconnectInterval)}");
            }

            if (pendingActionQueueLength < 0)
            {
                throw new ArgumentException($"{nameof(pendingActionQueueLength)} must pe nonnegative");
            }

            _connector = connector ?? DefaultConnector;
            _authenticator = authenticator;
            Dispatcher = dispatcher ?? new Dispatcher();
            _url = url;

            _state = new State.Uninitialized(this);
            _rtmModule = new RtmModule(this);

            _minReconnectInterval = minReconnectInterval;
            _maxReconnectInterval = maxReconnectInterval;
            _jitter = TimeSpan.FromMilliseconds(new Random().NextDouble() * minReconnectInterval.TotalMilliseconds);

            if (pendingActionQueueLength != 0)
            {
                _pendingRequests = new BlockingCollection<TaskCompletionSource<IConnection>>(pendingActionQueueLength);
            }
        }

        public Func<string, CancellationToken, Task<IConnection>> Connector => _connector;

        public static Task<IConnection> DefaultConnector(string url, CancellationToken ct)
        {
            return Connection.Connect(url, ct);
        }

        private void Cleanup()
        {
            Log.V("Cleaning up RTM client: {0}", this);

            _rtmModule.Dispose();

            if (_pendingRequests != null)
            {
                TaskCompletionSource<IConnection> tcs;
                while (_pendingRequests.TryTake(out tcs))
                {
                    tcs.TrySetCanceled();
                }

                _pendingRequests.Dispose();
            }

            OnEnterStopped = null;
            OnLeaveStopped = null;
            OnEnterConnecting = null;
            OnLeaveConnecting = null;
            OnEnterConnected = null;
            OnLeaveConnected = null;
            OnEnterAwaiting = null;
            OnLeaveAwaiting = null;
            OnEnterDisposed = null;
            OnError = null;

            OnUnsolicitedEvent = null;
        }

        private async void DoTransitionLoop()
        {
            Log.V("Transition loop started");

            while (true)
            {
                var next = _state.Next.GetResult();
                var prev = _state;

                // do transition
                _state.OnLeave();

                Log.I("Transitioning from '{0}' to '{1}'", prev, next);

                _state = next;
                if (_state == null)
                {
                    NotifyEnterDisposed();
                    break;
                }

                _state.OnEnter(prev);

                // start processing and await for the next state
                if (!_state.Process())
                {
                    break;
                }

                await _state.Next;
            }

            Log.V("Transition loop completed");
        }

        private void OnProcessConnected(IConnection connection)
        {            
            Log.V("Resuming pending requests because client is connected");
            TaskCompletionSource<IConnection> tcs;
            while (_pendingRequests != null && _pendingRequests.TryTake(out tcs))
            {
                tcs.TrySetResult(connection);
            }
        }

        private void NotifyEnterStopped()
        {
            OnEnterStopped.InvokeSafe();
        }

        private void NotifyLeaveStopped()
        {
            OnLeaveStopped.InvokeSafe();
        }

        private void NotifyEnterConnecting()
        {
            OnEnterConnecting.InvokeSafe();
        }

        private void NotifyLeaveConnecting()
        {
            OnLeaveConnecting.InvokeSafe();
        }

        private void NotifyEnterConnected(IConnection connection)
        {
            OnEnterConnected.InvokeSafe(connection);
        }

        private void NotifyLeaveConnected(IConnection connection)
        {
            OnLeaveConnected.InvokeSafe(connection);
        }

        private void NotifyEnterAwaiting()
        {
            OnEnterAwaiting.InvokeSafe();
        }

        private void NotifyLeaveAwaiting()
        {
            OnLeaveAwaiting.InvokeSafe();
        }

        private void NotifyEnterDisposed()
        {
            OnEnterDisposed.InvokeSafe();
        }

        private void NotifyUnsolicitedEvent(Pdu pdu)
        {
            OnUnsolicitedEvent.InvokeSafe(pdu);
        }

        private void NotifyError(Exception error)
        {
            OnError.InvokeSafe(error);
        }
    }
}