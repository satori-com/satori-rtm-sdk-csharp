using System.Diagnostics;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;

namespace Satori.Rtm
{
    public class UnityTraceListener : TraceListener
    {
    	public static readonly UnityTraceListener Instance = new UnityTraceListener();
    	 
    	public override void Write(string message)
        {
    		Debug.Log(message);
    	}

    	public override void WriteLine(string message)
        {
            Debug.LogFormat("{0}\n", message);
    	}

    	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
    		Debug.LogWarning("UnityTraceListener.TraceData method not supported");
    	}

    	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            Debug.LogWarning("UnityTraceListener.TraceData method not supported");
        }

    	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
    		TraceEvent(eventCache, source, eventType, id, "");
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
    			Debug.LogWarning(txt); // Unity Editor Tests treat error messages as test failure
    			return;
            }

    		if (eventType < TraceEventType.Information)
            {
    			Debug.LogWarning(txt);
    			return;
    		}

    		Debug.Log(txt);
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
            Debug.LogWarning(sb.ToString()); // Unity Editor Tests treat error messages as test failure
        }

    	public override void Fail(string message)
        {
            Debug.LogWarning(message); // Unity Editor Tests treat error messages as test failure
        }
    }
}