// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void BreakpointEventHandler (object sender, BreakpointEventArgs e);
	
	[Serializable]
	public class BreakpointEventArgs : DebuggerEventArgs
	{
		Breakpoint breakpoint;
		
		public Breakpoint Breakpoint {
			get {
				return breakpoint;
			}
		}
		
		public BreakpointEventArgs(NDebugger debugger, Breakpoint breakpoint): base(debugger)
		{
			this.breakpoint = breakpoint;
		}
	}
}
