// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	[TestFixture]
	public class RubyConsoleTextEditorTests
	{
		RubyConsoleTextEditor consoleTextEditor;
		TextEditor avalonEditTextEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			avalonEditTextEditor = new TextEditor();
			consoleTextEditor = new RubyConsoleTextEditor(avalonEditTextEditor);
		}
		
		[Test]
		public void RubyConsoleTextEditorImplementsIScriptingConsoleTextEditorInterface()
		{
			Assert.IsNotNull(consoleTextEditor as IScriptingConsoleTextEditor);
		}
		
		[Test]
		public void MakeCurrentTextEditorContentReadOnlyExtendsReadOnlyRegionToEntireDocument()
		{
			avalonEditTextEditor.Text = String.Empty;
			consoleTextEditor.Write("abc" + Environment.NewLine);
			consoleTextEditor.MakeCurrentContentReadOnly();
			
			IReadOnlySectionProvider readOnlySection = avalonEditTextEditor.TextArea.ReadOnlySectionProvider;
			Assert.IsFalse(readOnlySection.CanInsert(2));
		}
		
		[Test]
		public void WriteMethodUpdatesTextEditor()
		{
			avalonEditTextEditor.Text = String.Empty;
			consoleTextEditor.Write("abc");
			Assert.AreEqual("abc", avalonEditTextEditor.Text);
		}
		
		[Test]
		public void ZeroBasedConsoleTextEditorColumnPositionIsOneLessThanAvalonTextEditorColumnPositionWhichIsOneBased()
		{
			avalonEditTextEditor.Text = "test";
			avalonEditTextEditor.TextArea.Caret.Column = 3;
			
			Assert.AreEqual(2, consoleTextEditor.Column);
		}
		
		[Test]
		public void SettingConsoleTextEditorColumnPositionUpdatesAvalonTextEditorColumnPosition()
		{
			avalonEditTextEditor.Text = "test";
			avalonEditTextEditor.TextArea.Caret.Column = 0;

			consoleTextEditor.Column = 1;
			Assert.AreEqual(2, avalonEditTextEditor.TextArea.Caret.Column);
		}
		
		[Test]
		public void GetSelectionStartAndLengthWhenThreeCharactersSelected()
		{
			avalonEditTextEditor.Text = "te000xt";
			int startOffset = 2;
			int endOffset = 5;
			SimpleSelection expectedSelection = new SimpleSelection(startOffset, endOffset);
			avalonEditTextEditor.SelectionStart = expectedSelection.StartOffset;
			avalonEditTextEditor.SelectionLength = expectedSelection.Length;
			
			// Sanity check.
			Assert.AreEqual("000", avalonEditTextEditor.SelectedText);
			
			AssertSelectionsAreEqual(expectedSelection, consoleTextEditor);
		}
		
		void AssertSelectionsAreEqual(SimpleSelection expectedSelection, IScriptingConsoleTextEditor consoleTextEditor)
		{
			int selectionLength = consoleTextEditor.SelectionStart + consoleTextEditor.SelectionLength;
			SimpleSelection actualSelection = new SimpleSelection(consoleTextEditor.SelectionStart, selectionLength);
			Assert.AreEqual(expectedSelection, actualSelection);
		}
		
		[Test]
		public void GetSelectionStartAndLengthWhenNothingSelected()
		{
			avalonEditTextEditor.Text = "text";
			avalonEditTextEditor.TextArea.Caret.Column = 1;
			avalonEditTextEditor.SelectionLength = 0;
			
			SimpleSelection expectedSelection = new SimpleSelection(1, 1);
			AssertSelectionsAreEqual(expectedSelection, consoleTextEditor);
		}

		[Test]
		public void ConsoleTextEditorLineEqualsOneLessThanAvalonTextEditorCaretLine()
		{
			avalonEditTextEditor.Text = "abc\r\ndef";
			avalonEditTextEditor.TextArea.Caret.Line = 2;
			Assert.AreEqual(1, consoleTextEditor.Line);		
		}
		
		[Test]
		public void ConsoleTextEditorTotalLinesEqualsAvalonTextEditorTotalLines()
		{
			avalonEditTextEditor.Text = "abc\r\ndef\r\nghi";
			Assert.AreEqual(3, consoleTextEditor.TotalLines);
		}
		
		[Test]
		public void GetLineForZerothLineReturnsFirstLineInAvalonTextEditor()
		{
			avalonEditTextEditor.Text = "abc\r\ndef\r\nghi";
			Assert.AreEqual("abc", consoleTextEditor.GetLine(0));
		}
		
		[Test]
		public void ReplaceTextReplacesTextInAvalonEditTextEditor()
		{
			avalonEditTextEditor.Text = "abc\r\ndef";
			avalonEditTextEditor.TextArea.Caret.Line = 2;
			
			int lineOffset = 1;
			int length = 1;
			consoleTextEditor.Replace(lineOffset, length, "test");
			Assert.AreEqual("abc\r\ndtestf", avalonEditTextEditor.Text);
		}
	}
}
