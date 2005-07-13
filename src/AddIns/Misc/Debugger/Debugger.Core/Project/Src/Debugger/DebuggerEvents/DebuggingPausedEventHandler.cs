// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void DebuggingPausedEventHandler (object sender, DebuggingPausedEventArgs e);
	
	[Serializable]
	public class DebuggingPausedEventArgs : System.EventArgs 
	{
		PausedReason reason;
		
		public PausedReason Reason {
			get {
				return reason;
			}
		}
		
		public DebuggingPausedEventArgs(PausedReason reason)
		{
			this.reason = reason;
		}
	}
}
