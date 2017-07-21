#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Satori.Common
{
    public class MonitorAsync : IDisposable
    {
        private bool _entered;
        private Queue<Rec> _waiting = new Queue<Rec>();

        public MonitorAsync()
        {
        }

        public MonitorAsync(bool entered)
        {
            _entered = entered;
        }

        private object Sync => _waiting;

        public Task<IDisposable> Enter()
        {
            TaskCompletionSource<IDisposable> tcs;
            lock (Sync)
            {
                if (!_entered)
                {
                    _entered = true;
                    return Task.FromResult<IDisposable>(this);
                }

                tcs = new TaskCompletionSource<IDisposable>();
                _waiting.Enqueue(new Rec { Tcs = tcs, Ct = CancellationToken.None });
            }

            return tcs.Task;
        }

        public Task<bool> TryEnter(CancellationToken ct)
        {
            TaskCompletionSource<IDisposable> tcs;
            if (ct.IsCancellationRequested)
            {
                return Task.FromResult(false);
            }

            lock (Sync)
            {
                if (!_entered)
                {
                    _entered = true;
                    return Task.FromResult(true);
                }

                tcs = new TaskCompletionSource<IDisposable>();
                _waiting.Enqueue(new Rec { Tcs = tcs, Ct = ct });
            }

            return tcs.Task.ContinueWith(
                t => (t.Status == TaskStatus.RanToCompletion && t.Result != null),
                TaskContinuationOptions.ExecuteSynchronously);
        }

        public void Leave()
        {
            Rec rec;
            while (true)
            {
                lock (Sync)
                {
                    if (!_entered)
                    {
                        throw new Exception("MonitorAsync unbalanced leave call");
                    }

                    if (_waiting.Count == 0)
                    {
                        _entered = false;
                        return;
                    }

                    rec = _waiting.Dequeue();
                }

                if (!rec.Ct.IsCancellationRequested)
                {
                    if (rec.Tcs.TrySetResult(this))
                    {
                        return;
                    }
                }
                else
                {
                    rec.Tcs.TrySetResult(null);
                }
            }
        }

        public void Dispose()
        {
            Leave();
        }

        private struct Rec
        {
            public TaskCompletionSource<IDisposable> Tcs { get; set; }

            public CancellationToken Ct { get; set; }
        }
    }
}
