// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Debugger
{
	delegate object MethodInvokerWithReturnValue();
	
	class MTA2STA
	{
		Form hiddenForm;
		IntPtr hiddenFormHandle;
		
		public MTA2STA()
		{
			hiddenForm = new Form();
			// Force handle creation
			hiddenFormHandle = hiddenForm.Handle;
		}
		
		static void TraceMsg(string msg)
		{
			//System.Console.WriteLine("MTA2STA: " + msg);
		}
		
		// Try to avoid this since it will catch exceptions and it is slow
		public object CallInSTA(object targetObject, string functionName, object[] functionParameters)
		{
			return CallInSTA(delegate { return InvokeMethod(targetObject, functionName, functionParameters); });
		}
		
		public void CallInSTA(MethodInvoker callDelegate)
		{
			CallInSTA(callDelegate, true);
		}
		
		public object CallInSTA(MethodInvokerWithReturnValue callDelegate)
		{
			object returnValue = null;
			CallInSTA(delegate { returnValue = callDelegate(); }, false);
			return returnValue;
		}
			
		void CallInSTA(MethodInvoker callDelegate, bool mayAbandon)
		{
			if (hiddenForm.InvokeRequired == true) {
				// Warrning: BeginInvoke will not pass exceptions if you do not use MethodInvoker delegate!
				IAsyncResult async = hiddenForm.BeginInvoke(callDelegate);
				// Give it 1 second to run
				if (!async.AsyncWaitHandle.WaitOne(1000, true)) {
					// Abandon the call if possible
					if (mayAbandon) {
						System.Console.WriteLine("Callback time out! Unleashing thread.");
					} else {
						System.Console.WriteLine("Warring: Call in STA is taking too long");
						hiddenForm.EndInvoke(async); // Keep waiting
					}
				}
			} else {
				callDelegate();
			}
		}
		
		public static object MarshalParamTo(object param, Type outputType)
		{
			if (param is IntPtr) {
				return MarshalIntPtrTo((IntPtr)param, outputType);
			} else {
				return param;
			}
		}
		
		public static T MarshalIntPtrTo<T>(IntPtr param)
		{
			return (T)MarshalIntPtrTo(param, typeof(T));
		}
		
		public static object MarshalIntPtrTo(IntPtr param, Type outputType)
		{
			// IntPtr requested as output (must be before the null check so that we pass IntPtr.Zero)
			if (outputType == typeof(IntPtr)) {
				return param;
			}
			// The parameter is null pointer
			if ((IntPtr)param == IntPtr.Zero) {
				return null;
			}
			// String requested as output
			if (outputType == typeof(string)) {
				return Marshal.PtrToStringAuto((IntPtr)param);
			}
			// Marshal a COM object
			return Marshal.GetTypedObjectForIUnknown((IntPtr)param, outputType);
		}
		
		/// <summary>
		/// Uses reflection to call method. Automaticaly marshals parameters.
		/// </summary>
		/// <param name="targetObject">Targed object which contains the method. In case of static mehod pass the Type</param>
		/// <param name="functionName">The name of the function to call</param>
		/// <param name="functionParameters">Parameters which should be send to the function. Parameters will be marshaled to proper type.</param>
		/// <returns>Return value of the called function</returns>
		public static object InvokeMethod(object targetObject, string functionName, object[] functionParameters)
		{
			MethodInfo method;
			if (targetObject is Type) {
				method = ((Type)targetObject).GetMethod(functionName);
			} else {
				method = targetObject.GetType().GetMethod(functionName);
			}
			
			ParameterInfo[] methodParamsInfo = method.GetParameters();
			object[] convertedParams = new object[methodParamsInfo.Length];
			
			for (int i = 0; i < convertedParams.Length; i++) {
				convertedParams[i] = MarshalParamTo(functionParameters[i], methodParamsInfo[i].ParameterType);
			}
			
			TraceMsg ("Invoking " + functionName + "...");
			try {
				if (targetObject is Type) {
					return method.Invoke(null, convertedParams);
				} else {
					return method.Invoke(targetObject, convertedParams);
				}
			} catch (System.Exception exception) {
				throw new Debugger.DebuggerException("Invoke of " + functionName + " failed.", exception);
			}
		}
	}
}
