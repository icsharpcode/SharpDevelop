// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
