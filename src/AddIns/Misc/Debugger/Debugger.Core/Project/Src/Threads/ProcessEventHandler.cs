// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary
{
	public delegate void ProcessEventHandler (object sender, ProcessEventArgs args);

	[Serializable]
	public class ProcessEventArgs: EventArgs
	{
		Process process;

		public Process Process {
			get {
				return process;
			}
		}

		public ProcessEventArgs(Process process)
		{
			this.process = process;
		}
	}
}
