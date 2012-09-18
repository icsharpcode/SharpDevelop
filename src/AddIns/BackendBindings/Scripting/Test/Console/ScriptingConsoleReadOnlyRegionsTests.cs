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
