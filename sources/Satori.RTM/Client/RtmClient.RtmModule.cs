#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Satori.Common;

namespace Satori.Rtm.Client
{
    internal partial class RtmClient
    {
        public partial class RtmModule : IDispatchObject
        {
            public IDispatcher Dispatcher => _client.Dispatcher;

            public async void CreateSubscription(
                string channelOrSubId,
                SubscriptionConfig config)
            {
                Log.I("CreateSubscription method is dispatched, subscription id: '{0}'", channelOrSubId);
                await this.Yield();
                Log.V("CreateSubscription method is executing, subscription id: '{0}'", channelOrSubId);
                
                var observer = config.Observer;
                if (observer == null)
                {
                    observer = _subscriptionObserver;
                }
                else
                {
                    observer = new SubscriptionCompoundObserver(
                        _subscriptionObserver, 
                        observer);
                }

                ISubscription createdSub;
                Subscription storedSub;
                if (_susbscriptions.TryGetValue(channelOrSubId, out storedSub))
                {
                    createdSub = storedSub.ProcessSubscribe(config, observer);
                    if (createdSub == null)
                    {
                        _client.NotifyError(new InvalidOperationException($"Subscription '{channelOrSubId}' already exists"));
                        return;
                    }
                }
                else
                {
                    var sub = new Subscription(
                        _client, 
                        channelOrSubId, 
                        config.Mode, 
                        config.Filter, 
                        config.Period, 
                        config.Position, 
                        config.History, 
                        observer);
                    observer.NotifyCreated(sub);
                    _susbscriptions[channelOrSubId] = sub;
                    observer.NotifyEnterUnsubscribed(sub);
                    if (sub != null)
                    {
                        // subscription loop should be started after method is returned
                        this.Post(() => this.SubscriptionLoop(sub));
                    }

                    createdSub = sub;
                }

                Log.V("CreateSubscription method is completed, subscription id: '{0}'", channelOrSubId);
            }

            public async void RemoveSubscription(string subscriptionId)
            {
                Log.I("RemoveSubscription method is dispatched, subscription id: '{0}'", subscriptionId);
                await this.Yield();
                Log.V("RemoveSubscription method is executing, subscription id: '{0}'", subscriptionId);

                ISubscription removedSub;
                Subscription susbscription;
                if (!_susbscriptions.TryGetValue(subscriptionId, out susbscription))
                {
                    removedSub = null;
                }
                else
                {
                    removedSub = susbscription.ProcessUnsubscribe();
                }

                if (removedSub == null)
                {
                    _client.NotifyError(new InvalidOperationException($"Subscription '{subscriptionId}' doesn't exist"));
                    return;
                }

                Log.V("RemoveSubscription method is completed, subscription id: '{0}'", subscriptionId);
            }

            public async Task<ISubscription> GetSubscription(string subscriptionId)
            {
                Log.V("GetSubscription method is dispatched, subscription id: '{0}'", subscriptionId);
                await this.Yield();
                Log.V("GetSubscription method is executing, subscription id: '{0}'", subscriptionId);

                Subscription subs;
                if (_susbscriptions == null || !_susbscriptions.TryGetValue(subscriptionId, out subs))
                {
                    throw new InvalidOperationException($"Subscription '{subscriptionId}' doesn't exist");
                }
                else if (!subs.MarkAsDeleted)
                {
                    return subs;
                }
                else
                {
                    return subs.Future;
                }
            }

            public async Task<RtmPublishReply> Publish<T>(string channel, T message, Ack ack)
            {
                Log.I("Publish to channel '{0}', ack: {1}, message: '{2}'", channel, ack, message);

                var conn = await _client.GetConnection();
                Log.V("Publish to channel '{0}' executing, message: '{1}'", channel, message);

                if (conn == null)
                {
                    Log.W("Publish to channel '{0}' ignored because connection is null, message: '{1}'", channel, message);
                    return null;
                }

                var reply = await conn.RtmPublish(channel, message, ack).ConfigureAwait(false);

                Log.V("Publish to channel '{0}' completed, message: '{1}', reply: '{2}'", channel, message, reply);
                return reply;
            }

            public Task<RtmReadReply<T>> Read<T>(string channel)
            {
                var request = new RtmReadRequest
                {
                    Channel = channel
                };
                return Read<T>(request);
            }

            public async Task<RtmReadReply<T>> Read<T>(RtmReadRequest request)
            {
                Log.I("Read from channel, request: '{0}'", request);

                var conn = await _client.GetConnection();
                Log.V("Read from channel executing, request: '{0}'", request);

                if (conn == null)
                {
                    Log.W("Read from channel ignored because connection is null, request: '{0}'", request);
                    return null;
                }

                var reply = await conn.RtmRead<T>(request).ConfigureAwait(false);

                Log.V("Read from channel completed, request: '{0}', reply: '{1}'", request, reply);
                return reply;
            }

            public Task<RtmWriteReply> Write<T>(string channel, T message, Ack ack)
            {
                var request = new RtmWriteRequest<T>
                {
                    Channel = channel,
                    Message = message
                };

                return Write(request, ack);
            }

            public async Task<RtmWriteReply> Write<T>(RtmWriteRequest<T> request, Ack ack)
            {
                Log.I("Write to channel, request: '{0}', ack: {1}", request, ack);

                var conn = await _client.GetConnection();
                Log.V("Write to channel executing, request: '{0}'", request);

                if (conn == null)
                {
                    Log.W("Write to channel ignored because connection is null, request: '{0}'", request);
                    return null;
                }

                var reply = await conn.RtmWrite(request, ack).ConfigureAwait(false);

                Log.V("Write to channel completed, request: '{0}', reply: '{1}'", request, reply);
                return reply;
            }

