// <file>
//     <owner name="David Srbeckı" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void MessageEventHandler (object sender, MessageEventArgs e);
	
	public class MessageEventArgs : System.EventArgs 
	{
		string message;
		
		public string Message {
			get {
				return message;
			}
		}
		
		public MessageEventArgs(string message)
		{
			this.message = message;
		}
	}
}
