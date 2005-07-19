// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void DebuggerEventHandler (object sender, DebuggerEventArgs e);
	
	[Serializable]
	public class DebuggerEventArgs : EventArgs 
	{
		NDebugger debugger;

		public NDebugger Debugger {
			get {
				return debugger;
			}
		}

		public DebuggerEventArgs(NDebugger debugger)
		{
			this.debugger = debugger;
		}
	}
}
