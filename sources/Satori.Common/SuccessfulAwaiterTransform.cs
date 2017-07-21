#pragma warning disable 1591

using System;

namespace Satori.Common
{
    public class SuccessfulAwaiterTransform<I, O> : ISuccessfulAwaiter<O>
    {
        private readonly Func<I, O> _transform;
        private readonly ISuccessfulAwaiter<I> _awaiter;

        public SuccessfulAwaiterTransform(ISuccessfulAwaiter<I> awaiter, Func<I, O> transform)
        {
            _awaiter = awaiter;
            _transform = transform;
        }

        public bool IsCompleted => _awaiter.IsCompleted;

        public O GetResult()
        {
            return _transform(_awaiter.GetResult());
        }

        public void OnCompleted(Action continuation)
        {
            _awaiter.OnCompleted(continuation);
        }
    }
}
