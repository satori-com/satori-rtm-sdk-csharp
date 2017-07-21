#pragma warning disable 1591

namespace Satori.Common
{
    public static class QueueAsyncExtensions
    {
        public static T TryDequeue<T>(this QueueAsync<T> queue)
        {
            T res;
            queue.TryDequeue(out res);
            return res;
        }
    }
}
