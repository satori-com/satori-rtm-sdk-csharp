#pragma warning disable 1591

using System;
using System.Threading;
using Satori.Common;

namespace Satori.Rtm
{
    public partial class SingleThreadDispatcher : IDispatcher
    {
        public readonly int ThreadId = Thread.CurrentThread.ManagedThreadId;

        private readonly Dispatcher dispatcher = new Dispatcher();

        public void Post(Action act)
        {
            if (Thread.CurrentThread.ManagedThreadId == ThreadId)
            {
                dispatcher.Post(act);
            }
            else
            {
                dispatcher.Enqueue(act);
            }
        }

        public void ProcessQueue()
        {
            if (Thread.CurrentThread.ManagedThreadId != ThreadId)
            {
                throw new InvalidOperationException("Queue must be processed on the original thread #" + ThreadId);
            }

            dispatcher.ProcessQueue();
        }

        public ISuccessfulAwaiter<bool> Yield()
        {
            return this;
        }
    }

    public partial class SingleThreadDispatcher : ISuccessfulAwaiter<bool>
    {
        public bool IsCompleted => false;

        public bool GetResult() => true;

        public void OnCompleted(Action continuation)
        {
            Post(continuation);
        }
    }
}
