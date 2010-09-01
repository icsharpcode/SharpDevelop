// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Gui
{
	[TestFixture]
	public class SendLineToPythonConsoleCommandTests
	{
		SendLineToPythonConsoleCommand sendLineToConsoleCommand;
		MockConsoleTextEditor fakeConsoleTextEditor;
		MockTextEditor fakeTextEditor;
		MockWorkbench workbench;
		MockPythonConsole fakeConsole;
		
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
			workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.py");
			fakeConsoleTextEditor = workbench.MockPythonConsolePad.MockConsoleTextEditor;
			fakeConsole = workbench.MockPythonConsolePad.MockPythonConsole;
			fakeTextEditor = workbench.ActiveMockEditableViewContent.MockTextEditor;
			sendLineToConsoleCommand = new SendLineToPythonConsoleCommand(workbench);
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
		public void Run_SingleLineInTextEditor_PythonConsolePadBroughtToFront()
		{
			CreateSendLineToConsoleCommand();
			AddSingleLineToTextEditor("print 'hello'");
			
			sendLineToConsoleCommand.Run();
			
			bool broughtToFront = workbench.MockPythonConsolePad.BringToFrontCalled;
			Assert.IsTrue(broughtToFront);
		}
	}
}
