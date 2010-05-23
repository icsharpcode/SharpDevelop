// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
	
	public class MessageReceivedEventArgs : EventArgs
	{
		string message;
		
		public MessageReceivedEventArgs(string message)
		{
			this.message = message;
		}
		
		public string Message {
			get { return message; }
		}
	}
}
