// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Gui
{
	[TestFixture]
	public class SendLineToRubyConsoleCommandTests
	{
		SendLineToRubyConsoleCommand sendLineToConsoleCommand;
		MockConsoleTextEditor fakeConsoleTextEditor;
		MockTextEditor fakeTextEditor;
		MockWorkbench workbench;
		MockRubyConsole fakeConsole;
		
		[Test]
		public void Run_SingleLineInTextEditor_FirstLineSentToRubyConsole()
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
			workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.rb");
			fakeConsoleTextEditor = workbench.MockRubyConsolePad.MockConsoleTextEditor;
			fakeConsole = workbench.MockRubyConsolePad.MockRubyConsole;
			fakeTextEditor = workbench.ActiveMockEditableViewContent.MockTextEditor;
			sendLineToConsoleCommand = new SendLineToRubyConsoleCommand(workbench);
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
		public void Run_TwoLinesInTextEditorCursorOnFirstLine_FirstLineSentToRubyConsole()
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
		public void Run_SingleLineInTextEditor_RubyConsolePadBroughtToFront()
		{
			CreateSendLineToConsoleCommand();
			AddSingleLineToTextEditor("print 'hello'");
			
			sendLineToConsoleCommand.Run();
			
			bool broughtToFront = workbench.MockRubyConsolePad.BringToFrontCalled;
			Assert.IsTrue(broughtToFront);
		}
	}
}
