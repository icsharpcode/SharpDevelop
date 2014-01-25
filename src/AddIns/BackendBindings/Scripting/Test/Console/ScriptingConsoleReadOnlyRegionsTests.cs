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
	/// Tests that the user cannot type into read-only regions of the text editor. The
	/// ScriptingConsole itself restricts typing itself by handling key press events.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleReadOnlyRegionsTests : ScriptingConsoleTestsBase
	{
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			base.CreateConsole();
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}
		
		[Test]
		public void MakeCurrentContentReadOnlyIsCalled()
		{
			Assert.IsTrue(FakeConsoleTextEditor.IsMakeCurrentContentReadOnlyCalled);
		}

		[Test]
		public void LeftArrowThenInsertNewCharacterInsertsText()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			
			Assert.AreEqual("ACB", TestableScriptingConsole.GetCurrentLine());
		}
		
		[Test]
		public void MoveOneCharacterIntoPromptTypingShouldBePrevented()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			
			Assert.AreEqual(String.Empty, TestableScriptingConsole.GetCurrentLine());
		}

		[Test]
		public void MoveOneCharacterIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableScriptingConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", FakeConsoleTextEditor.Text);
		}
		
		[Test]
		public void MoveTwoCharactersIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableScriptingConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", FakeConsoleTextEditor.Text);
		}
		
		[Test]
		public void SelectLastCharacterOfPromptThenPressingTheBackspaceKeyShouldNotRemoveAnything()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.SelectionStart = prompt.Length - 1;
			FakeConsoleTextEditor.SelectionLength = 2;
			FakeConsoleTextEditor.Column += 2;
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableScriptingConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", FakeConsoleTextEditor.Text);
		}
		
		[Test]
		public void CanMoveIntoPromptRegionWithLeftCursorKey()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.IsFalse(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithRightCursorKey()
		{
			FakeConsoleTextEditor.Column = 0;
			Assert.IsFalse(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Right));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithUpCursorKey()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			FakeConsoleTextEditor.Column = 0;
			Assert.IsFalse(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CanMoveInReadOnlyRegionWithDownCursorKey()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			FakeConsoleTextEditor.Column = 0;
			FakeConsoleTextEditor.Line = 0;
			Assert.IsFalse(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}		
		
		[Test]
		public void BackspaceKeyPressedIgnoredIfLineIsEmpty()
		{
			Assert.IsTrue(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
		}
		
		[Test]
		public void BackspaceOnPreviousLine()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);

			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
			
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			
			// Move up a line with cursor.
			FakeConsoleTextEditor.Line = 0;
			
			Assert.IsTrue(FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
			Assert.AreEqual("C", TestableScriptingConsole.GetCurrentLine());
		}
		
		[Test]
		public void CanBackspaceFirstCharacterOnLine()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.Column = 5;
			FakeConsoleTextEditor.SelectionStart = 5;
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual(String.Empty, TestableScriptingConsole.GetCurrentLine());
		}
		
		[Test]
		public void PreviewKeyDown_ControlCInReadOnlyRegion_HandledSetToFalseSoCopyInReadOnlyRegionAllowed()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			bool result = FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C, ModifierKeys.Control);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void PreviewKeyDown_ControlAInReadOnlyRegion_HandledSetToFalseSoSelectAllInReadOnlyRegionAllowed()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			bool result = FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A, ModifierKeys.Control);
			
			Assert.IsFalse(result);
		}
	}
}
