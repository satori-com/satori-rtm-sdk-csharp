#pragma warning disable 1591

using System;
using System.Threading;

namespace Satori.Common
{
    public static class AwaiterExtensions
    {
        public static ISuccessfulAwaiter<O> Map<I, O>(this ISuccessfulAwaiter<I> awaiter, Func<I, O> transform)
        {
            return new SuccessfulAwaiterTransform<I, O>(awaiter, transform);
        }

        public static ISuccessfulAwaiter<T> GetAwaiter<T>(this ISuccessfulAwaiter<T> awaiter)
        {
            return awaiter;
        }

        public static ISuccessfulAwaiter GetAwaiter(this ISuccessfulAwaiter awaiter)
        {
            return awaiter;
        }

        public static IAwaiter GetAwaiter(this IAwaiter awaiter)
        {
            return awaiter;
        }

        public static IAwaiter<T> GetAwaiter<T>(this IAwaiter<T> awaiter)
        {
            return awaiter;
        }

        public static void Wait(this ISuccessfulAwaiter awaiter)
        {
            if (!awaiter.IsCompleted)
            {
                var evt = new ManualResetEventSlim();
                awaiter.OnCompleted(evt.Set);
                evt.Wait();
            }

            awaiter.GetResult();
            return;
        }

        public static T Wait<T>(this ISuccessfulAwaiter<T> awaiter)
        {
            if (!awaiter.IsCompleted)
            {
                var evt = new ManualResetEventSlim();
                awaiter.OnCompleted(evt.Set);
                evt.Wait();
            }

            return awaiter.GetResult();
        }
    }
}
