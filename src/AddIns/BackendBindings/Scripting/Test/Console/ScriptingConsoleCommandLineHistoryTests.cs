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
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the ScriptingConsole's command line history.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleCommandLineHistoryTestFixture : ScriptingConsoleTestsBase
	{
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			base.CreateConsole();
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}

		[Test]
		public void UpArrowKeyPressed()
		{
			Assert.IsTrue(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CurrentLineAfterUpArrowKeyPressed()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual("BC", TestableScriptingConsole.GetCurrentLine());
		}
		
		[Test]
		public void TextEditorCursorIsAtEndOfLineAfterUpArrowKeyPressed()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 2, FakeConsoleTextEditor.Column);
		}
		
		[Test]
		public void TextAfterUpArrowKeyPressedTwiceThenDownArrowKey()
		{
			UpArrowKeyPressedTwiceThenDownArrowKey();
			Assert.AreEqual("BC", TestableScriptingConsole.GetCurrentLine());
		}

		[Test]
		public void TextEditorCursorAfterUpArrowKeyPressedTwice()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 1, FakeConsoleTextEditor.Column);
		}
		
		[Test]
		public void DownArrowKeyHandled()
		{
			Assert.IsTrue(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}
		
		void UpArrowKeyPressedTwiceThenDownArrowKey()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down);
		}
	}
}
