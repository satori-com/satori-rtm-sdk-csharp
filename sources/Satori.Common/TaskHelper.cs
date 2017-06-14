using System;
using System.Threading.Tasks;

namespace Satori.Common
{
    internal static class TaskHelper
    {
        public static Exception Unwrap(AggregateException exn)
        {
            if (exn.InnerException != null && (exn.InnerExceptions == null || exn.InnerExceptions.Count <= 1))
            {
                return Unwrap(exn.InnerException);
            }

            if (exn.InnerExceptions != null && exn.InnerExceptions.Count == 1)
            {
                return Unwrap(exn.InnerExceptions[0]);
            }
            else
            {
                return exn;
            }
        }

        public static Exception Unwrap(Exception exn)
        {
            if (exn is AggregateException)
            {
                return Unwrap((AggregateException)exn);
            }
            else
            {
                return exn;
            }
        }
    }
}
