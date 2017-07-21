#pragma warning disable 1591

using System;
using System.Diagnostics;
using System.Text;

namespace Satori.Rtm
{
    public class TraceLogger : Logger
    {
        private string _source;

        public TraceLogger(string source)
        {
            _source = source;
        }

        public override void LogEvent(LogLevel level, Exception exception, string message)
        {
            var cache = new TraceEventCache();
            var etype = GetEventType(level);
            if (Trace.Listeners.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(System.DateTime.Now.ToString("O"));
                sb.Append(" ");
                if (message != null)
                {
                    sb.Append(message);
                }

                if (exception != null)
                {
                    sb.AppendLine();
                    if (PrintStackTrace)
                    {
                        sb.AppendLine(exception.ToString());
                    }
                    else
                    {
                        sb.AppendLine(exception.Message);
                    }
                }

                var fullMessage = sb.ToString();
                foreach (TraceListener l in Trace.Listeners)
                {
                    l.TraceEvent(cache, _source, etype, Id, fullMessage);
                }
            }
        }

        private static TraceEventType GetEventType(LogLevel level)
        {
            switch (level)
            {
            case LogLevel.Error:
                return TraceEventType.Error;
            case LogLevel.Warning:
                return TraceEventType.Warning;
            case LogLevel.Info:
                return TraceEventType.Information;
            default:
                return TraceEventType.Verbose;
            }
        }
    }
}