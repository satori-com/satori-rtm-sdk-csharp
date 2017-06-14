// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Resources;
using System.Runtime.CompilerServices;

namespace System
{
    internal partial class SR
    {
        public static readonly string net_uri_NotAbsolute = "This operation is not supported for a relative URI.";
        public static readonly string net_WebSockets_Scheme = "Only Uris starting with 'ws://' or 'wss://' are supported.";
        public static readonly string net_WebSockets_AlreadyStarted = "The WebSocket has already been started.";
        public static readonly string net_WebSockets_NotConnected = "The WebSocket is not connected.";
        public static readonly string net_WebSockets_NoDuplicateProtocol = "Duplicate protocols are not allowed: '{0}'.";
        public static readonly string net_WebSockets_ArgumentOutOfRange_TooSmall = "The argument must be a value greater than {0}.";
        public static readonly string net_WebSockets_Argument_InvalidMessageType = "The message type '{0}' is not allowed for the '{1}' operation. Valid message types are: '{2}, {3}'. To close the WebSocket, use the '{4}' operation instead.";
        public static readonly string net_WebSockets_InvalidState_ClosedOrAborted = "The '{0}' instance cannot be used for communication because it has been transitioned into the '{1}' state.";
        public static readonly string net_Websockets_AlreadyOneOutstandingOperation = "here is already one outstanding '{0}' call for this WebSocket instance. ReceiveAsync and SendAsync can be called simultaneously, but at most one outstanding operation for each of them is allowed at the same time.";
        public static readonly string net_securityprotocolnotsupported = "The requested security protocol is not supported.";
        public static readonly string net_webstatus_ConnectFailure = "Unable to connect to the remote server";
        public static readonly string net_WebSockets_AcceptUnsupportedProtocol = "The WebSocket client request requested '{0}' protocol(s), but server is only accepting '{1}' protocol(s).";
        public static readonly string net_WebSockets_InvalidResponseHeader = "The '{0}' header value '{1}' is invalid.";
        public static readonly string net_WebSockets_InvalidState = "The WebSocket is in an invalid state ('{0}') for this operation. Valid states are: '{1}'";
        public static readonly string net_WebSockets_InvalidEmptySubProtocol = "Empty string is not a valid subprotocol value. Please use \\\"null\\\" to specify no value.";
        public static readonly string net_WebSockets_InvalidCharInProtocolString = "The WebSocket protocol '{0}' is invalid because it contains the invalid character '{1}'.";
        public static readonly string net_WebSockets_ReasonNotNull = "The close status description '{0}' is invalid. When using close status code '{1}' the description must be null.";
        public static readonly string net_WebSockets_InvalidCloseStatusCode = "The close status code '{0}' is reserved for system use only and cannot be specified when calling this method.";
        public static readonly string net_WebSockets_InvalidCloseStatusDescription = "The close status description '{0}' is too long. The UTF8-representation of the status description must not be longer than {1} bytes.";
        public static readonly string net_WebSockets_UnsupportedPlatform = "The WebSocket protocol is not supported on this platform.";
        public static readonly string net_WebSockets_Generic = "An internal WebSocket error occurred. Please see the innerException, if present, for more details. ";
        public static readonly string net_WebSockets_InvalidMessageType_Generic = "The received  message type is invalid after calling {0}. {0} should only be used if no more data is expected from the remote endpoint. Use '{1}' instead to keep being able to receive data but close the output channel.";
        public static readonly string net_Websockets_WebSocketBaseFaulted = "An exception caused the WebSocket to enter the Aborted state. Please see the InnerException, if present, for more details.";
        public static readonly string net_WebSockets_NotAWebSocket_Generic = "A WebSocket operation was called on a request or response that is not a WebSocket.";
        public static readonly string net_WebSockets_UnsupportedWebSocketVersion_Generic = "Unsupported WebSocket version.";
        public static readonly string net_WebSockets_UnsupportedProtocol_Generic = "The WebSocket request or response operation was called with unsupported protocol(s). ";
        public static readonly string net_WebSockets_HeaderError_Generic = "The WebSocket request or response contained unsupported header(s). ";
        public static readonly string net_WebSockets_ConnectionClosedPrematurely_Generic = "The remote party closed the WebSocket connection without completing the close handshake.";
        public static readonly string net_WebSockets_InvalidState_Generic = "The WebSocket instance cannot be used for communication because it has been transitioned into an invalid state.";

        // This method is used to decide if we need to append the exception message parameters to the message when calling SR.Format.
        // by default it returns false.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool UsingResourceKeys ()
        {
            return false;
        }

        internal static string Format (string resourceFormat, params object [] args)
        {
            if (args != null)
            {
                if (UsingResourceKeys())
                {
                    return resourceFormat + StringEx.Join(", ", args);
                }

                return string.Format(resourceFormat, args);
            }

            return resourceFormat;
        }

        internal static string Format (string resourceFormat, object p1)
        {
            if (UsingResourceKeys())
            {
                return StringEx.Join(", ", resourceFormat, p1);
            }

            return string.Format(resourceFormat, p1);
        }

        internal static string Format (string resourceFormat, object p1, object p2)
        {
            if (UsingResourceKeys())
            {
                return StringEx.Join(", ", resourceFormat, p1, p2);
            }

            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format (string resourceFormat, object p1, object p2, object p3)
        {
            if (UsingResourceKeys())
            {
                return StringEx.Join(", ", resourceFormat, p1, p2, p3);
            }

            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}