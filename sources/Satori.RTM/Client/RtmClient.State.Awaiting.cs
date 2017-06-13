#pragma warning disable 1591

using System;
using System.Threading;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State
        {
            public class Awaiting : State
            {
                private Timer _timer;

                public Awaiting(State prev) : base(prev)
                {
                }

                public override void OnEnter(State prev)
                {
                    Log.V("Entering state: {0}", this);
                    Fsm.NotifyEnterAwaiting();
                }

                public override void OnLeave()
                {
                    Log.V("Leaving state: {0}", this);
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    Fsm.NotifyLeaveAwaiting();
                }

                public override bool Process()
                {
                    Log.V("Processing state: {0}", this);
                    try
                    {
                        TimeSpan interval = Fsm.NextReconnectInterval();
                        _timer = new Timer(TimerCallback, this, interval, TimeSpan.FromMilliseconds(-1));
                        Log.V("Timer is created, state: {0}", this);
                    }
                    catch (Exception exn)
                    {
                        Log.E(exn, "Failed to create timer, state: {0}", this);
                        Fsm.NotifyError(TaskHelper.Unwrap(exn));
                        Complete(new Stopped(this));
                    }

                    return true;
                }

                public override State Start()
                {
                    Log.V("Handling start, state: {0}", this);
                    return Complete(new Connecting(this));
                }

                public override State Stop()
                {
                    Log.V("Handling stop, state: '{0}'", this);
                    return Complete(new Stopped(this));
                }

                private static async void TimerCallback(object state)
                {
                    var s = (Awaiting)state;
                    s.Log.V("Timer callback executing, state: {0}", s);
                    await s.Yield();
                    s.Complete(new Connecting(s));
                }
            }
        }
    }
}