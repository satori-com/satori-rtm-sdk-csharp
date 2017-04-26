#pragma warning disable 1591

using System;
using System.Diagnostics;

namespace Satori.Rtm
{
    public static class LoggerExtensions
    {
        #region Errors
        
        public static bool E(this Logger log)
        {
            return log != null && log.ShouldTrace(Logger.LogLevel.Error);
        }

        [Conditional("TRACE")]
        public static void E(this Logger log, Exception exception, string message)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, exception, message);
            }
        }

        [Conditional("TRACE")]
        public static void E<T>(this Logger log, Exception exception, string format, T arg)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, exception, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void E<T1, T2>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, exception, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void E<T1, T2, T3>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, exception, string.Format(format, arg1, arg2, arg3));
            }
        }

        [Conditional("TRACE")]
        public static void E(this Logger log, string message)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, null, message);
            }
        }

        [Conditional("TRACE")]
        public static void E<T>(this Logger log, string format, T arg)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, null, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void E<T1, T2>(this Logger log, string format, T1 arg1, T2 arg2)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, null, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void E<T1, T2, T3>(this Logger log, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.E())
            {
                log.LogEvent(Logger.LogLevel.Error, null, string.Format(format, arg1, arg2, arg3));
            }
        }

        #endregion

        #region Warnings

        public static bool W(this Logger log)
        {
            return log != null && log.ShouldTrace(Logger.LogLevel.Warning);
        }

        [Conditional("TRACE")]
        public static void W(this Logger log, Exception exception, string message)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, exception, message);
            }
        }

        [Conditional("TRACE")]
        public static void W<T>(this Logger log, Exception exception, string format, T arg)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, exception, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void W<T1, T2>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, exception, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void W<T1, T2, T3>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, exception, string.Format(format, arg1, arg2, arg3));
            }
        }

        [Conditional("TRACE")]
        public static void W(this Logger log, string message)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, null, message);
            }
        }

        [Conditional("TRACE")]
        public static void W<T>(this Logger log, string format, T arg)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, null, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void W<T1, T2>(this Logger log, string format, T1 arg1, T2 arg2)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, null, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void W<T1, T2, T3>(this Logger log, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.W())
            {
                log.LogEvent(Logger.LogLevel.Warning, null, string.Format(format, arg1, arg2, arg3));
            }
        }

        #endregion

        #region Info

        public static bool I(this Logger log)
        {
            return log != null && log.ShouldTrace(Logger.LogLevel.Info);
        }

        [Conditional("TRACE")]
        public static void I(this Logger log, Exception exception, string message)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, exception, message);
            }
        }

        [Conditional("TRACE")]
        public static void I<T>(this Logger log, Exception exception, string format, T arg)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, exception, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void I<T1, T2>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, exception, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void I<T1, T2, T3>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, exception, string.Format(format, arg1, arg2, arg3));
            }
        }

        [Conditional("TRACE")]
        public static void I(this Logger log, string message)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, null, message);
            }
        }

        [Conditional("TRACE")]
        public static void I<T>(this Logger log, string format, T arg)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, null, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void I<T1, T2>(this Logger log, string format, T1 arg1, T2 arg2)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, null, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void I<T1, T2, T3>(this Logger log, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.I())
            {
                log.LogEvent(Logger.LogLevel.Info, null, string.Format(format, arg1, arg2, arg3));
            }
        }

        #endregion

        #region Verbose

        public static bool V(this Logger log)
        {
            return log != null && log.ShouldTrace(Logger.LogLevel.Verbose);
        }

        [Conditional("TRACE")]
        public static void V(this Logger log, Exception exception, string message)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, exception, message);
            }
        }

        [Conditional("TRACE")]
        public static void V<T>(this Logger log, Exception exception, string format, T arg)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, exception, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void V<T1, T2>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, exception, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void V<T1, T2, T3>(this Logger log, Exception exception, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, exception, string.Format(format, arg1, arg2, arg3));
            }
        }

        [Conditional("TRACE")]
        public static void V(this Logger log, string message)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, null, message);
            }
        }

        [Conditional("TRACE")]
        public static void V<T>(this Logger log, string format, T arg)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, null, string.Format(format, arg));
            }
        }

        [Conditional("TRACE")]
        public static void V<T1, T2>(this Logger log, string format, T1 arg1, T2 arg2)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, null, string.Format(format, arg1, arg2));
            }
        }

        [Conditional("TRACE")]
        public static void V<T1, T2, T3>(this Logger log, string format, T1 arg1, T2 arg2, T3 arg3)
        {
            if (log.V())
            {
                log.LogEvent(Logger.LogLevel.Verbose, null, string.Format(format, arg1, arg2, arg3));
            }
        }

        #endregion
    }
}