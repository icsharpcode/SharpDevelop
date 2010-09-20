// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class SendLineToScriptingConsoleCommandTests : SendToScriptingConsoleCommandTestsBase
	{
		SendLineToScriptingConsoleCommand sendLineToConsoleCommand;
		
		[Test]
		public void Run_SingleLineInTextEditor_FirstLineSentToPythonConsole()
		{
			CreateSendLineToConsoleCommand();
			AddSingleLineToTextEditor("print 'hello'");
			sendLineToConsoleCommand.Run();
			
			string text = fakeConsole.TextPassedToSendLine;
			
			string expectedText = "print 'hello'";
			Assert.AreEqual(expectedText, text);
		}
		
		void CreateSendLineToConsoleCommand()
		{
			base.CreateFakeWorkbench();
			sendLineToConsoleCommand = new SendLineToScriptingConsoleCommand(workbench);
		}
		
		void AddSingleLineToTextEditor(string line)
		{
			fakeTextEditor.Document.Text = line;
			fakeTextEditor.Caret.Line = 1;

			SetTextToReturnFromTextEditorGetLine(line);
		}
		
		void SetTextToReturnFromTextEditorGetLine(string line)
		{
			FakeDocumentLine documentLine = new FakeDocumentLine();
			documentLine.Text = line;
			fakeTextEditor.FakeDocument.DocumentLineToReturnFromGetLine = documentLine;			
		}
		
		[Test]
		public void Run_TwoLinesInTextEditorCursorOnFirstLine_FirstLineSentToPythonConsole()
		{
			CreateSendLineToConsoleCommand();
			
			fakeTextEditor.Document.Text = 
				"print 'hello'\r\n" +
				"print 'world'\r\n";
			
			fakeTextEditor.Caret.Line = 1;
			
			SetTextToReturnFromTextEditorGetLine("print 'hello'");
			
			sendLineToConsoleCommand.Run();
			string text = fakeConsole.TextPassedToSendLine;
			
			string expectedText = "print 'hello'";
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Run_SingleLineInTextEditor_ScriptingConsolePadBroughtToFront()
		{
			CreateSendLineToConsoleCommand();
			AddSingleLineToTextEditor("print 'hello'");
			
			sendLineToConsoleCommand.Run();
			
			bool broughtToFront = workbench.FakeScriptingConsolePad.BringToFrontCalled;
			Assert.IsTrue(broughtToFront);
		}
	}
}
