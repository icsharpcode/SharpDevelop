// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class SendSelectedTextToScriptingConsoleCommandTests : SendToScriptingConsoleCommandTestsBase
	{
		SendSelectedTextToScriptingConsoleCommand sendSelectedTextToConsoleCommand;
		
		[Test]
		public void Run_TwoLinesSelectedInTextEditor_TextSentToPythonConsole()
		{
			CreateSendSelectedTextToConsoleCommand();
			
			string selectedText = 
				"print 'a'\r\n" +
				"print 'b'\r\n";
			
			fakeTextEditor.SelectedText = selectedText;
			sendSelectedTextToConsoleCommand.Run();
			
			string text = fakeConsole.TextPassedToSendText;
			
			Assert.AreEqual(selectedText, text);
		}
		
		void CreateSendSelectedTextToConsoleCommand()
		{
			base.CreateFakeWorkbench();
			sendSelectedTextToConsoleCommand = new SendSelectedTextToScriptingConsoleCommand(workbench);
		}
		
		[Test]
		public void Run_SingleLineSelectedInTextEditor_ScriptingConsolePadBroughtToFront()
		{
			CreateSendSelectedTextToConsoleCommand();
			fakeTextEditor.SelectedText = "test";
			
			sendSelectedTextToConsoleCommand.Run();
			
			bool broughtToFront = workbench.FakeScriptingConsolePad.BringToFrontCalled;
			Assert.IsTrue(broughtToFront);
		}
	}
}
