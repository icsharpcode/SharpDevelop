// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
