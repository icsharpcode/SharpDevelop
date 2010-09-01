// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's command line history.
	/// </summary>
	[TestFixture]
	public class PythonConsoleCommandLineHistoryTestFixture : PythonConsoleTestsBase
	{
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.Write(prompt, Style.Prompt);
			
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestablePythonConsole.Write(prompt, Style.Prompt);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestablePythonConsole.Write(prompt, Style.Prompt);
		}

		[Test]
		public void UpArrowKeyPressed()
		{
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CurrentLineAfterUpArrowKeyPressed()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual("BC", TestablePythonConsole.GetCurrentLine());
		}
		
		[Test]
		public void TextEditorCursorIsAtEndOfLineAfterUpArrowKeyPressed()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 2, MockConsoleTextEditor.Column);
		}
		
		[Test]
		public void TextAfterUpArrowKeyPressedTwiceThenDownArrowKey()
		{
			UpArrowKeyPressedTwiceThenDownArrowKey();
			Assert.AreEqual("BC", TestablePythonConsole.GetCurrentLine());
		}

		[Test]
		public void TextEditorCursorAfterUpArrowKeyPressedTwice()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 1, MockConsoleTextEditor.Column);
		}
		
		[Test]
		public void DownArrowKeyHandled()
		{
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}
		
		void UpArrowKeyPressedTwiceThenDownArrowKey()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down);
		}
	}
}
