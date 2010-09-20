// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using ICSharpCode.Scripting;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests that pressing the enter key in the middle of a typed in line in the console
	/// leaves the line alone and moves the cursor to the next line. By default the text editor
	/// will break the line and move the last part to the second line.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleEnterKeyTests : ScriptingConsoleTestsBase
	{
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			base.CreateConsole();
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}
		
		public void EnterKeyDoesNotBreakUpExistingLine()
 		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
 			FakeConsoleTextEditor.SelectionStart = 1 + prompt.Length;
 			FakeConsoleTextEditor.Column = 1 + prompt.Length;
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
 			
			Assert.AreEqual(">>> AB\r\n", FakeConsoleTextEditor.Text);
 		}
 		
 		[Test]
 		public void PreviousLineIsReadOnlyAfterEnterPressed()
 		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
 			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
 			
 			// Move up a line with cursor.
 			FakeConsoleTextEditor.Line = 0;
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
 			
			string expectedText = 
				">>> AB\r\n" +
				">>> ";
 			Assert.AreEqual(expectedText, FakeConsoleTextEditor.Text);
 		}
	}
}
