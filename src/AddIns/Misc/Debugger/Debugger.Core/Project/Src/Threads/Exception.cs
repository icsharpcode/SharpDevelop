// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{	
	public class Exception: DebuggerObject
	{
		Process           process;
		Thread            thread;
		ICorDebugValue    corValue;
		ExceptionType     exceptionType;
		SourcecodeSegment location;
		DateTime          creationTime;
		string            callstack;
		string            type;
		string            message;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		internal Exception(Thread thread)
		{
			creationTime = DateTime.Now;
			this.process = thread.Process;
			this.thread = thread;
			corValue = thread.CorThread.CurrentException;
			exceptionType = thread.CurrentExceptionType;
			Value runtimeValue = new Value(process,
			                               string.Empty,
			                               delegate { return corValue; } );
			message = runtimeValue.GetMember("_message").AsString;
			
			if (thread.LastFunctionWithLoadedSymbols != null) {
				location = thread.LastFunctionWithLoadedSymbols.NextStatement;
			}
			
			callstack = "";
			int callstackItems = 0;
			foreach(Function function in thread.Callstack) {
				if (callstackItems >= 100) {
					callstack += "...\n";
					break;
				}
				
				SourcecodeSegment loc = function.NextStatement;
				callstack += function.Name + "()";
				if (loc != null) {
					callstack += " - " + loc.SourceFullFilename + ":" + loc.StartLine + "," + loc.StartColumn;
				}
				callstack += "\n";
				callstackItems++;
			}
			
			type = runtimeValue.Type.FullName;
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
		
		public string Callstack {
			get {
				return callstack;
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
