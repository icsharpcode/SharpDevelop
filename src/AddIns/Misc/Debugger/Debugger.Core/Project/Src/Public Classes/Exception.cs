// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{	
	public class Exception
	{
		Thread         thread;
		ICorDebugValue corValue;
		Variable       runtimeVariable;
		ObjectVariable runtimeVariableException;
	
		internal Exception(Thread thread)
		{
			this.thread = thread;
			thread.CorThread.GetCurrentException(out corValue);
			runtimeVariable    = VariableFactory.CreateVariable(corValue, "$exception");
			runtimeVariableException = (ObjectVariable)runtimeVariable;
			while (runtimeVariableException.Type != "System.Exception") {
				if (runtimeVariableException.HasBaseClass == false) {
					runtimeVariableException = null;
					break;
				}
				runtimeVariableException = runtimeVariableException.BaseClass;
			}
		}
	
		public override string ToString() {
			return "Exception data:\n" + runtimeVariable.ToString() + "\n" +
			       "---------------\n" +
			       runtimeVariableException.SubVariables.ToString();
		}
	
	    public string Type {
			get {
				return runtimeVariable.Type;
			}
		}
	
		public string Message {
			get {
				return runtimeVariableException.SubVariables["_message"].Value.ToString();
			}
		}
		
		public bool IsHandled {
			get {
				return thread.CurrentExceptionIsHandled;
			}
		}
	}
}
