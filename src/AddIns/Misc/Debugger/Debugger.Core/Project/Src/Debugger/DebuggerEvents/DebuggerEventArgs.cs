// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace DebuggerLibrary 
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
