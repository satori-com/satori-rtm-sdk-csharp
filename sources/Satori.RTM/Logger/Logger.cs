#pragma warning disable 1591

using System;
using System.Diagnostics;
using System.Threading;

namespace Satori.Rtm
{
    public abstract class Logger
    {
        private static int counter = 0;

        public Logger() : this(LogLevel.Warning)
        { 
        }

        public Logger(LogLevel level)
        {
            Level = level;
            Id = Interlocked.Increment(ref counter);
        }

        public enum LogLevel
        {
            Error = 0,
            Warning,
            Info,
            Information,
            Verbose
        }

        public LogLevel Level { get; protected set; }

        public int Id { get; private set; }

        public bool PrintStackTrace { get; set; } = true;

        public virtual void SetLevel(LogLevel level)
        {
            Level = level;
        }

        [Conditional("TRACE")]
        public abstract void LogEvent(LogLevel type, Exception exception, string message);

        public bool ShouldTrace(LogLevel level)
        {
            return level <= Level;
        }
    }
}