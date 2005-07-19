// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void DebuggingPausedEventHandler (object sender, DebuggingPausedEventArgs e);
	
	[Serializable]
	public class DebuggingPausedEventArgs : DebuggerEventArgs
	{
		PausedReason reason;

		bool resumeDebugging = false;
		
		public PausedReason Reason {
			get {
				return reason;
			}
		}

		internal bool ResumeDebugging {
			get {
				return resumeDebugging;
			}
		}

		/// <summary>
		/// Call this function to resume debugging when event is handled
		/// 
		/// This is prefered to calling Continue() since it ensures Continue is
		/// called only once and never before all events are handled
		/// </summary>
		public void ResumeDebuggingAfterEvent()
		{
			resumeDebugging = true;
		}
		
		public DebuggingPausedEventArgs(NDebugger debugger, PausedReason reason): base(debugger)
		{
			this.reason = reason;
		}
	}
}
