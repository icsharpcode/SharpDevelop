// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void DebuggingIsResumingEventHandler (object sender, DebuggingIsResumingEventArgs e);
	
	[Serializable]
	public class DebuggingIsResumingEventArgs : DebuggerEventArgs
	{
		bool abort;
		
		public bool Abort {
			get {
				return abort;
			}
			set {
				abort = value;
			}
		}
		
		public DebuggingIsResumingEventArgs(NDebugger debugger): base(debugger)
		{
			this.abort = false;
		}
	}
}
