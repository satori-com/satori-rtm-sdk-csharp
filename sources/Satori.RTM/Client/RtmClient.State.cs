#pragma warning disable 1591

using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State : IDispatchObject
        {
            protected readonly RtmClient Fsm;

            private readonly SuccessfulAwaiter<State> _process = new SuccessfulAwaiter<State>();

            protected State(RtmClient fsm)
            {
                Fsm = fsm;
            }

            protected State(State prev)
            {
                Fsm = prev.Fsm;
            }

            public IDispatcher Dispatcher => Fsm.Dispatcher;

            public ISuccessfulAwaiter<State> Next => _process;

            private Logger Log => Fsm.Log;

            public override string ToString()
            {
                return $"[{GetType().Name}]";
            }

            public abstract void OnEnter(State prev);

            public abstract void OnLeave();

            public virtual bool Process()
            {
                return true;
            }

            public virtual State Complete(State next)
            {
                if (_process.Succeed(next))
                { 
                    Log.V("Next state is set successfully, current: {0}, next: {1}", this, next);
                    return _process.Result;
                } 
                else  
                {
                    Log.V("Next state is not set because current state already completed, " +
                          "current: {0}, completed: {1}, next: {2}", this, _process?.Result, next);
                    return null;
                }
            }

            public abstract State Start();

            public abstract State Stop();

            public State Dispose()
            {
                Log.V("Disposing state: {0}", this);
                _process.Succeed(null);
                return null;
            }

            public virtual IConnection GetConnection()
            {
                Log.V("GetConnection method called, state: {0}", this);
                return null;
            }
        }
    }
}