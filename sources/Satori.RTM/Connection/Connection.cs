#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Satori.Common;
using Satori.Rtm.Client;

namespace Satori.Rtm
{
    public partial class Connection : IConnection, IDisposable
    {
        private readonly WsSerialization _plink;
        private long _id = 0;
        private Dictionary<string, TaskCompletionSource<ConnectionOperationResult>> _hash = new Dictionary<string, TaskCompletionSource<ConnectionOperationResult>>();

        public Connection(WsSerialization plink)
        {
            _plink = plink;
        }

        public Logger Log { get; } = DefaultLoggers.Connection;

        public static async Task<IConnection> Connect(string url, ConnectionOptions options, CancellationToken ct)
        {
            var plink = await WsSerialization.Connect(new Uri(url), options, ct, DefaultLoggers.Serialization).ConfigureAwait(false);
            return new Connection(plink);
        }

        public async Task<ConnectionStepResult> DoStep()
        {
            var pdu = await _plink.Recv().ConfigureAwait(false);

            if (pdu == null)
            {
                Dispose();
                return new ConnectionStepResult.Disconnected();
            }

            var id = pdu.Id;
            if (id == null)
            {
                return new ConnectionStepResult.UnsolicitedEvent(pdu);
            }

            TaskCompletionSource<ConnectionOperationResult> tcs;
            lock (_hash)
            {
                if (!_hash.TryGetValue(id, out tcs))
                {
                    return new ConnectionStepResult.UnexpectedReply(pdu);
                }

                _hash.Remove(id);
            }

            // notify about operation completion
            if (pdu.Action.EndsWith("/ok"))
            {
                var res = new ConnectionOperationResult.Reply.Positive(pdu);
                if (tcs != null)
                {
                    tcs.TrySetResult(res);
                }

                return new ConnectionStepResult.ExpectedReply.Positive(pdu);
            }
            else if (pdu.Action.EndsWith("/error"))
            {
                var res = new ConnectionOperationResult.Reply.Negative(pdu);
                if (tcs != null)
                {
                    tcs.TrySetResult(res);
                }

                return new ConnectionStepResult.ExpectedReply.Negative(pdu);
            }
            else
            {
                var res = new ConnectionOperationResult.Reply.Unknown(pdu);
                if (tcs != null)
                {
                    tcs.TrySetResult(res);
                }

                return new ConnectionStepResult.ExpectedReply.UnknownOutcome(pdu);
            }
        }

        public async Task<ConnectionOperationResult> DoOperationAsync<T>(string action, T body)
        {
            var id = GenNewId();
            var tcs = new TaskCompletionSource<ConnectionOperationResult>();
            lock (_hash)
            {
                _hash[id] = tcs;
            }

            try
            {
                await _plink.Send(
                    new Pdu
                    {
                        Action = action,
                        Body = body != null ? JToken.FromObject(body) : null,
                        Id = id
                    }).ConfigureAwait(false);
            }
            catch
            {
                lock (_hash)
                {
                    _hash.Remove(id);
                }

                throw;
            }

            return await tcs.Task;
        }

        public Task DoOperationNoAckAsync<T>(string action, T body)
        {
            return _plink.Send(
                new Pdu
                {
                    Action = action,
                    Body = body != null ? JToken.FromObject(body) : null
                });
        }

        public async Task Close()
        {
            try
            {
                await _plink.Close().ConfigureAwait(false);
            }
            catch (Exception exn)
            {
                Log.W(exn, "Failed to close serialization link properly");
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            TaskCompletionSource<ConnectionOperationResult>[] ops;
            lock (_hash)
            {
                var vals = _hash.Values;
                ops = new TaskCompletionSource<ConnectionOperationResult>[vals.Count];
                vals.CopyTo(ops, 0);
            }

            foreach (var op in ops)
            {
                op.TrySetResult(new ConnectionOperationResult.Disconnected());
            }

            _plink.Dispose();
        }

        private string GenNewId()
        {
            return Interlocked.Increment(ref _id).ToString();
        }
    }
}