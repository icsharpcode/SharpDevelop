// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace CustomSinks
{
	public class ChatServer: MarshalByRefObject
	{
		public event TextMessageEventHandler NewMessage;

		public void SendMessage(string message)
		{
			Console.WriteLine("Message received:" + message);
			if (NewMessage != null) {
				NewMessage(this, new TextMessageEventArgs(message));
			}
		}
	}
}
