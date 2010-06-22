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
	public class PythonConsoleCommandLineHistoryTestFixture
	{
		PythonConsole pythonConsole;
		MockConsoleTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			pythonConsole = new PythonConsole(textEditor, null);
			pythonConsole.Write(prompt, Style.Prompt);
			
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			pythonConsole.Write(prompt, Style.Prompt);
			textEditor.RaisePreviewKeyDownEvent(Key.B);
			textEditor.RaisePreviewKeyDownEvent(Key.C);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			pythonConsole.Write(prompt, Style.Prompt);
		}

		[Test]
		public void UpArrowKeyPressed()
		{
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CurrentLineAfterUpArrowKeyPressed()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual("BC", pythonConsole.GetCurrentLine());
		}
		
		[Test]
		public void TextEditorCursorIsAtEndOfLineAfterUpArrowKeyPressed()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 2, textEditor.Column);
		}
		
		[Test]
		public void TextAfterUpArrowKeyPressedTwiceThenDownArrowKey()
		{
			UpArrowKeyPressedTwiceThenDownArrowKey();
			Assert.AreEqual("BC", pythonConsole.GetCurrentLine());
		}

		[Test]
		public void TextEditorCursorAfterUpArrowKeyPressedTwice()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 1, textEditor.Column);
		}
		
		[Test]
		public void DownArrowKeyHandled()
		{
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}
		
		void UpArrowKeyPressedTwiceThenDownArrowKey()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down);
		}
	}
}
