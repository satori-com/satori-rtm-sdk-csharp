#pragma warning disable 1591

using System;
using System.Threading;

namespace Satori.Common
{
    public static class UnhandledExceptionWatcher
    {
        public static event Action<Exception> OnError;

        public static void Swallow(Exception exception)
        {
            var handlers = Volatile.Read(ref OnError);
            if (handlers != null)
            {
                handlers(exception);
            }
        }
    }
}
