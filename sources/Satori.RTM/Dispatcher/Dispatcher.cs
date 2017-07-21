#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Satori.Common;

namespace Satori.Rtm
{
    public partial class Dispatcher : IDispatcher
    {
        private bool _acquired = false;
        private Queue<Action> _queue = new Queue<Action>();

        public Logger Log { get; } = Client.DefaultLoggers.Dispatcher;

        public void Enqueue(Action act)
        {
            lock (_queue)
            {
                if (_queue.Count > 100)
                {
                    Log.W("Queue is too big: {0}", _queue.Count);
                }

                _queue.Enqueue(act);
            }
        }

        public void Post(Action act)
        {
            lock (_queue)
            {
                if (_acquired)
                {
                    Enqueue(act);
                    return;
                }

                _acquired = true;
            }

            ProcessQueue(act);
        }

        public void ProcessQueue()
        {
            lock (_queue)
            {
                if (_acquired)
                {
                    return;
                }

                _acquired = true;
            }

            ProcessQueue(delegate { });
        }

        public ISuccessfulAwaiter<bool> Yield()
        {
            return this;
        }

        private void ProcessQueue(Action act)
        {
            while (true)
            {
                try
                {
                    act();
                }
                catch (Exception exn)
                {
                    Log.E(exn, "Unhandled exception while executing dispatched action");
                    UnhandledExceptionWatcher.Swallow(exn);
                }

                lock (_queue)
                {
                    if (_queue.Count <= 0)
                    {
                        _acquired = false;
                        return;
                    }

                    act = _queue.Dequeue();
                }
            }
        }
    }

    public partial class Dispatcher : ISuccessfulAwaiter<bool>
    {
        public bool IsCompleted => false;

        public bool GetResult() => true;

        public void OnCompleted(Action continuation)
        {
            Post(continuation);
        }
    }
}