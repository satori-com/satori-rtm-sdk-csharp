using System;
using System.Threading;
using System.Threading.Tasks;
using Satori.Common;

namespace Satori.Rtm.Test
{
    public class TestConnection : IConnection
    {
        private readonly IConnection _connection;
        private readonly Func<ConnectionOperationResult, ConnectionOperationResult> _operationTransform;
        private readonly Func<ConnectionStepResult, ConnectionStepResult> _stepTransform;
        private readonly QueueAsync<Pdu<object>> _requests;
        private readonly QueueAsync<Pdu> _replies;

        public TestConnection(
            IConnection connection, 
            Func<ConnectionOperationResult, ConnectionOperationResult> operationTransform,
            Func<ConnectionStepResult, ConnectionStepResult> stepTransform,
            QueueAsync<Pdu<object>> requests, 
            QueueAsync<Pdu> replies)
        {
            _connection = connection;
            _operationTransform = operationTransform;
            _stepTransform = stepTransform;
            _requests = requests;
            _replies = replies;
        }

        public static async Task<IConnection> Connect(string url, CancellationToken ct, Func<ConnectionStepResult, ConnectionStepResult> transform)
        {
            var con = await Connection.Connect(url, ct).ConfigureAwait(false);
            return new TestConnection(con, null, transform, null, null);
        }

        public static async Task<IConnection> Connect(string url, CancellationToken ct, Func<ConnectionOperationResult, ConnectionOperationResult> transform)
        {
            var con = await Connection.Connect(url, ct).ConfigureAwait(false);
            return new TestConnection(con, transform, null, null, null);
        }

        public static async Task<IConnection> Connect(
            string url, 
            CancellationToken ct, 
            QueueAsync<Pdu<object>> requests,
            QueueAsync<Pdu> replies)
        {
            var con = await Connection.Connect(url, ct).ConfigureAwait(false);
            return new TestConnection(con, null, null, requests, replies);
        }

        public Task Close()
        {
            return _connection.Close();
        }

        public async Task<ConnectionOperationResult> DoOperationAsync<T>(string action, T body)
        {
            _requests?.Enqueue(
                new Pdu<object>
                { 
                    Action = action, 
                    Body = body
                });

            var res = await _connection.DoOperationAsync(action, body);

            var reply = res.AsReply();
            if (reply != null) 
            {
                _replies?.Enqueue(reply.pdu);
            }

            return _operationTransform?.Invoke(res) ?? res;
        }

        public Task DoOperationNoAckAsync<T>(string action, T body)
        {
            _requests?.Enqueue(
                new Pdu<object>
            { 
                Action = action, 
                Body = body
            });
            return _connection.DoOperationNoAckAsync(action, body);
        }

        public async Task<ConnectionStepResult> DoStep()
        {
            var res = await _connection.DoStep();
            return _stepTransform?.Invoke(res) ?? res;
        }
    }
}
