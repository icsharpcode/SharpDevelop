// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests that pressing the enter key in the middle of a typed in line in the python console
	/// leaves the line alone and moves the cursor to the next line. By default the text editor
	/// will break the line and move the last part to the second line.
	/// </summary>
	[TestFixture]
	public class PythonConsoleEnterKeyTestFixture
	{
		MockConsoleTextEditor textEditor;
		PythonConsole console;
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			console = new PythonConsole(textEditor, null);
			console.Write(prompt, Style.Prompt);
		}
		
		public void EnterKeyDoesNotBreakUpExistingLine()
 		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEvent(Key.B);
 			textEditor.SelectionStart = 1 + prompt.Length;
 			textEditor.Column = 1 + prompt.Length;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
 			
			Assert.AreEqual(">>> AB\r\n", textEditor.Text);
 		}
 		
 		[Test]
 		public void PreviousLineIsReadOnlyAfterEnterPressed()
 		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEvent(Key.B);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
 			console.Write(prompt, Style.Prompt);
 			
 			// Move up a line with cursor.
 			textEditor.Line = 0;
			textEditor.RaisePreviewKeyDownEvent(Key.C);
 			
			string expectedText = 
				">>> AB\r\n" +
				">>> ";
 			Assert.AreEqual(expectedText, textEditor.Text);
 		}
	}
}
