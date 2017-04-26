#pragma warning disable 1591

using System;
using System.Runtime.CompilerServices;

namespace Satori.Common
{
    public interface IObservableSink<in T>
    {
        void Next(T val);
    }

    public interface ISuccessfulCompletionSink<in TResult>
    {
        bool Succeed(TResult result);
    }

    public interface ICompletionSink<in TResult> : ISuccessfulCompletionSink<TResult>
    {
        bool Fail(Exception error);

        bool Cancel();
    }

    public interface ISuccessfulCompletionSink
    {
        bool Succeed();
    }

    public interface ICompletionSink : ISuccessfulCompletionSink
    {
        bool Fail(Exception error);

        bool Cancel();
    }

    public interface IAwaiter : ISuccessfulAwaiter
    {
        bool IsSucceeded { get; }

        bool IsFailed { get; }
    }

    public interface IAwaiter<out TResult> : ISuccessfulAwaiter<TResult>
    {
        bool IsSucceeded { get; }

        bool IsFailed { get; }
    }

    public interface ISuccessfulAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }

        void GetResult();
    }

    public interface ISuccessfulAwaiter<out TResult> : INotifyCompletion
    {
        bool IsCompleted { get; }

        TResult GetResult();
    }
}
