// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET_4_6
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    internal static class SocketExtensions
    {
        internal static Task ConnectAsync(this Socket socket, IPAddress address, int port)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            socket.BeginConnect(address, port, iar =>
            {
                var innerTcs = (TaskCompletionSource<bool>)iar.AsyncState;
                try
                {
                    ((Socket)innerTcs.Task.AsyncState).EndConnect(iar);
                    innerTcs.TrySetResult(true);
                }
                catch (Exception e) { innerTcs.TrySetException(e); }
            }, tcs);
            return tcs.Task;
        }
    }
}
#endif
