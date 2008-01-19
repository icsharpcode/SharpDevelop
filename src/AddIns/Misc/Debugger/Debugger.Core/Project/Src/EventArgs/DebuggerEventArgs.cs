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
