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
	public delegate void TextMessageEventHandler (object sender, TextMessageEventArgs args);

	[Serializable]
	public class TextMessageEventArgs : EventArgs
	{
		string message;

		public TextMessageEventArgs(string message)
		{
			this.message = message;
		}

		public string Message {
			get {
				return message;
			}
		}
	}
}
