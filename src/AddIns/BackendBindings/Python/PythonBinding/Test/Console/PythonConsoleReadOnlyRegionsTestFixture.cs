// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests that the user cannot type into read-only regions of the text editor. The
	/// PythonConsole itself restricts typing itself by handling key press events.
	/// </summary>
	[TestFixture]
	public class PythonConsoleReadOnlyRegionsTestFixture
	{
		PythonConsole console;
		MockTextEditor textEditor;
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			console = new PythonConsole(textEditor, null);
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
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseKeyPressEvent('b');
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			textEditor.RaiseKeyPressEvent('c');
			
			Assert.AreEqual("acb", console.GetCurrentLine());
		}
		
		[Test]
		public void MoveOneCharacterIntoPromptTypingShouldBePrevented()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			textEditor.RaiseKeyPressEvent('a');
			
			Assert.AreEqual(String.Empty, console.GetCurrentLine());
		}

		[Test]
		public void MoveOneCharacterIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			textEditor.RaiseDialogKeyPressEvent(Keys.Back);
			
			Assert.AreEqual("a", console.GetCurrentLine());
			Assert.AreEqual(prompt + "a", textEditor.Text);
		}
		
		[Test]
		public void MoveTwoCharactersIntoPromptAndBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			textEditor.RaiseDialogKeyPressEvent(Keys.Back);
			
			Assert.AreEqual("a", console.GetCurrentLine());
			Assert.AreEqual(prompt + "a", textEditor.Text);
		}
		
		[Test]
		public void SelectLastCharacterOfPromptThenPressingTheBackspaceKeyShouldNotRemoveAnything()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.SelectionStart = prompt.Length - 1;
			textEditor.SelectionLength = 2;
			textEditor.Column += 2;
			textEditor.RaiseDialogKeyPressEvent(Keys.Back);
			
			Assert.AreEqual("a", console.GetCurrentLine());
			Assert.AreEqual(prompt + "a", textEditor.Text);
		}
		
		[Test]
		public void CanMoveIntoPromptRegionWithLeftCursorKey()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Left);
			Assert.IsFalse(textEditor.RaiseDialogKeyPressEvent(Keys.Left));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithRightCursorKey()
		{
			textEditor.Column = 0;
			Assert.IsFalse(textEditor.RaiseDialogKeyPressEvent(Keys.Right));
		}
		
		[Test]
		public void CanMoveOutOfPromptRegionWithUpCursorKey()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			console.Write(prompt, Style.Prompt);
			textEditor.Column = 0;
			Assert.IsFalse(textEditor.RaiseDialogKeyPressEvent(Keys.Up));
		}
		
		[Test]
		public void CanMoveInReadOnlyRegionWithDownCursorKey()
		{
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			console.Write(prompt, Style.Prompt);
			textEditor.Column = 0;
			textEditor.Line = 0;
			Assert.IsFalse(textEditor.RaiseDialogKeyPressEvent(Keys.Down));
		}		
		
		[Test]
		public void BackspaceKeyPressedIgnoredIfLineIsEmpty()
		{
			Assert.IsTrue(textEditor.RaiseDialogKeyPressEvent(Keys.Back));
		}
		
		[Test]
		public void BackspaceOnPreviousLine()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseKeyPressEvent('b');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);

			console.Write(prompt, Style.Prompt);
			
			textEditor.RaiseKeyPressEvent('c');
			
			// Move up a line with cursor.
			textEditor.Line = 0;
			
			Assert.IsTrue(textEditor.RaiseDialogKeyPressEvent(Keys.Back));
			Assert.AreEqual("c", console.GetCurrentLine());
		}		
	}
}
