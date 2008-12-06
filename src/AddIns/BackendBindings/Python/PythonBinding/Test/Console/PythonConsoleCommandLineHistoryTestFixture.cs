// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's command line history.
	/// </summary>
	[TestFixture]
	public class PythonConsoleCommandLineHistoryTestFixture
	{
		PythonConsole pythonConsole;
		MockTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			pythonConsole = new PythonConsole(textEditor, null);
			pythonConsole.Write(prompt, Style.Prompt);
			
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			pythonConsole.Write(prompt, Style.Prompt);
			textEditor.RaiseKeyPressEvent('b');
			textEditor.RaiseKeyPressEvent('c');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			pythonConsole.Write(prompt, Style.Prompt);
		}

		[Test]
		public void UpArrowKeyPressed()
		{
			Assert.IsTrue(textEditor.RaiseDialogKeyPressEvent(Keys.Up));
		}
		
		[Test]
		public void CurrentLineAfterUpArrowKeyPressed()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			Assert.AreEqual("bc", pythonConsole.GetCurrentLine());
		}
		
		[Test]
		public void TextEditorCursorIsAtEndOfLineAfterUpArrowKeyPressed()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			Assert.AreEqual(prompt.Length + 2, textEditor.Column);
		}
		
		[Test]
		public void TextAfterUpArrowKeyPressedTwiceThenDownArrowKey()
		{
			UpArrowKeyPressedTwiceThenDownArrowKey();
			Assert.AreEqual("bc", pythonConsole.GetCurrentLine());
		}

		[Test]
		public void TextEditorCursorAfterUpArrowKeyPressedTwice()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			Assert.AreEqual(prompt.Length + 1, textEditor.Column);
		}
		
		[Test]
		public void DownArrowKeyHandled()
		{
			Assert.IsTrue(textEditor.RaiseDialogKeyPressEvent(Keys.Down));
		}
		
		void UpArrowKeyPressedTwiceThenDownArrowKey()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			textEditor.RaiseDialogKeyPressEvent(Keys.Up);
			textEditor.RaiseDialogKeyPressEvent(Keys.Down);
		}
	}
}
