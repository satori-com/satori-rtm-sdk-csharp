#pragma warning disable 1591

using System;
using Satori.Common;

namespace Satori.Rtm
{
    public interface IDispatcher
    {
        void Post(Action act);
        
        ISuccessfulAwaiter<bool> Yield();
    }

    public interface IDispatchObject
    {
        IDispatcher Dispatcher { get; }
    }

    public static class DispatchObjectExtensions
    {
        public static void Post(this IDispatchObject dispObj, Action act)
        {
            dispObj.Dispatcher.Post(act);
        }

        public static ISuccessfulAwaiter<bool> Yield(this IDispatchObject dispObj)
        {
            return dispObj.Dispatcher.Yield();
        }
    }
}