// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the ScriptingConsole ReadLine method.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleReadTests : ScriptingConsoleTestsBase
	{	
		[Test]
		public void ReadLine_AutoIndentIsTwo_TwoSpacesWrittenToConsoleTextEditor()
		{
			CreateConsole();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			int indent = 2;
			TestableScriptingConsole.ReadLine(indent);
			
			string expectedText = "  ";
			Assert.AreEqual(expectedText, FakeConsoleTextEditor.TextPassedToAppend);
		}
		
		[Test]
		public void ReadLine_AutoIndentIsZero_NoTextWrittenToConsoleTextEditor()
		{
			CreateConsole();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			FakeConsoleTextEditor.IsAppendCalled = false;
			
			TestableScriptingConsole.ReadLine(0);
			
			Assert.IsFalse(FakeConsoleTextEditor.IsAppendCalled);
		}
		
		[Test]
		public void ProcessPreviewKeyDown_TextEditorPreviewKeyDownEventFiresWithLetterA_ReturnsFalseForLetterThatShouldBeHandledByTextEditorItself()
		{
			CreateConsole();
			bool result = FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ProcessPreviewKeyDown_TextEditorPreviewKeyDownEventFiresWithEnterKey_ReturnsFalseForLetterThatShouldBeHandledByTextEditorItself()
		{
			CreateConsole();
			bool result = FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ReadLine_NoLinesAvailable_ReturnsNull()
		{
			CreateConsole();
			string line = TestableScriptingConsole.ReadLine(0);
			
			Assert.IsNull(line);
		}
		
		[Test]
		public void ReadLine_OneLineWaitingAndAutoIndentIsTwo_TwoSpacesAddedToLineText()
		{
			CreateConsole();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			int indent = 2;
			string line = TestableScriptingConsole.ReadLine(indent);
			
			string expectedLine = "  A";
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void FireLineReceivedEvent_LineReceivedEventHandlerRegistered_CallsEventHandler()
		{
			CreateConsole();
			
			bool fired = false;
			TestableScriptingConsole.LineReceived += delegate { 
				fired = true;
			};
			
			TestableScriptingConsole.CallBaseFireLineReceivedEvent();
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void ReadFirstUnreadLine_OneLineUnread_ReturnsLine()
		{
			CreateConsole();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			string line = TestableScriptingConsole.ReadFirstUnreadLine();
			
			string expectedline = "A";
			Assert.AreEqual(expectedline, line);
		}
		
		[Test]
		public void ReadFirstUnreadLine_OneLineUnreadReadTwoLines_ReturnsNull()
		{
			CreateConsole();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			TestableScriptingConsole.ReadFirstUnreadLine();
			string line = TestableScriptingConsole.ReadFirstUnreadLine();
			
			Assert.IsNull(line);
		}
	}
}
