#pragma warning disable 1591

using System;

namespace Satori.Rtm
{
    public class PduException : Exception
    {
        public readonly Pdu Pdu;
        public readonly RtmError Error;

        public PduException(Pdu pdu, RtmError error)
        {
            Pdu = pdu;
            Error = error;
        }

        public PduException(Pdu pdu) : this(pdu, pdu.Body?.ToObject<RtmError>()) 
        {
        }
    }

    public class PduException<T> : PduException where T : RtmError
    {
        public PduException(Pdu pdu) : base(pdu, pdu.Body?.ToObject<T>()) 
        {
        }

        public new T Error => (T)base.Error;
    }
}
