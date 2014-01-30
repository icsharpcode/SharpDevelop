// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
