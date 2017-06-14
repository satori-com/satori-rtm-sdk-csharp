using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Satori.Rtm
{
    public class IosTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            Console.WriteLine("IosTraceListener.TraceData method not supported");
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            Console.WriteLine("IosTraceListener.TraceData method not supported");
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEvent(eventCache, source, eventType, id, string.Empty);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(source);
            sb.Append(" [tid=");
            sb.Append(Thread.CurrentThread.ManagedThreadId);
            sb.Append("]: ");
            sb.Append(message);
            var txt = sb.ToString();

            if (eventType < TraceEventType.Warning)
            {
                Console.Error.WriteLine(txt); 
                return;
            }

            Console.WriteLine(txt);
        }

        public override void Fail(string message, string detailMessage)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message);
            if (!string.IsNullOrEmpty(detailMessage))
            {
                sb.AppendLine();
                sb.Append(detailMessage);
            }

            Console.Error.WriteLine(sb.ToString()); 
        }

        public override void Fail(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}