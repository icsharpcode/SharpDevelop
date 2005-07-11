// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{	
	public class Exception
	{
		NDebugger         debugger;
		Thread            thread;
		ICorDebugValue    corValue;
		Variable          runtimeVariable;
		ObjectVariable    runtimeVariableException;
		ExceptionType     exceptionType;
		SourcecodeSegment location;
		DateTime          creationTime;
		string            type;
		string            message;
	
		internal Exception(NDebugger debugger, Thread thread)
		{
			creationTime = DateTime.Now;
			this.debugger = debugger;
			this.thread = thread;
			thread.CorThread.GetCurrentException(out corValue);
			exceptionType = thread.CurrentExceptionType;
			runtimeVariable    = VariableFactory.CreateVariable(debugger, corValue, "$exception");
			runtimeVariableException = runtimeVariable as ObjectVariable;
			if (runtimeVariableException != null) {
				while (runtimeVariableException.Type != "System.Exception") {
					if (runtimeVariableException.HasBaseClass == false) {
						runtimeVariableException = null;
						break;
					}
					runtimeVariableException = runtimeVariableException.BaseClass;
				}
				message = runtimeVariableException.SubVariables["_message"].Value.ToString();
			}
			try {
				location = thread.NextStatement;
			} catch (NextStatementNotAviableException) {
				location = new SourcecodeSegment();
			}
			type = runtimeVariable.Type;
		}
	
		public override string ToString() {
			return "Exception data:\n" + runtimeVariable.ToString() + "\n" +
			       "---------------\n" +
			       runtimeVariableException.SubVariables.ToString();
		}
	
	    public string Type {
			get {
				return type;
			}
		}
	
		public string Message {
			get {
				return message;
			}
		}

		public ExceptionType ExceptionType{
			get {
				return exceptionType;
			}
		}

		public SourcecodeSegment Location {
			get {
				return location;
			}
		}

		public DateTime CreationTime {
			get {
				return creationTime;
			}
		}
	}
}
