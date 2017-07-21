#pragma warning disable 1591

namespace Satori.Rtm
{
    public class SubscribeException : PduException<RtmSubscribeError>
    {
        public SubscribeException(Pdu pdu) : base(pdu) 
        {
        }
    }

    public class UnsubscribeException : PduException<RtmUnsubscribeError>
    {
        public UnsubscribeException(Pdu pdu) : base(pdu) 
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
