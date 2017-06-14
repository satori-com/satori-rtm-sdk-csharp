#pragma warning disable 1591

using System;
using System.Threading.Tasks;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class Subscription
    {
        public partial class State
        {
            private readonly SuccessfulAwaiter<State> _awaiter;
            private readonly Subscription _fsm;
            private readonly IConnection _connection;
            private readonly RtmSubscribeRequest _request;

            public State(Subscription fsm, IConnection connection)
            {
                _fsm = fsm;
                _connection = connection;
                _awaiter = new SuccessfulAwaiter<State>();

                if (IsSubscribing)
                {
                    _request = new RtmSubscribeRequest
                    {
                        Channel = string.IsNullOrEmpty(_fsm.Filter) ? _fsm.SubscriptionId : null,
                        SubscriptionId = string.IsNullOrEmpty(fsm.Filter) ? null : _fsm.SubscriptionId,
                        Filter = _fsm.Filter,
                        Period = _fsm.Period,
                        FastForward = _fsm.IsFastForward,
                        Position = _fsm.Position,
                        History = _fsm.History
                    };
                }
            }

            public bool IsUnsubscribed => this is Unsubscribed;

            public bool IsSubscribing => this is Subscribing;

            public bool IsSubscribed => this is Subscribed;

            public bool IsUnsubscribing => this is Unsubscribing;

            public bool IsFailed => this is Failed;

            protected Logger Log => _fsm.Log;

            public override string ToString()
            {
                return $"[{GetType().Name}]";
            }

            public void OnEnter(State prev)
            {
                Log.V("Entering state: {0}", this);
                if (IsUnsubscribed) 
                {
                    _fsm.NotifyEnterUnsubscribed();
                }
                else if (IsSubscribing)
                {
                    _fsm.NotifyEnterSubscribing(_request);
                }
                else if (IsSubscribed) 
                {
                    _fsm.NotifyEnterSubscribed();
                }
                else if (IsUnsubscribing) 
                {
                    _fsm.NotifyEnterUnsubscribing();
                }
                else if (IsFailed) 
                {
                    _fsm.NotifyEnterFailed();
                }
            }

            public void OnLeave()
            {
                Log.V("Leaving state: {0}", this);
                if (IsUnsubscribed) 
                {
                    _fsm.NotifyLeaveUnsubscribed();
                }
                else if (IsSubscribing)
                {
                    _fsm.NotifyLeaveSubscribing();
                }
                else if (IsSubscribed) 
                {
                    _fsm.NotifyLeaveSubscribed();
                }
                else if (IsUnsubscribing) 
                {
                    _fsm.NotifyLeaveUnsubscribing();
                }
                else if (IsFailed) 
                {
                    _fsm.NotifyLeaveFailed();
                }
            }

            public ISuccessfulAwaiter<State> Process()
            {
                Log.V("Processing state: {0}", this);
   
                if (IsUnsubscribed) 
                {
                    if (_fsm.MarkAsDeleted)
                    {
                        Log.V("Exit FSM because subscription is removed, state: {0}", this);
                        _awaiter.Succeed(null);
                    } 
                    else 
                    {
                        var con = _fsm._client.GetState().GetConnection();
                        if (con != null)
                        {
                            _awaiter.Succeed(new Subscribing(this, con));
                        }
                    }
                } 
                else if (IsSubscribing)
                {
                    ProcessSubscribingState();
                } 
                else if (IsSubscribed) 
                {
                    if (_fsm.MarkAsDeleted)
                    {
                        _awaiter.Succeed(new Unsubscribing(this));
                    }
                }
                else if (IsUnsubscribing)
                {
                    _connection.RtmUnsubscribe(_fsm.SubscriptionId).ContinueWith(
                        t => _fsm._client.Post(() => ProcessUnsubscribeResult(t)),
                        TaskContinuationOptions.ExecuteSynchronously);
                }

                return _awaiter;
            }

            public ISuccessfulAwaiter<State> Yield()
            {
                return _fsm.Yield();
            }

            public bool ProcessMarkedAsDeleted()
            {
                Log.V("Processing mark as deleted, state: {0}", this);
                if (IsUnsubscribed) 
                {
                    return _awaiter.Succeed(null);
                } 
                else if (IsSubscribed)
                {
                    var next = new Unsubscribing(this);
                    if (_awaiter.Succeed(next))
                    {
                        return true;
                    }

                    return false;
                } 
                else if (IsFailed) 
                {
                    return _awaiter.Succeed(new Unsubscribed(_fsm));
                }

                return false;
            }

            public void ProcessConnected(IConnection connection)
            {
                Log.V("Processing connected, state: {0}", this);
                if (IsUnsubscribed)
                {
                    if (!_fsm.MarkAsDeleted)
                    {
                        var next = new Subscribing(this, connection);
                        _awaiter.Succeed(next);
                    }
                }
            }

            public void ProcessDisconnected()
            {
                Log.V("Processing disconnected, state: {0}", this);
                if (!_awaiter.IsCompleted)
                {
                    var next = new Unsubscribed(_fsm);
                    _awaiter.Succeed(next);
                }
            }

            public State ProcessSubscriptionData(RtmSubscriptionData data)
            {
                Log.V("Processing subscription data, state: {0}, data: {1}", this, data);
                if (IsSubscribed)
                {
                    if (_awaiter.IsCompleted)
                    {
                        Log.V("Subscription data ignored because state is completed, state: {0}, data: {1}", this, data);
                        return null;
                    }

                    if (_fsm.IsTrackPosition) 
                    {
                        _fsm.Position = data.Position;
                    }

                    return this;
                } 

                Log.V("Subscription data ignored because state is not subscribed state, state: {0}, data: {1}", this, data);
                return null;
            }

            public State ProcessSubscriptionInfo(RtmSubscriptionInfo info)
            {
                Log.V("Processing subscription info, state: {0}, info: {1}", this, info);
                if (IsSubscribed)
                {
                    if (_awaiter.IsCompleted)
                    {
                        Log.V("Subscription info ignored because state is completed, state: {0}, info: {1}", this, info);
                        return null;
                    }

                    if (_fsm.IsTrackPosition && !string.IsNullOrEmpty(info.Position))
                    {
                        _fsm.Position = info.Position;
                    }

                    return this;
                }

                Log.V("Subscription info ignored because state is not subscribed state, state: {0}, info: {1}", this, info);
                return null;
            }

            public virtual State ProcessSubscriptionError(RtmSubscriptionError error)
            {
                Log.V("Processing subscription error, state: {0}, error: {1}", this, error);

                if (_awaiter.IsCompleted)
                {
                    Log.V("Subscription error ignored because state is completed, state: {0}, error: {1}", this, error);
                    return null;
                }

                if (IsSubscribed)
                {
                    if (_fsm.IsTrackPosition && !string.IsNullOrEmpty(error.Position))
                    {
                        _fsm.Position = error.Position;
                    }

                    _awaiter.Succeed(new Failed(this));
                    return _awaiter.Result;
                } 
                else if (IsUnsubscribing)
                {
                    if (_fsm.IsTrackPosition && !string.IsNullOrEmpty(error.Position))
                    {
                        _fsm.Position = error.Position;
                    }

                    _awaiter.Succeed(new Unsubscribed(_fsm));
                    return _awaiter.Result;
                }

                Log.V("Subscription error ignored because state is not subscribed or unsubscribing state, state: {0}, error: {1}", this, error);
                return null;
            }

            private async void ProcessSubscribingState() 
            {
                try
                {
                    RtmSubscribeReply reply;
                    try 
                    {
                        reply = await _connection.RtmSubscribe(_request).ConfigureAwait(false);
                    } 
                    finally 
                    {
                        await Yield();
                    }

                    if (_awaiter.IsCompleted)
                    {
                        Log.V("Subscribe reply is ignored because state is completed, state: {0}", this);
                        return;
                    }

                    _fsm.Position = _fsm.IsTrackPosition ? reply.Position : null;
                    _fsm.History = null;
                    _awaiter.Succeed(new Subscribed(this));
                }
                catch (DisconnectedException) 
                {
                    Log.V("Subscribe operation failed because client is disconnected, state: {0}", this);
                    _awaiter.Succeed(new Unsubscribed(this));
                }
                catch (Exception exn)
                {
                    Log.E(exn, "Subscribe operation failed, state: {0}", this);
                    _fsm.NotifySubscribeError(_fsm, TaskHelper.Unwrap(exn));
                    _awaiter.Succeed(new Failed(this));
                }
            }

            private void ProcessUnsubscribeResult(Task<RtmUnsubscribeReply> t)
            {
                Log.V("Processing unsubscribe result, state: {0}", this);
                if (_awaiter.IsCompleted)
                {
                    Log.V("Unsubscribe result ignored because state is completed, state: {0}", this);
                    return;
                }

                if (t.Status == TaskStatus.RanToCompletion)
                {
                    var reply = t.Result;
                    if (_fsm.IsTrackPosition && reply?.Position != null)
                    {
                        _fsm.Position = reply.Position;
                    }

                    _awaiter.Succeed(new Unsubscribed(_fsm));
                }
                else
                {
                    Log.W("Unsubscribe error is not recoverable. Connection will be reopened");
                    if (t.Exception != null)
                    {
                        _fsm.NotifyUnsubscribeError(_fsm, TaskHelper.Unwrap(t.Exception));
                    }

                    _connection.Close();
                }
            }

            public class Unsubscribed : State
            {
                public Unsubscribed(Subscription fsm) : base(fsm, null)
                {
                }

                public Unsubscribed(State prev) : base(prev._fsm, prev._connection)
                {
                }
            }

            public class Subscribing : State
            {
                public Subscribing(State prev, IConnection connection) : base(prev._fsm, connection)
                {
                }
            }

            public class Subscribed : State
            {
                public Subscribed(State prev) : base(prev._fsm, prev._connection)
                {
                }
            }

            public class Unsubscribing : State
            {
                public Unsubscribing(State prev) : base(prev._fsm, prev._connection)
                {
                }
            }

            public class Failed : State
            {
                public Failed(State prev) : base(prev._fsm, null)
                {
                }
            }
        }
    }
}