// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	[TestFixture]
	public class RubyConsoleSendLineTests : RubyConsoleTestsBase
	{
		[Test]
		public void SendLine_NoUnreadLines_AddsLineToUnreadLines()
		{
			SendLineToConsole("test");
			string[] unreadLines = TestableRubyConsole.GetUnreadLines();
			
			string[] expectedUnreadlines = new string[] {"test"};
			
			Assert.AreEqual(expectedUnreadlines, unreadLines);
		}
		
		void SendLineToConsole(string text)
		{
			base.CreateRubyConsole();
			WritePrompt();
			TestableRubyConsole.SendLine(text);
		}
		
		void WritePrompt()
		{
			TestableRubyConsole.Write(">>> ", Style.Prompt);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_CreatesLockForPreviousLines()
		{
			SendLineToConsole("test");
			List<string> lines = TestableRubyConsole.LockCreated.Lines;
			List<string> expectedLines = TestableRubyConsole.GetUnreadLinesList();
			
			Assert.AreEqual(expectedLines, lines);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LockForPreviousLinesIsDisposed()
		{
			SendLineToConsole("test");
			bool disposed = TestableRubyConsole.LockCreated.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineNotAddedBeforeLockCreated()
		{
			SendLineToConsole("test");
			int count = TestableRubyConsole.LockCreated.UnreadLineCountWhenLockCreated;
			int expectedCount = 0;
			
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineAddedBeforeLockDisposed()
		{
			SendLineToConsole("test");
			int count = TestableRubyConsole.LockCreated.UnreadLineCountWhenLockDisposed;
			int expectedCount = 1;
			
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventIsFired()
		{
			SendLineToConsole("test");
			bool fired = TestableRubyConsole.IsLineReceivedEventFired;
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventAfterLineAddedToUnreadLines()
		{
			SendLineToConsole("test");
			int count = TestableRubyConsole.UnreadLineCountWhenLineReceivedEventFired;
			int expectedCount = 1;
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineWrittenToConsoleTextEditor()
		{
			SendLineToConsole("test");
			string text = MockConsoleTextEditor.TextPassedToWrite;
			string expectedTextWritten = "test\r\n";
			
			Assert.AreEqual(expectedTextWritten, text);
		}
		
		[Test]
		public void SendLine_TwoLinesInConsoleTextEditorCursorOnFirstLine_CursorMovedToEndOfLastLineBeforeTextWritten()
		{
			base.CreateRubyConsole();
			WritePrompt();
			MockConsoleTextEditor.Text = 
				">>> first\r\n" +
				">>> second\r\n" +
				">>> ";
			
			MockConsoleTextEditor.Line = 0;
			MockConsoleTextEditor.Column = 0;
			TestableRubyConsole.SendLine("test");
			
			int expectedLine = 2;
			int expectedColumn = 4;
			Location expectedLocation = new Location(expectedColumn, expectedLine);
			
			Location location = MockConsoleTextEditor.CursorLocationWhenWriteTextCalled;
			
			Assert.AreEqual(expectedLocation, location);			
		}
		
		[Test]
		public void SendLine_NoUnreadLines_NoTextWrittenToConsoleTextEditorBeforeFirstPromptIsWritten()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.SendLine("test");
			string text = MockConsoleTextEditor.TextPassedToWrite;
			
			Assert.IsNull(text);
		}
		
		[Test]
		public void Write_SendLineCalledButNoPromptWritten_WritesOutSavedSendLineText()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.SendLine("test");
			
			TestableRubyConsole.Write(">>> ", Style.Prompt);
			string text = MockConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_CalledTwiceAfterSendLineCalledButNoPromptWritten_WritesOutSavedSendLineTextOnlyOnce()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.SendLine("test");
			
			TestableRubyConsole.Write(">>> ", Style.Prompt);
			TestableRubyConsole.Write(">>> ", Style.Prompt);

			string text = MockConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n" +
				">>> ";
			Assert.AreEqual(expectedText, text);
		}
	}
}
