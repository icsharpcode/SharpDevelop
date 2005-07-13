// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void ThreadEventHandler (object sender, ThreadEventArgs e);
	
	[Serializable]
	public class ThreadEventArgs : System.EventArgs 
	{
		Thread thread;
		
		public Thread Thread {
			get {
				return thread;
			}
		}
		
		public ThreadEventArgs(Thread thread)
		{
			this.thread = thread;
		}
	}
}
