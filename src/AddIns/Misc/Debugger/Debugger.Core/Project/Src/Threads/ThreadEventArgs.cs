// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger 
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
