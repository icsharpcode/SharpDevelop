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
using ICSharpCode.NRefactory;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
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
			
			string text = base.FakeConsoleTextEditor.TextPassedToAppend;
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
		public void SendText_FirstPromptNotYetWrittenToConsole_NoTextWrittenToConsoleTextEditor()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendText("test");
			string text = FakeConsoleTextEditor.TextPassedToAppend;
			
			Assert.IsNull(text);
		}
		
		[Test]
		public void Write_SendTextCalledButNoPromptWritten_WritesOutSavedText()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendText("test");
			
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			string text = FakeConsoleTextEditor.TextPassedToAppend;
			
			string expectedText = "test";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_SendTextCalledWithTwoLinesButNoPromptWrittenAndWriteCalledTwice_WritesOutSecondLineOfSavedText()
		{
			base.CreateConsole();
			string text = 
				"first\r\n" +
				"second";
			TestableScriptingConsole.SendText(text);
			
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			
			string textPassedToWrite = FakeConsoleTextEditor.TextPassedToAppend;
			string expectedText =  "second";
			
			Assert.AreEqual(expectedText, textPassedToWrite);
		}
		
		[Test]
		public void SendText_TwoLinesSelected_FirstLineOfTextWrittenToTextEditor()
		{
			string selectedText = 
				"first\r\n" +
				"second";
			
			SendTextToConsole(selectedText);
			
			string text = base.FakeConsoleTextEditor.TextPassedToAppend;
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
			
			string text = base.FakeConsoleTextEditor.TextPassedToAppend;
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
