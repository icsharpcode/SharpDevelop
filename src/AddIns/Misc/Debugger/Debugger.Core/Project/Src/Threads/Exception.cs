// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{	
	public class Exception: RemotingObjectBase
	{
		NDebugger         debugger;
		Thread            thread;
		ICorDebugValue    corValue;
		Variable          runtimeVariable;
		ObjectVariable    runtimeVariableException;
		ExceptionType     exceptionType;
		SourcecodeSegment location;
		DateTime          creationTime;
		string            callstack;
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
