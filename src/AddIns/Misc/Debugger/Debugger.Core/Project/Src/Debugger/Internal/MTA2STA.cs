// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using DebuggerInterop.Core;

namespace DebuggerInterop.Core
{
	class MTA2STA
	{		
		Form hiddenForm;
		IntPtr hiddenFormHandle;
		
		object   targetObject        = null;
		string   functionName        = null;
		Object[] functionParameters  = null;
		
		Thread MTAThread;
		
		static object OnlyOneAtTimeLock = new Object();
		static object DataLock = new Object();
		object returnValue;
		
		public MTA2STA()
		{
			hiddenForm = new Form();
			hiddenFormHandle = hiddenForm.Handle;
		}
		
		void TraceMsg(string msg)
		{
			System.Console.WriteLine("MTA2STA: " + msg);
		}
		
		public object CallInSTA (object targetObject, string functionName, object[] functionParameters)
		{
			lock (OnlyOneAtTimeLock) {
				TraceMsg("call to process: " + functionName + " {");
							
				lock (DataLock) {
					this.targetObject        = targetObject;
					this.functionName        = functionName;
					this.functionParameters  = functionParameters;
				}
				
				MTAThread = Thread.CurrentThread;
				if (hiddenForm.InvokeRequired == true) {
					IAsyncResult async = hiddenForm.BeginInvoke(new EventHandler(PerformCall));
					//while (async.AsyncWaitHandle.WaitOne(1000,true) == false) {
					//	System.Console.WriteLine("Waiting for callback...");
					//}
					if (async.AsyncWaitHandle.WaitOne(1000,true) == false) {
						System.Console.WriteLine("Callback time out! Unleashing debugger thread.");
					}
				} else {
					PerformCall(hiddenForm, EventArgs.Empty);
				}
		
				TraceMsg("} // MTA2STA: call processed: " + functionName);
			}
			return returnValue;
		}

		void PerformCall(object sender, EventArgs e)
		{
			returnValue = Call(targetObject, functionName, functionParameters);
		}

		public object Call (object targetObject, string functionName, object[] functionParameters)
		{
			MethodInfo method;
			object[] outputParams;
			lock (DataLock) {
				object[] inputParams = functionParameters;
				if (targetObject is Type) {
					method = ((Type)targetObject).GetMethod(functionName);
				} else {
					method = targetObject.GetType().GetMethod(functionName);
				}
				ParameterInfo[] outputParamsInfo = method.GetParameters();
				outputParams = null;
				if (outputParamsInfo != null) {
					outputParams = new object[outputParamsInfo.Length];
					for (int i = 0; i < outputParams.Length; i++) {
						if (inputParams[i] == null) {
							outputParams[i] = null;
						} else if (inputParams[i] is IntPtr) {
							if (outputParamsInfo[i].ParameterType == typeof(IntPtr)) {
								outputParams[i] = inputParams[i];							
							} else if ((IntPtr)inputParams[i] == IntPtr.Zero) {
								outputParams[i] = null;
							} else if (outputParamsInfo[i].ParameterType == typeof(string)) {
								outputParams[i] = Marshal.PtrToStringAuto((IntPtr)inputParams[i]);
							} else {
								try{
									outputParams[i] = null;
									outputParams[i] = Marshal.GetTypedObjectForIUnknown((IntPtr)inputParams[i], outputParamsInfo[i].ParameterType);
								} catch (System.Exception exception) {
									System.Diagnostics.Debug.Fail("Marshaling of argument " + i.ToString() + " of " + functionName + " failed.", exception.ToString());
								}
							}
						} else {
							outputParams[i] = inputParams[i];
						}
					}
				}
			}
			TraceMsg ("Invoke " + functionName + "{");
			object returnValue = null;
			try {
				if (targetObject is Type) {
					returnValue = method.Invoke(null, outputParams);
				} else {
					returnValue = method.Invoke(targetObject, outputParams);
				}
			} catch (System.Exception exception) {
				System.Diagnostics.Debug.Fail("Invoke of " + functionName + " failed.", exception.ToString());
			}
			TraceMsg ("} \\\\ Invoke");
			return returnValue;
		}
	}
}
