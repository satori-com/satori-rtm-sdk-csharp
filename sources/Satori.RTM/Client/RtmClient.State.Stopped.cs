#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State
        {
            public class Stopped : State
            {
                public Stopped(State prev) : base(prev)
                {
                }

                protected Stopped(RtmClient fsm) : base(fsm)
                {
                }

                public override void OnEnter(State prev)
                {
                    Log.V("Entering state: {0}", this);
                    Fsm.NotifyEnterStopped();
                }

                public override void OnLeave()
                {
                    Log.V("Leaving state: {0}", this);
                    Fsm.NotifyLeaveStopped();
                }

                public override State Start()
                {
                    Log.V("Handling start, state: {0}", this);
                    return Complete(new Connecting(this));
                }

                public override State Stop()
                {
                    Log.V("Handling stop, state: {0}", this);
                    return this;
                }
            }
        }
    }
}