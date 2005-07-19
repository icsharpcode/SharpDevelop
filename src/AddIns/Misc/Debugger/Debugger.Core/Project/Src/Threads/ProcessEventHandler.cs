// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary
{
	public delegate void ProcessEventHandler (object sender, ProcessEventArgs args);

	[Serializable]
	public class ProcessEventArgs: DebuggerEventArgs
	{
		Process process;

		public Process Process {
			get {
				return process;
			}
		}

		public ProcessEventArgs(NDebugger debugger, Process process): base(debugger)
		{
			this.process = process;
		}
	}
}
