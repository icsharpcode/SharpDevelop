// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	public class DebuggerException: System.Exception
	{
		public DebuggerException() {}
		public DebuggerException(string message): base(message) {}
		public DebuggerException(string message, System.Exception inner): base(message, inner) {}
	}
	
	public class ProcessExitedException: DebuggerException
	{
		public ProcessExitedException(): base("Process exited") {}
	}
}
