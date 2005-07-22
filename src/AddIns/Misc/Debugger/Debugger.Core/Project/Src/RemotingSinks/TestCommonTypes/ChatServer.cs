// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
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
