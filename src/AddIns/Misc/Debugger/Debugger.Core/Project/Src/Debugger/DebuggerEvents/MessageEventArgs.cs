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
	public class MessageEventArgs : ProcessEventArgs
	{
		int level;
		string message;
		string category;
		
		public int Level {
			get {
				return level;
			}
		}
		
		public string Message {
			get {
				return message;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public MessageEventArgs(Process process, string message): this(process, 0, message, String.Empty)
		{
			this.message = message;
		}
		
		public MessageEventArgs(Process process, int level, string message, string category): base(process)
		{
			this.level = level;
			this.message = message;
			this.category = category;
		}
	}
}
