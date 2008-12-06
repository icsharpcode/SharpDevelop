// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger
{
	/// <summary>
	/// Holds information about the state of paused debugger.
	/// Expires when when Continue is called on debugger.
	/// </summary>
	public class PauseSession: DebuggerObject
	{
		Process process;
		PausedReason pausedReason;
		
		public Process Process {
			get { return process; }
		}
		
		public PausedReason PausedReason {
			get { return pausedReason; }
		}
		
		public PauseSession(Process process, PausedReason pausedReason)
		{
			this.process = process;
			this.pausedReason = pausedReason;
		}
	}
}
