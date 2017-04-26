#pragma warning disable 1591

using System;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State
        {
            public class Connected : State
            {
                private readonly IConnection _connection;

                public Connected(Connecting prev, IConnection connection) : base(prev)
                {
                    _connection = connection;
                }

                public override void OnEnter(State prev)
                {
                    Log.V("Entering state: {0}", this);
                    Fsm.ResetReconnectInterval();
                    Fsm.NotifyEnterConnected(_connection);
                }

                public override void OnLeave()
                {
                    Log.V("Leaving state: {0}", this);
                    Fsm.NotifyLeaveConnected(_connection);
                    try
                    {
                        _connection.Close();
                    }
                    catch (Exception exn)
                    {
                        Log.E(exn, "Failed to close connection, state: {0}", this);
                    }
                }

                public override bool Process()
                {
                    Log.V("Processing state: {0}", this);
                    ReadLoop();
                    Fsm.OnProcessConnected(_connection);
                    return true;
                }

                public override State Start()
                {
                    Log.V("Handling start, state: {0}", this);

                    // already started, do nothing
                    return null;
                }

                public override State Stop()
                {
                    Log.V("Handling stop, state: {0}", this);
                    return Complete(new Stopped(this));
                }

                public override IConnection GetConnection()
                {
                    if (_process.IsCompleted)
                    {
                        Log.V("GetConnection method returns null because state is completed, state: {0}", this);
                        return null;
                    }

                    return _connection;
                }

                private async void ReadLoop()
                {
                    Log.V("Starting read loop, state: {0}", this);

                    Exception error = null;
                    try
                    {
                        var exit = false;
                        do
                        {
                            Log.V("Awaiting for step in read loop, state: {0}", this);

                            ConnectionStepResult step;
                            try
                            {
                                step = await _connection.DoStep().ConfigureAwait(false);
                            }
                            finally
                            {
                                await this.Yield();
                            }

                            Log.V("Step completed in read loop, state: {0}", this);

                            if (_process.IsCompleted)
                            {
                                Log.V("Exit read loop because state is completed, state: {0}", this);
                                return;
                            }

                            exit = step.Match(
                                disconnected: _ =>
                                {
                                    Log.V("Disconnected step, state: {0}", this);
                                    return true;
                                },
                                expectedReply: _ =>
                                {
                                    Log.V("Expected reply step, state: {0}", this);
                                    return false;
                                },
                                unexpectedReply: _ =>
                                {
                                    Log.W("Unexpected reply step, state: {0}", this);
                                    return false;
                                },
                                unsolicitedEvent: uns =>
                                {
                                    var pdu = uns.pdu;
                                    if (pdu.Action == RtmActions.CoreError)
                                    {
                                        Log.W("Unsolicited error step, state: {0}, pdu: {1}", this, pdu);
                                        error = new PduException(pdu);
                                        return true;
                                    }
                                    
                                    Log.V("Unsolicited event step, state: {0}, pdu: {1}", this, pdu);
                                    
                                    Fsm.NotifyUnsolicitedEvent(pdu);
                                    return false;
                                },
                                error: e =>
                                {
                                    Log.V("Error step, state: {0}, error: {1}", this, e?.error);
                                    error = e.error;
                                    return true;
                                });
                        } while (!exit);
                    }
                    catch (Exception exn)
                    {
                        Log.E(exn, "Exception in ReadLoop methid, state: {0}", this);
                        error = exn;
                    }

                    if (error != null)
                    {
                        Fsm.NotifyError(error);
                    }

                    _process.Succeed(new Awaiting(this));
                }
            }
        }
    }
}