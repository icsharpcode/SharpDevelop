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
