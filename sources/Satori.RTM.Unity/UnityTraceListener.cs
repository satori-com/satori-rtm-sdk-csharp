using System.Diagnostics;
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
    		var txt = $"{source} [tid={Thread.CurrentThread.ManagedThreadId}]: {message}";
            if (eventType < TraceEventType.Warning)
            {
    			Debug.LogError(txt);
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
            Debug.LogError($"{message}\n{detailMessage}");
    	}

    	public override void Fail(string message)
        {
            Debug.LogError(message);
    	}
    }
}