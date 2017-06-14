#pragma warning disable 1591

using Satori.Common;
using System;
using System.Threading.Tasks;

namespace Satori.Rtm
{
    public static class TaskExtensions
    {
        public static void ContinueOnDispatcher<T>(this Task<T> task, IDispatcher dispatcher, Action<T> onSuccess, Action<Exception> onFailure)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    var exn = TaskHelper.Unwrap(t.Exception);
                    dispatcher.Post(() => onFailure(exn));
                }
                else
                {
                    dispatcher.Post(() => onSuccess(t.Result));
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

    }
}
