// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
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
