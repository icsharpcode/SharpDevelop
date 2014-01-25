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
using ICSharpCode.NRefactory;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ScriptingConsoleSendLineTests : ScriptingConsoleTestsBase
	{
		[Test]
		public void SendLine_NoUnreadLines_AddsLineToUnreadLines()
		{
			SendLineToConsole("test");
			string[] unreadLines = TestableScriptingConsole.GetUnreadLines();
			
			string[] expectedUnreadlines = new string[] {"test"};
			
			Assert.AreEqual(expectedUnreadlines, unreadLines);
		}
		
		void SendLineToConsole(string text)
		{
			base.CreateConsole();
			WritePrompt();
			TestableScriptingConsole.SendLine(text);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventIsFired()
		{
			SendLineToConsole("test");
			bool fired = TestableScriptingConsole.IsLineReceivedEventFired;
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventAfterLineAddedToUnreadLines()
		{
			SendLineToConsole("test");
			int count = TestableScriptingConsole.UnreadLineCountWhenLineReceivedEventFired;
			int expectedCount = 1;
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineWrittenToConsoleTextEditor()
		{
			SendLineToConsole("test");
			string text = FakeConsoleTextEditor.TextPassedToAppend;
			string expectedTextWritten = "test\r\n";
			
			Assert.AreEqual(expectedTextWritten, text);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_NoTextWrittenToConsoleTextEditorBeforeFirstPromptIsWritten()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendLine("test");
			string text = FakeConsoleTextEditor.TextPassedToAppend;
			
			Assert.IsNull(text);
		}
		
		[Test]
		public void Write_SendLineCalledButNoPromptWritten_WritesOutSavedSendLineText()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendLine("test");
			
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			string text = FakeConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_CalledTwiceAfterSendLineCalledButNoPromptWritten_WritesOutSavedSendLineTextOnlyOnce()
		{
			base.CreateConsole();
			TestableScriptingConsole.SendLine("test");
			
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);
			TestableScriptingConsole.Write(">>> ", ScriptingStyle.Prompt);

			string text = FakeConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n" +
				">>> ";
			Assert.AreEqual(expectedText, text);
		}
	}
}
