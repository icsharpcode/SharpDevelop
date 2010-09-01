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
	public class RubyConsoleReadOnlyRegionsTestFixture : RubyConsoleTestsBase
	{
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void MakeCurrentContentReadOnlyIsCalled()
		{
			Assert.IsTrue(MockConsoleTextEditor.IsMakeCurrentContentReadOnlyCalled);
		}

		[Test]
		public void LeftArrowThenInsertNewCharacterInsertsText()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			
			Assert.AreEqual("ACB", TestableRubyConsole.GetCurrentLine());
		}
		
		[Test]
		public void MoveOneCharacterIntoPromptTypingShouldBePrevented()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			
			Assert.AreEqual(String.Empty, TestableRubyConsole.GetCurrentLine());
		}

		[Test]
		public void MoveOneCharacterIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableRubyConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void MoveTwoCharactersIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableRubyConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void SelectLastCharacterOfPromptThenPressingTheBackspaceKeyShouldNotRemoveAnything()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.SelectionStart = prompt.Length - 1;
			MockConsoleTextEditor.SelectionLength = 2;
			MockConsoleTextEditor.Column += 2;
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual("A", TestableRubyConsole.GetCurrentLine());
			Assert.AreEqual(prompt + "A", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void CanMoveIntoPromptRegionWithLeftCursorKey()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.IsFalse(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithRightCursorKey()
		{
			MockConsoleTextEditor.Column = 0;
			Assert.IsFalse(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Right));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithUpCursorKey()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableRubyConsole.Write(prompt, Style.Prompt);
			MockConsoleTextEditor.Column = 0;
			Assert.IsFalse(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CanMoveInReadOnlyRegionWithDownCursorKey()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableRubyConsole.Write(prompt, Style.Prompt);
			MockConsoleTextEditor.Column = 0;
			MockConsoleTextEditor.Line = 0;
			Assert.IsFalse(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}		
		
		[Test]
		public void BackspaceKeyPressedIgnoredIfLineIsEmpty()
		{
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
		}
		
		[Test]
		public void BackspaceOnPreviousLine()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);

			TestableRubyConsole.Write(prompt, Style.Prompt);
			
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			
			// Move up a line with cursor.
			MockConsoleTextEditor.Line = 0;
			
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back));
			Assert.AreEqual("C", TestableRubyConsole.GetCurrentLine());
		}

		[Test]
		public void CanBackspaceFirstCharacterOnLine()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.Column = 5;
			MockConsoleTextEditor.SelectionStart = 5;
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			
			Assert.AreEqual(String.Empty, TestableRubyConsole.GetCurrentLine());
		}
	}
}
