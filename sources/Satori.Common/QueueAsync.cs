#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Satori.Common
{
    public partial class QueueAsync<T> : IObservableSink<T>
    {
        void IObservableSink<T>.Next(T val)
        {
            Enqueue(val);
        }
    }

    public partial class QueueAsync<T> : IDisposable
    {
        private readonly object _sync = new object();

        private Queue<T> _queue = new Queue<T>();
        private Queue<TaskCompletionSource<T>> _awaiters = new Queue<TaskCompletionSource<T>>();

        public bool IsDisposed => _queue == null;

        public Task<T> Dequeue()
        {
            lock (_sync)
            {
                if (_queue == null)
                {
                    throw new OperationCanceledException();
                }

                if (_queue.Count == 0)
                {
                    var tcs = new TaskCompletionSource<T>();
                    _awaiters.Enqueue(tcs);
                    return tcs.Task;
                }

                return Task.FromResult(_queue.Dequeue());
            }
        }

        /// <summary>
        /// try to immediately get item out of the queue if any
        /// </summary>
        /// <param name="res">hold dequeued item if succeeded, or default value otherwise</param>
        /// <returns>true if item was retrieved, false otherwise</returns>
        public bool TryDequeue(out T res)
        {
            lock (_sync)
            {
                if (_queue == null || _queue.Count == 0)
                {
                    res = default(T);
                    return false;
                }

                res = _queue.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// put item to queue
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <exception cref="OperationCanceledException">if queue was disposed</exception>
        /// <returns>true if enqueued</returns>
        public bool Enqueue(T item)
        {
            TaskCompletionSource<T> tcs;
            lock (_sync)
            {
                if (_queue == null)
                {
                    throw new OperationCanceledException();
                }

                if (_awaiters.Count == 0)
                {
                    _queue.Enqueue(item);
                    return true;
                }

                tcs = _awaiters.Dequeue();
            }

            return tcs.TrySetResult(item);
        }

        /// <summary>
        /// Dispose queue, it will cancel all awaiters if any, 
        /// all further attempts to enqueue or dequeue will fail 
        /// with OperationCanceledException exception
        /// </summary>
        /// <returns>items that were not dequeued</returns>
        public Queue<T> Cancel()
        {
            Queue<TaskCompletionSource<T>> toCancel;
            Queue<T> unhandled;
            lock (_sync)
            {
                toCancel = _awaiters;
                unhandled = _queue;
                _awaiters = null;
                _queue = null;
            }

            if (toCancel != null)
            {
                while (toCancel.Count > 0)
                {
                    if (!toCancel.Dequeue().TrySetCanceled())
                    {
                        Debug.Fail("Cannot cancel some awaiters");
                    }
                }
            }

            return unhandled;
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}
