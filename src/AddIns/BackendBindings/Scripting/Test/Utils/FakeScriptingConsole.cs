// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeScriptingConsole : IScriptingConsole
	{
		public event EventHandler LineReceived;
		
		public string TextPassedToSendLine;
		public string TextPassedToSendText;
		public bool IsWriteLineCalled;
		public string TextPassedToWriteLine;
		public ScriptingStyle ScriptingStylePassedToWriteLine;
		public string TextPassedToWrite;
		public ScriptingStyle ScriptingStylePassedToWrite;
		public string TextToReturnFromReadLine;
		public int AutoIndentSizePassedToReadLine;
		public string TextToReturnFromReadFirstUnreadLine;
		public bool IsReadLineCalled;
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
		}
		
		public void SendText(string text)
		{
			TextPassedToSendText = text;
		}
		
		public void WriteLine()
		{
			IsWriteLineCalled = true;
		}
		
		public void WriteLine(string text, ScriptingStyle style)
		{
			TextPassedToWriteLine = text;
			ScriptingStylePassedToWriteLine = style;
		}
		
		public void Write(string text, ScriptingStyle style)
		{
			TextPassedToWrite = text;
			ScriptingStylePassedToWrite = style;
		}
		
		public string ReadLine(int autoIndentSize)
		{
			IsReadLineCalled = true;
			AutoIndentSizePassedToReadLine = autoIndentSize;
			return TextToReturnFromReadLine;
		}
		
		public string ReadFirstUnreadLine()
		{
			return TextToReturnFromReadFirstUnreadLine;
		}
		
		public void FireLineReceivedEvent()
		{
			if (LineReceived != null) {
				LineReceived(this, new EventArgs());
			}
		}
	}
}
