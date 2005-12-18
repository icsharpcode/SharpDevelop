// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
{	
	[Serializable]
	public class BreakpointEventArgs : DebuggerEventArgs
	{
		Breakpoint breakpoint;
		
		public Breakpoint Breakpoint {
			get {
				return breakpoint;
			}
		}
		
		public BreakpointEventArgs(Breakpoint breakpoint): base(breakpoint.Debugger)
		{
			this.breakpoint = breakpoint;
		}
	}
}
