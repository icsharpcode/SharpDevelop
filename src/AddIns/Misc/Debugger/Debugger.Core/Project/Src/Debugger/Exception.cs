// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Text;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{	
	public class Exception: DebuggerObject
	{
		Thread thread;
		ExceptionType exceptionType;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return thread.Process;
			}
		}
		
		ICorDebugValue CorValue {
			get {
				return thread.CorThread.CurrentException;
			}
		}
		
		internal Exception(Thread thread, ExceptionType exceptionType)
		{
			this.thread = thread;
			this.exceptionType = exceptionType;
		}
		
		public Value RuntimeValue {
			get {
				return new Value(Process, CorValue);
			}
		}
		
		public string Type {
			get {
				return this.RuntimeValue.Type.FullName;
			}
		}
		
		public string Message {
			get {
				return this.RuntimeValue.GetMemberValue("_message").AsString;
			}
		}
		
		public ExceptionType ExceptionType {
			get {
				return exceptionType;
			}
		}
		
		public bool IsUnhandled {
			get {
				return ExceptionType == ExceptionType.DEBUG_EXCEPTION_UNHANDLED;
			}
		}
		
		public SourcecodeSegment Location {
			get {
				if (thread.MostRecentStackFrameWithLoadedSymbols != null) {
					return thread.MostRecentStackFrameWithLoadedSymbols.NextStatement;
				} else {
					return null;
				}
			}
		}
		
		public string Callstack {
			get {
				StringBuilder callstack = new StringBuilder();
				foreach(StackFrame stackFrame in thread.GetCallstack(100)) {
					callstack.Append(stackFrame.MethodInfo.Name);
					callstack.Append("()");
					SourcecodeSegment loc = stackFrame.NextStatement;
					if (loc != null) {
						callstack.Append(" - ");
						callstack.Append(loc.SourceFullFilename);
						callstack.Append(":");
						callstack.Append(loc.StartLine);
						callstack.Append(",");
						callstack.Append(loc.StartColumn);
					}
					callstack.Append("\n");
				}
				return callstack.ToString();
			}
		}
		
		public bool Intercept()
		{
			if (!thread.CorThread.Is<ICorDebugThread2>()) return false; // Is the debuggee .NET 2.0?
			if (thread.MostRecentStackFrame == null) return false; // Is frame available?  It is not at StackOverflow
			
			try {
				thread.CorThread.CastTo<ICorDebugThread2>().InterceptCurrentException(thread.MostRecentStackFrame.CorILFrame.CastTo<ICorDebugFrame>());
			} catch (COMException e) {
				// 0x80131C02: Cannot intercept this exception
				if ((uint)e.ErrorCode == 0x80131C02) {
					return false;
				}
				throw;
			}
			
			thread.CurrentException = null;
			Process.AsyncContinue_KeepDebuggeeState();
			Process.WaitForPause();
			return true;
		}
	}
	
	public class ExceptionEventArgs: ProcessEventArgs
	{
		Exception exception;
		
		public Exception Exception {
			get {
				return exception;
			}
		}
		
		public ExceptionEventArgs(Process process, Exception exception):base(process)
		{
			this.exception = exception;
		}
	}
}
