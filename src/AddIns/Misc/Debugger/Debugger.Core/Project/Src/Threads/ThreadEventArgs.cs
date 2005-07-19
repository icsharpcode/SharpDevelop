// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	[Serializable]
	public class ThreadEventArgs : DebuggerEventArgs
	{
		Thread thread;
		
		public Thread Thread {
			get {
				return thread;
			}
		}
		
		public ThreadEventArgs(NDebugger debugger, Thread thread): base(debugger)
		{
			this.thread = thread;
		}
	}
}
