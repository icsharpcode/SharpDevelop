// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