            public async Task<RtmDeleteReply> Delete(string channel, Ack ack)
            {
                var request = new RtmDeleteRequest
                {
                    Channel = channel
                };

                Log.I("Delete from channel, request: '{0}', ack: {1}", request, ack);

                var conn = await _client.GetConnection();
                Log.V("Delete from channel, request: '{0}' executing", request);

                if (conn == null)
                {
                    Log.W("Delete from channel ignored because connection is null, request: '{0}'", request);
                    return null;
                }

                var reply = await conn.RtmDelete(request, ack).ConfigureAwait(false);

                Log.V("Delete from channel completed, request: '{0}', reply: '{1}'", request, reply);
                return reply;
            }
        }

        public partial class RtmModule
        {
            private readonly RtmClient _client;
            private Dictionary<string, Subscription> _susbscriptions;
            private SubscriptionObserver _subscriptionObserver;

            public RtmModule(RtmClient client)
            {
                _client = client;
                _susbscriptions = new Dictionary<string, Subscription>();
                _subscriptionObserver = new SubscriptionObserver();
                client.OnEnterConnected += OnClientEnterConnected;
                client.OnLeaveConnected += OnClientLeaveConnected;
                client.OnUnsolicitedEvent += OnClientUnsolicitedEvent;
            }

            public void Dispose()
            {
                Log.V("RtmModule disposing");
                if (_client != null)
                {
                    _client.OnEnterConnected -= OnClientEnterConnected;
                    _client.OnLeaveConnected -= OnClientLeaveConnected;
                    _client.OnUnsolicitedEvent -= OnClientUnsolicitedEvent;
                }

                var susbscriptions = _susbscriptions;
                _susbscriptions = null;

                foreach (var s in susbscriptions.Values)
                {
                    s.ProcessUnsubscribe();
                }

                _subscriptionObserver = null;
                Log.V("RtmModule disposed");
            }

            public async void SubscriptionLoop(Subscription subscription)
            {
                Log.V("Starting subscription loop for '{0}'", subscription);

                while (true)
                {
                    var future = await subscription.Process();
                    if (future == null)
                    {
                        break;
                    }
                    else
                    {
                        subscription = new Subscription(
                            _client, 
                            subscription.SubscriptionId, 
                            future.Mode, 
                            future.Filter, 
                            future.Period, 
                            future.Position, 
                            future.History, 
                            future.Observer);
                        _susbscriptions[subscription.SubscriptionId] = subscription;
                        Log.V("Subscription is replaced: '{0}'", subscription);
                    }
                }

                if (_susbscriptions != null)
                {
                    _susbscriptions.Remove(subscription.SubscriptionId);
                }

                Log.V("Finished subscription loop for '{0}'", subscription);
            }

            private void OnClientLeaveConnected(IConnection con)
            {
                Log.V("Handling OnLeaveConnected event");
                var subs = new Subscription[_susbscriptions.Count];
                _susbscriptions.Values.CopyTo(subs, 0);
                foreach (var s in subs)
                {
                    s.ProcessDisconnected();
                }
            }

            private void OnClientEnterConnected(IConnection con)
            {
                Log.V("Handling OnEnterConnected event");
                foreach (var s in _susbscriptions.Values)
                {
                    s.ProcessConnected(con);
                }
            }

            private void OnClientUnsolicitedEvent(Pdu pdu)
            {
                Log.V("Handling OnUnsolicitedEvent event, pdu: '{0}'", pdu);

                if (pdu.Action == RtmActions.SubscriptionData)
                {
                    ProcessSubscriptionData(
                        pdu.Body.ToObject<RtmSubscriptionData>());
                }
                else if (pdu.Action == RtmActions.SubscriptionInfo)
                {
                    ProcessSubscriptionInfo(
                        pdu.Body.ToObject<RtmSubscriptionInfo>());
                }
                else if (pdu.Action == RtmActions.SubscriptionError)
                {
                    ProcessSubscriptionError(
                        pdu.Body.ToObject<RtmSubscriptionError>());
                }
            }

            private void ProcessSubscriptionError(RtmSubscriptionError error)
            {
                Log.V("Processing subscription error: '{0}'", error);

                if (error == null)
                {
                    Log.E("Subscription error is null");
                    return;
                }

                Subscription susbscription;
                if (!_susbscriptions.TryGetValue(error.SubscriptionId, out susbscription))
                {
                    throw new InvalidOperationException($"Subscription not found, error: '{error}'");
                }

                susbscription.ProcessSubscriptionError(error);
            }

            private void ProcessSubscriptionData(RtmSubscriptionData data)
            {
                Log.V("Processing subscription data: '{0}'", data);

                if (data == null)
                {
                    Log.E("Subscription data is null");
                    return;
                }

                Subscription susbscription;
                if (!_susbscriptions.TryGetValue(data.SubscriptionId, out susbscription))
                {
                    throw new InvalidOperationException($"Subscription not found, data: '{data}'");
                }

                susbscription.ProcessSubscriptionData(data);
            }

            private void ProcessSubscriptionInfo(RtmSubscriptionInfo info)
            {
                Log.V("Processing subscription info: '{0}'", info);

                if (info == null)
                {
                    Log.E("Subscription info is null");
                    return;
                }

                Subscription susbscription;
                if (!_susbscriptions.TryGetValue(info.SubscriptionId, out susbscription))
                {
                    throw new InvalidOperationException($"Subscription not found, info: '{info}'");
                }

                susbscription.ProcessSubscriptionInfo(info);
            }
        }
    }
}