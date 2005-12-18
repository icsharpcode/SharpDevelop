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
