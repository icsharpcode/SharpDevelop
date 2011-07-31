// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeScriptingConsole : IScriptingConsole
	{
		public event EventHandler LineReceived;
		
		public string TextPassedToSendLine;
		public List<string> AllTextPassedToSendLine = new List<string>();
		public string TextPassedToSendText;
		public bool IsWriteLineCalled;
		public string TextPassedToWriteLine;
		public ScriptingStyle ScriptingStylePassedToWriteLine;
		public string TextPassedToWrite;
		public List<string> AllTextPassedToWrite = new List<string>();
		public List<string> AllTextPassedToWriteLine = new List<string>();
		public ScriptingStyle ScriptingStylePassedToWrite;
		public string TextToReturnFromReadLine;
		public int AutoIndentSizePassedToReadLine;
		public string TextToReturnFromReadFirstUnreadLine;
		public bool IsReadLineCalled;
		public bool IsDisposeCalled;
		public int MaximumVisibleColumns;
		public bool IsClearCalled;
		
		public string LastLinePassedToWriteLine {
			get { return AllTextPassedToWriteLine[AllTextPassedToWriteLine.Count - 1]; }
		}
		
		public string FirstLinePassedToSendLine {
			get { return AllTextPassedToSendLine[0]; }
		}
		
		public string LastLinePassedToSendLine {
			get { return AllTextPassedToSendLine[AllTextPassedToSendLine.Count - 1]; }
		}
		
		public bool ScrollToEndWhenTextWritten { get; set; }
		
		public void SendLine(string text)
		{
			TextPassedToSendLine = text;
			AllTextPassedToSendLine.Add(text);
		}
		
		public void SendText(string text)
		{
			TextPassedToSendText = text;
		}
		
		public void WriteLine()
		{
			IsWriteLineCalled = true;
			AllTextPassedToWriteLine.Add(String.Empty);
		}
		
		public void WriteLine(string text, ScriptingStyle style)
		{
			TextPassedToWriteLine = text;
			ScriptingStylePassedToWriteLine = style;
			AllTextPassedToWriteLine.Add(text);
		}
		
		public void Write(string text, ScriptingStyle style)
		{
			TextPassedToWrite = text;
			ScriptingStylePassedToWrite = style;
			AllTextPassedToWrite.Add(text);
		}
		
		public virtual string ReadLine(int autoIndentSize)
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
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
		
		public int GetMaximumVisibleColumns()
		{
			return MaximumVisibleColumns;
		}
		
		public void Clear()
		{
			IsClearCalled = true;
		}
	}
}
