using System;
using System.Threading.Tasks;

namespace Satori.Common
{
    internal static class TaskExtensions
    {
        public static void ContinueSyncWith<T>(this Task<T> task, Action<T> onSuccess, Action<Exception> onFailure)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    onFailure(t.Exception);
                }
                else
                {
                    onSuccess(t.Result);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
