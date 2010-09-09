// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ScriptingConsoleSendTextTests : ScriptingConsoleTestsBase
	{
		[Test]
		public void SendText_NoNewLineInText_AppendTextToEndOfActiveConsoleInputLine()
		{
			SendTextToConsole("test");
			
			string text = base.MockConsoleTextEditor.TextPassedToWrite;
			string expectedText = "test";
			Assert.AreEqual(expectedText, text);
		}
		
		void SendTextToConsole(string text)
		{
			base.CreateConsole();
			WritePrompt();
			TestableScriptingConsole.SendText(text);
		}
		
		[Test]
		public void SendText_TextEditorHasTextAfterPrompt_CursorMovedToEndOfLastLineBeforeTextWritten()
		{
			base.CreateConsole();
			WritePrompt();
			MockConsoleTextEditor.Text =  ">>> first";
			
			MockConsoleTextEditor.Line = -1;
			MockConsoleTextEditor.Column = -1;
			TestableScriptingConsole.SendText("test");
			
			int expectedLine = 0;
			int expectedColumn = 9;
			Location expectedLocation = new Location(expectedColumn, expectedLine);
			
			Location location = MockConsoleTextEditor.CursorLocationWhenWriteTextCalled;
			
			Assert.AreEqual(expectedLocation, location);			
		}
		
		[Test]
		public void SendText_FirstPromptNotYetWrittenToConsole_NoTextWrittenToConsoleTextEditor()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendText("test");
			string text = MockConsoleTextEditor.TextPassedToWrite;
			
			Assert.IsNull(text);
		}
		
		[Test]
		public void Write_SendTextCalledButNoPromptWritten_WritesOutSavedText()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendText("test");
			
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			string text = MockConsoleTextEditor.Text;
			
			string expectedText =  ">>> test";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void SendText_TwoLinesSelected_FirstLineOfTextWrittenToTextEditor()
		{
			string selectedText = 
				"first\r\n" +
				"second";
			
			SendTextToConsole(selectedText);
			
			string text = base.MockConsoleTextEditor.TextPassedToWrite;
			string expectedText = "first\r\n";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_TwoLinesSelectedAndFirstLineOfTextWrittenToTextEditor_SecondLineWrittenAfterPrompt()
		{
			string selectedText = 
				"first\r\n" +
				"second";
			
			SendTextToConsole(selectedText);
			WritePrompt();
			
			string text = base.MockConsoleTextEditor.TextPassedToWrite;
			string expectedText = "second";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void SendText_TwoLinesSelected_UnreadLinesContainsFirstLineOnly()
		{
			string selectedText = 
				"first\r\n" +
				"second";
			
			SendTextToConsole(selectedText);
			
			string[] unreadLines = base.TestableScriptingConsole.GetUnreadLines();
			string[] expectedUnreadLines = new string[] { "first" };
			Assert.AreEqual(expectedUnreadLines, unreadLines);
		}
		
		[Test]
		public void SendText_TwoLinesSelected_LineReceivedEventIsFired()
		{
			string selectedText = 
				"first\r\n" +
				"second";
			
			SendTextToConsole(selectedText);
			bool fired = TestableScriptingConsole.IsLineReceivedEventFired;
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SendText_ThreeLinesSelected_UnreadLinesContainsFirstTwoLines()
		{
			string selectedText = 
				"first\r\n" +
				"second\r\n" +
				"third";
			
			SendTextToConsole(selectedText);
			
			string[] unreadLines = base.TestableScriptingConsole.GetUnreadLines();
			string[] expectedUnreadLines = new string[] { "first", "second" };
			Assert.AreEqual(expectedUnreadLines, unreadLines);
		}
	}
}
