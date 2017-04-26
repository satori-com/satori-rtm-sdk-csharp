#pragma warning disable 1591

using System;
using System.Threading;
using System.Threading.Tasks;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public abstract partial class State
        {
            public class Connecting : State
            {
                private readonly CancellationTokenSource _cts = new CancellationTokenSource();

                public Connecting(State prev) : base(prev)
                {
                }

                public override void OnEnter(State prev)
                {
                    Log.V("Entering state: {0}", this);
                    Fsm.NotifyEnterConnecting();
                }

                public override void OnLeave()
                {
                    Log.V("Leaving state: {0}", this);
                    Fsm.NotifyLeaveConnecting();
                    try
                    {
                        _cts.Cancel();
                    }
                    catch (Exception exn)
                    {
                        Log.E(exn, "Unhandled exception while cancelling connection");
                        UnhandledExceptionWatcher.Swallow(exn);
                    }
                }

                public override bool Process()
                {
                    Log.V("Processing state: {0}", this);
                    Connect();
                    return true;
                }

                public override State Start()
                {
                    Log.V("Handling start, state: {0}", this);

                    // already started
                    return null;
                }

                public override State Stop()
                {
                    Log.V("Handling stop, state: {0}", this);
                    var res = Complete(new Stopped(this));
                    return res;
                }

                private async Task<IConnection> CreateConnection(CancellationToken ct)
                {
                    IConnection con;
                    try
                    {
                        Log.V("CreateConnection method started, state: {0}", this);
                        try
                        {
                            con = await Fsm._connector(Fsm._url, ct).ConfigureAwait(false);
                            Log.V("Connector callback completed, state: {0}", this);

                            if (Fsm._authenticator != null)
                            {
                                Log.I("Authenticting, state: {0}", this);
                                try
                                {
                                    var t = Fsm._authenticator(con);
                                    while (t.Status == TaskStatus.WaitingForActivation)
                                    {
                                        var step = await con.DoStep();
                                        if (step.IsDisconnected())
                                        {
                                            throw new DisconnectedException();
                                        }
                                    }

                                    var res = t.Result;
                                    Log.I("Authenticated, state: {0}", this);
                                }
                                catch (Exception exn)
                                {
                                    Log.W(exn, "Authentication failed, state: {0}", this);
                                    await con.Close().ConfigureAwait(false);
                                    throw;
                                }
                            }
                        }
                        finally
                        {
                            await Dispatcher.Yield();
                        }
                    }
                    catch (Exception exn)
                    {
                        Log.W(exn, "CreateConnection method failed, state: {0}", this);
                        throw exn;
                    }

                    Log.V("CreateConnection method completed, state: {0}", this);
                    return con;
                }

                private async void Connect()
                {
                    Log.V("Connect method started, state: {0}", this);
                    try
                    {
                        IConnection con;
                        try
                        {
                            con = await CreateConnection(_cts.Token);
                        }
                        finally
                        {
                            await this.Yield();
                        }

                        if (!_process.Succeed(new Connected(this, con)))
                        {
                            Log.V("Closing connection because current state already completed, state: {0}", this);
                            await con.Close();
                        }
                    }
                    catch (Exception exn)
                    {
                        if (_process.IsCompleted)
                        {
                            Log.V(exn, "Connect method failed but current state already completed, state: {0}", this);
                            return;
                        }

                        Log.W(exn, "Connect method failed, state: {0}", this);
                        Fsm.NotifyError(exn);
                        Complete(new Awaiting(this));
                    }

                    Log.V("Connect method completed, state: {0}", this);
                }
            }
        }
    }
}