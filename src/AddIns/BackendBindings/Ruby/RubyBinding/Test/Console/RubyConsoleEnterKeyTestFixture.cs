// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests that pressing the enter key in the middle of a typed in line in the Ruby console
	/// leaves the line alone and moves the cursor to the next line. By default the text editor
	/// will break the line and move the last part to the second line.
	/// </summary>
	[TestFixture]
	public class RubyConsoleEnterKeyTestFixture
	{
		MockTextEditor textEditor;
		RubyConsole console;
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			console = new RubyConsole(textEditor, null);
			console.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void EnterKeyDoesNotBreakUpExistingLine()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseKeyPressEvent('b');
			textEditor.SelectionStart = 1 + prompt.Length;
			textEditor.Column = 1 + prompt.Length;
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			
			Assert.AreEqual(">>> ab\r\n", textEditor.Text);
		}
		
		[Test]
		public void PreviousLineIsReadOnlyAfterEnterPressed()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseKeyPressEvent('b');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			console.Write(prompt, Style.Prompt);
			
			// Move up a line with cursor.
			textEditor.Line = 0;
			Assert.IsTrue(textEditor.RaiseKeyPressEvent('c'));
			
			string expectedText = ">>> ab\r\n" +
								">>> ";
			Assert.AreEqual(expectedText, textEditor.Text);
		}
	}
}
