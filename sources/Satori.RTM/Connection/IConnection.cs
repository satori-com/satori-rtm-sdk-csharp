#pragma warning disable 1591

using System.Threading.Tasks;

namespace Satori.Rtm
{
    /// <summary>
    /// Describes a connection to the RTM service. 
    /// An instance of this interface is created every time 
    /// when a <see cref="Client.IRtmClient"/> client enters 
    /// a connecting state. 
    /// A custom implementation of this interface can be provided 
    /// to a <see cref="Client.IRtmClient"/> client via 
    /// <see cref="Client.RtmClientBuilder.Connector"/> property of 
    /// the <see cref="Client.RtmClientBuilder"/> builder.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Sends a PDU to the RTM service. the PDU is constructed with <paramref name="action"/> 
        /// and <paramref name="body"/>. The id filed of the PDU is assigned automatically. 
        /// </summary>
        /// <returns>The task which completes when an acknowledgement is received from the RTM service.</returns>
        /// <param name="action">The action field of the PDU.</param>
        /// <param name="body">The body filed of the PDU</param>
        /// <typeparam name="T">The type of the body could be any reference or value type. 
        /// Newtonsoft.Json library is used for serialization. </typeparam>
        Task<ConnectionOperationResult> DoOperationAsync<T>(string action, T body);

        /// <summary>
        /// Similar to <see cref="DoOperationAsync{T}"/> method except that it doesn't 
        /// receive an acknowledgement from the RTM service.  
        /// </summary>
        /// <returns>The task which completes when the PDU is sent.</returns>
        /// <param name="action">The action filed of the PDU</param>
        /// <param name="body">The body filed of the PDU</param>
        /// <typeparam name="T">The type of the body could be any reference or value type.
        /// Newtonsoft.Json library is used for serialization. </typeparam>
        Task DoOperationNoAckAsync<T>(string action, T body);

        /// <summary>
        /// Reads a single PDU from the RTM service. 
        /// This method should be looped in order to receive solicited 
        /// and unsolicited PDUs. 
        /// </summary>
        /// <returns>See <see cref="ConnectionStepResult"/> for possible results.</returns>
        Task<ConnectionStepResult> DoStep();

        /// <summary>
        /// Sends a close message to websocket and disposes this connection. 
        /// </summary>
        /// <returns>The task completes when a websocket is closed and the connection is disposed. </returns>
        Task Close();
    }
}