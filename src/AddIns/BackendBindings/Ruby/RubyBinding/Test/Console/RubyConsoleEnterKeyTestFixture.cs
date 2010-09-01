// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests that pressing the enter key in the middle of a typed in line in the Ruby console
	/// leaves the line alone and moves the cursor to the next line. By default the text editor
	/// will break the line and move the last part to the second line.
	/// </summary>
	[TestFixture]
	public class RubyConsoleEnterKeyTestFixture : RubyConsoleTestsBase
	{
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void EnterKeyDoesNotBreakUpExistingLine()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.SelectionStart = 1 + prompt.Length;
			MockConsoleTextEditor.Column = 1 + prompt.Length;
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			Assert.AreEqual(">>> AB\r\n", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void PreviousLineIsReadOnlyAfterEnterPressed()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableRubyConsole.Write(prompt, Style.Prompt);
			
			// Move up a line with cursor.
			MockConsoleTextEditor.Line = 0;
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			
			string expectedText = 
				">>> AB\r\n" +
				">>> ";
			Assert.AreEqual(expectedText, MockConsoleTextEditor.Text);
		}
	}
}
