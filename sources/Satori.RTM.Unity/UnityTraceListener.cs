/**
This script is for adding to Unity projects scripts to hookup tracing to Unity debug log
Integration example:
		public static class Logger {
			public static TraceSource trace = new TraceSource("mz.demos.unity");
			static Logger(){
				AddLogListener (trace, SourceLevels.All);
				AddLogListener (RtmClient.trace, SourceLevels.All); 
				AddLogListener (RtmService.trace, SourceLevels.All);
				AddLogListener (RtSerialization.defaultTrace, SourceLevels.All);
				AddLogListener (RtWebSocket.defaultTrace, SourceLevels.All);
			}
			
			static void AddLogListener(TraceSource source, SourceLevels level) {
				try {
					source.Switch.Level = level;
					source.Listeners.Add(UnityTraceListener.instance);
				} catch (Exception exn) {
					UnityEngine.Debug.LogException(exn);
				}
			}
		}
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Satori.Rtm 
{
    public class UnityTraceListener : TraceListener {

    	public static readonly UnityTraceListener Instance = new UnityTraceListener();
    	 
    	public override void Write(string message) {
    		Debug.Log("UnityTraceListener.Write");
    	}

    	public override void WriteLine(string message) {
    		Debug.Log("UnityTraceListener.WriteLine");
    	}

    	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
    		Debug.Log("UnityTraceListener.TraceData");
    	}

    	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data) {
    		Debug.Log("UnityTraceListener.TraceData");
    	}

    	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id) {
    		TraceEvent(eventCache, source, eventType, id, "");
    	}

    	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message) {
    		var txt = source +  "[" + Thread.CurrentThread.ManagedThreadId + "]: " + message;
            if (eventType < TraceEventType.Warning) {
    			Debug.LogError(txt);
    			return;
            }

    		if (eventType < TraceEventType.Information) {
    			Debug.LogWarning(txt);
    			return;
    		}

    		Debug.Log(txt);
    	}
    	public override void Fail(string message, string detailMessage) {
    		Debug.Log("UnityTraceListener.Fail");
    	}
    	public override void Fail(string message) {
    		Debug.Log("UnityTraceListener.Fail");
    	}
    }
}