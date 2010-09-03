// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	[TestFixture]
	public class PythonConsoleSendLineTests : PythonConsoleTestsBase
	{
		[Test]
		public void SendLine_NoUnreadLines_AddsLineToUnreadLines()
		{
			SendLineToConsole("test");
			string[] unreadLines = TestablePythonConsole.GetUnreadLines();
			
			string[] expectedUnreadlines = new string[] {"test"};
			
			Assert.AreEqual(expectedUnreadlines, unreadLines);
		}
		
		void SendLineToConsole(string text)
		{
			base.CreatePythonConsole();
			WritePrompt();
			TestablePythonConsole.SendLine(text);
		}
		
		void WritePrompt()
		{
			TestablePythonConsole.Write(">>> ", Style.Prompt);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_CreatesLockForPreviousLines()
		{
			SendLineToConsole("test");
			List<string> lines = TestablePythonConsole.LockCreated.Lines;
			List<string> expectedLines = TestablePythonConsole.GetUnreadLinesList();
			
			Assert.AreEqual(expectedLines, lines);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LockForPreviousLinesIsDisposed()
		{
			SendLineToConsole("test");
			bool disposed = TestablePythonConsole.LockCreated.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineNotAddedBeforeLockCreated()
		{
			SendLineToConsole("test");
			int count = TestablePythonConsole.LockCreated.UnreadLineCountWhenLockCreated;
			int expectedCount = 0;
			
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineAddedBeforeLockDisposed()
		{
			SendLineToConsole("test");
			int count = TestablePythonConsole.LockCreated.UnreadLineCountWhenLockDisposed;
			int expectedCount = 1;
			
			Assert.AreEqual(expectedCount, count);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventIsFired()
		{
			SendLineToConsole("test");
			bool fired = TestablePythonConsole.IsLineReceivedEventFired;
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SendLine_NoUnreadLines_LineReceivedEventAfterLineAddedToUnreadLines()
		{
			SendLineToConsole("test");
			int count = TestablePythonConsole.UnreadLineCountWhenLineReceivedEventFired;
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
			base.CreatePythonConsole();
			WritePrompt();
			MockConsoleTextEditor.Text = 
				">>> first\r\n" +
				">>> second\r\n" +
				">>> ";
			
			MockConsoleTextEditor.Line = 0;
			MockConsoleTextEditor.Column = 0;
			TestablePythonConsole.SendLine("test");
			
			int expectedLine = 2;
			int expectedColumn = 4;
			Location expectedLocation = new Location(expectedColumn, expectedLine);
			
			Location location = MockConsoleTextEditor.CursorLocationWhenWriteTextCalled;
			
			Assert.AreEqual(expectedLocation, location);			
		}
		
		[Test]
		public void SendLine_NoUnreadLines_NoTextWrittenToConsoleTextEditorBeforeFirstPromptIsWritten()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.SendLine("test");
			string text = MockConsoleTextEditor.TextPassedToWrite;
			
			Assert.IsNull(text);
		}
		
		[Test]
		public void Write_SendLineCalledButNoPromptWritten_WritesOutSavedSendLineText()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.SendLine("test");
			
			TestablePythonConsole.Write(">>> ", Style.Prompt);
			string text = MockConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Write_CalledTwiceAfterSendLineCalledButNoPromptWritten_WritesOutSavedSendLineTextOnlyOnce()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.SendLine("test");
			
			TestablePythonConsole.Write(">>> ", Style.Prompt);
			TestablePythonConsole.Write(">>> ", Style.Prompt);

			string text = MockConsoleTextEditor.Text;
			
			string expectedText = 
				">>> test\r\n" +
				">>> ";
			Assert.AreEqual(expectedText, text);
		}
	}
}
