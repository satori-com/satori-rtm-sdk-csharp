#pragma warning disable 1591

using System;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satori.Common;

namespace Satori.Rtm
{
    public partial class WsSerialization : ISerialization
    {
        private readonly MonitorAsync _sendMon = new MonitorAsync();
        private readonly MonitorAsync _recvMon = new MonitorAsync();
        private WebSocket _wsock;
        private int _pduSizeEstimation = 1024;

        public WsSerialization(WebSocket wsock, Logger log)
        {
            _wsock = wsock;
            Log = log;
        }

        public Logger Log { get; } = Client.DefaultLoggers.Serialization;

        // for diagnostics
        public bool IsDisposed => _wsock == null;

        public static async Task<WsSerialization> Connect(Uri uri, CancellationToken ct, Logger log)
        {
            log.V("Connecting to '{0}', host: {1}, port: {2}", uri?.OriginalString, uri?.Host, uri?.Port);

            var wsock = new System.Net.WebSockets.Managed.ClientWebSocket();
            wsock.Options.KeepAliveInterval = TimeSpan.FromSeconds(60);
            try
            {
                await wsock.ConnectAsync(uri, ct).ConfigureAwait(false);
            }
            catch (Exception exn)
            {
                if (!ct.IsCancellationRequested)
                {
                    log.W(exn, "Connecting failed");
                }
                else
                {
                    log.V("Connecting aborted");
                }

                throw;
            }

            return new WsSerialization(wsock, log);
        }

        public async Task Send(Pdu pdu)
        {
            Log.V("Send method started, pdu: {0}", pdu);

            ArraySegment<byte> data;
            var jobj = JObject.FromObject(pdu);
            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                jobj.WriteTo(new JsonTextWriter(sw)
                {
#if DEBUG
                    Formatting = Formatting.Indented
#else
                    Formatting = Formatting.None
#endif
                });
                var json = sw.ToString();
                Log.V("Serialized PDU to json: {0}, pdu: {1}", json, pdu);

                data = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            }

            await _sendMon.Enter().ConfigureAwait(false);
            try
            {
                var wsock = _wsock;
                if (wsock == null)
                {
                    throw new Exception($"Sending PDU is aborted because websocket is disposed, pdu: {pdu}");
                }

                await wsock.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                _sendMon.Leave();
            }

            Log.V("Send method completed, pdu: {0}", pdu);
        }

        /// <summary>
        /// Receive next PDU
        /// </summary>
        /// <returns>Task completes with PDU or exception</returns>
        /// <exception cref="WebSocketException">Exception from WebSocket. 
        /// When aborted, observed inner exception as OperationCanceledException.
        /// However user should rely on disposable property to check if it was a requested closure </exception>
        public async Task<Pdu> Recv()
        {
            Log.V("Read method started");
            var seg = new ArraySegment<byte>(new byte[_pduSizeEstimation]);
            await _recvMon.Enter().ConfigureAwait(false);
            try
            {
                while (true)
                {
                    var wsock = _wsock;
                    if (wsock == null)
                    {
                        throw new Exception("Reading PDU is aborted because websocket is disposed");
                    }

                    Log.V("Waiting for data");
                    var res = await wsock.ReceiveAsync(seg, CancellationToken.None).ConfigureAwait(false);
                    Log.V("Processing data");
                    seg = new ArraySegment<byte>(seg.Array, seg.Offset + res.Count, seg.Count - res.Count);
                    if (res.EndOfMessage)
                    {
                        if (res.MessageType == WebSocketMessageType.Close)
                        {
                            Log.V("Close message received");
                            _wsock = null;
                            return null;
                        }

                        _pduSizeEstimation = ((_pduSizeEstimation * 95) + (seg.Offset * 5) + 99) / 100;
                        break;
                    }

                    if (seg.Count <= 0)
                    {
                        // expand buffer
                        var newArray = new byte[seg.Array.Length * 2];
                        Buffer.BlockCopy(seg.Array, 0, newArray, 0, seg.Offset);
                        seg = new ArraySegment<byte>(newArray, seg.Offset, newArray.Length - seg.Offset);
                        Log.I("Read buffer expanded, new size: {0}", newArray.Length);
                    }
                }
            }
            finally
            {
                _recvMon.Leave();
            }

            Log.V("Deserializing PDU");
            var pdu = JsonConvert.DeserializeObject<Pdu>(
                Encoding.UTF8.GetString(seg.Array, 0, seg.Offset));
            if (Log.V())
            {
                Log.V("Received PDU: {0}", JsonConvert.SerializeObject(pdu, Formatting.Indented));
            }

            Log.V("Read method finished");
            return pdu;
        }

        /// <summary>
        /// Aborts transport operations. Tasks currently in operation, such as receiving, 
        /// will complete (with Exception).
        /// </summary>
        public void Dispose()
        {
            Log.V("Disposing serialization object");
            var ws = Interlocked.Exchange(ref _wsock, null);
            if (ws == null)
            {
                Log.V("Serilalization object has been already disposed");
            }
            else
            {
                try
                {
                    ws.Dispose();
                }
                catch (Exception exn)
                {
                    Log.W(exn, "Disposing websocket failed");
                }
            }
        }

        public async Task Close()
        {
            Log.V("Closing serialization object");
            var ws = Interlocked.Exchange(ref _wsock, null);
            if (ws == null)
            {
                Log.V("Serilalization object has been already disposed");
                return;
            }

            if (ws.State != WebSocketState.Closed && ws.State != WebSocketState.Aborted)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", CancellationToken.None);
            }

            Dispose();
        }
    }
}