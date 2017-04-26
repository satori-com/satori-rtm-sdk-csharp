#pragma warning disable 1591

using System;
using System.Threading.Tasks;

namespace Satori.Rtm
{
    public interface ISerialization : IDisposable
    {
        Task<Pdu> Recv();

        Task Send(Pdu pdu);
    }
}
