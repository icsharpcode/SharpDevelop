// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
