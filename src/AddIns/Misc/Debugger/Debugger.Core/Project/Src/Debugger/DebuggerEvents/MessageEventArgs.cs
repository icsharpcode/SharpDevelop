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
	public class MessageEventArgs : DebuggerEventArgs
	{
		string message;
		
		public string Message {
			get {
				return message;
			}
		}
		
		public MessageEventArgs(NDebugger debugger, string message): base(debugger)
		{
			this.message = message;
		}
	}
}
