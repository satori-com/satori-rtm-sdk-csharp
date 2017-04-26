#pragma warning disable 1591

namespace Satori.Rtm
{
    public class RtmSubscribeException : PduException<RtmSubscribeError>
    {
        public RtmSubscribeException(Pdu pdu) : base(pdu) 
        {
        }
    }

    public class RtmUnsubscribeException : PduException<RtmUnsubscribeError>
    {
        public RtmUnsubscribeException(Pdu pdu) : base(pdu) 
        {
        }
    }

    public class AuthException : PduException
    {
        public AuthException(Pdu pdu) : base(pdu) 
        {
        }
    }
}
