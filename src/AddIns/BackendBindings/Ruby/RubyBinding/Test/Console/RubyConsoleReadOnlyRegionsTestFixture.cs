// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

using ICSharpCode.RubyBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests that the user cannot type into read-only regions of the text editor. The
	/// RubyConsole itself restricts typing itself by handling key press events.
	/// </summary>
	[TestFixture]
	public class RubyConsoleReadOnlyRegionsTestFixture
	{
		RubyConsole console;
		MockConsoleTextEditor textEditor;
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			console = new RubyConsole(textEditor, null);
			console.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void MakeCurrentContentReadOnlyIsCalled()
		{
			Assert.IsTrue(textEditor.IsMakeCurrentContentReadOnlyCalled);
		}

		[Test]
		public void LeftArrowThenInsertNewCharacterInsertsText()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEvent(Key.B);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			textEditor.RaisePreviewKeyDownEvent(Key.C);
			
			Assert.AreEqual("ACB", console.GetCurrentLine());
		}
		
		[Test]
		public void MoveOneCharacterIntoPromptTypingShouldBePrevented()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			
			Assert.AreEqual(String.Empty, console.GetCurrentLine());
		}

		[Test]
		public void MoveOneCharacterIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", console.GetCurrentLine());
			Assert.AreEqual(prompt + "A", textEditor.Text);
		}
		
		[Test]
		public void MoveTwoCharactersIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", console.GetCurrentLine());
			Assert.AreEqual(prompt + "A", textEditor.Text);
		}
		
		[Test]
		public void SelectLastCharacterOfPromptThenPressingTheBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.SelectionStart = prompt.Length - 1;
			textEditor.SelectionLength = 2;
			textEditor.Column += 2;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", console.GetCurrentLine());
			Assert.AreEqual(prompt + "A", textEditor.Text);
		}
		
		[Test]
		public void CanMoveIntoPromptRegionWithLeftCursorKey()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.IsFalse(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithRightCursorKey()
		{
			textEditor.Column = 0;
			Assert.IsFalse(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Right));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithUpCursorKey()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			console.Write(prompt, Style.Prompt);
			textEditor.Column = 0;
			Assert.IsFalse(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CanMoveInReadOnlyRegionWithDownCursorKey()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			console.Write(prompt, Style.Prompt);
			textEditor.Column = 0;
			textEditor.Line = 0;
			Assert.IsFalse(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}		
		
		[Test]
		public void BackspaceKeyPressedIgnoredIfLineIsEmpty()
		{
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
		}
		
		[Test]
		public void BackspaceOnPreviousLine()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEvent(Key.B);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);

			console.Write(prompt, Style.Prompt);
			
			textEditor.RaisePreviewKeyDownEvent(Key.C);
			
			// Move up a line with cursor.
			textEditor.Line = 0;
			
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
			Assert.AreEqual("C", console.GetCurrentLine());
		}

		[Test]
		public void CanBackspaceFirstCharacterOnLine()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.Column = 5;
			textEditor.SelectionStart = 5;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual(String.Empty, console.GetCurrentLine());
		}
	}
}
