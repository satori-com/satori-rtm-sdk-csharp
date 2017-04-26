#pragma warning disable 1591

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State
        {
            public class Uninitialized : Stopped
            {
                public Uninitialized(RtmClient fsm) : base(fsm)
                {
                }

                public override void OnEnter(State prev)
                {
                    Log.V("Entering state: {0}", this);

                    base.OnEnter(prev);
                    if (_process.Result != null)
                    {
                        Fsm.Cleanup();
                    }
                }

                public override bool Process()
                {
                    Log.V("Processing state: {0}", this);
                    return false; // break loop
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

                public override State Complete(State next)
                {
                    Log.V("Completing state: {0}, next: {1}", this, next);

                    if (_process.Succeed(next))
                    {
                        Log.V("Next state is set successfully, current: {0}, next: {1}", this, next);
                        Fsm.DoTransitionLoop();
                    }

                    return _process.Result;
                }
            }
        }
    }
}