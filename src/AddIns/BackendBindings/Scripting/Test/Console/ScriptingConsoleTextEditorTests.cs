// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	[RequiresSTA]
	public class ScriptingConsoleTextEditorTests
	{
		ScriptingConsoleTextEditor consoleTextEditor;
		TextEditor avalonEditTextEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			avalonEditTextEditor = new TextEditor();
			consoleTextEditor = new ScriptingConsoleTextEditor(avalonEditTextEditor);
		}
		
		[Test]
		public void InterfaceImplemented_NewInstance_ImplementsIScriptingConsoleTextEditorInterface()
		{
			Assert.IsNotNull(consoleTextEditor as IScriptingConsoleTextEditor);
		}
		
		[Test]
		public void MakeCurrentContentReadOnly_OneLineOfTextInTextEditor_ExtendsReadOnlyRegionToEntireDocument()
		{
			avalonEditTextEditor.Text = String.Empty;
			consoleTextEditor.Append("abc" + Environment.NewLine);
			consoleTextEditor.MakeCurrentContentReadOnly();
			
			IReadOnlySectionProvider readOnlySection = avalonEditTextEditor.TextArea.ReadOnlySectionProvider;
			bool canInsertInsideText = readOnlySection.CanInsert(2);
			Assert.IsFalse(canInsertInsideText);
		}
		
		[Test]
		public void Append_TextEditorHasNoText_UpdatesTextEditor()
		{
			avalonEditTextEditor.Text = String.Empty;
			consoleTextEditor.Append("abc");
			
			string text = avalonEditTextEditor.Text;
			string expectedText = "abc";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Append_CursorNotAtEndOfText_TextIsWrittenAtEnd()
		{
			avalonEditTextEditor.Text = "abc";
			avalonEditTextEditor.CaretOffset = 0;
			consoleTextEditor.Append("d");
			
			string text = avalonEditTextEditor.Text;
			string expectedText = "abcd";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Column_TextEditorColumnSetToThree_ZeroBasedConsoleTextEditorColumnPositionReturnsTwoWhichIsOneLessThanAvalonTextEditorColumnPositionWhichIsOneBased()
		{
			avalonEditTextEditor.Text = "test";
			avalonEditTextEditor.TextArea.Caret.Column = 3;
			
			int column = consoleTextEditor.Column;
			int expectedColumn = 2;
			
			Assert.AreEqual(expectedColumn, column);
		}
		
		[Test]
		public void Column_SettingConsoleTextEditorColumnPosition_UpdatesAvalonTextEditorColumnPosition()
		{
			avalonEditTextEditor.Text = "test";
			avalonEditTextEditor.TextArea.Caret.Column = 0;

			consoleTextEditor.Column = 1;
			int column = avalonEditTextEditor.TextArea.Caret.Column;
			int expectedColumn = 2;
			
			Assert.AreEqual(expectedColumn, column);
		}
		
		[Test]
		public void SelectionStart_ThreeCharactersSelectedInTextEditor_ConsoleTextEditorSelectionIsEqualToTextEditorSelection()
		{
			avalonEditTextEditor.Text = "te000xt";
			int startOffset = 2;
			int endOffset = 5;
			Selection expectedSelection = Selection.Create(avalonEditTextEditor.TextArea, startOffset, endOffset);
			avalonEditTextEditor.SelectionStart = expectedSelection.SurroundingSegment.Offset;
			avalonEditTextEditor.SelectionLength = expectedSelection.Length;
			
			// Sanity check.
			Assert.AreEqual("000", avalonEditTextEditor.SelectedText);
			
			AssertSelectionsAreEqual(expectedSelection, consoleTextEditor);
		}
		
		void AssertSelectionsAreEqual(Selection expectedSelection, IScriptingConsoleTextEditor consoleTextEditor)
		{
			int selectionEnd = consoleTextEditor.SelectionStart + consoleTextEditor.SelectionLength;
			Selection actualSelection = Selection.Create(avalonEditTextEditor.TextArea, consoleTextEditor.SelectionStart, selectionEnd);
			Assert.AreEqual(expectedSelection, actualSelection);
		}
		
		[Test]
		public void SelectionLength_NothingSelectedInTextEditor_ConsoleTextEditorSelectionMatchesTextEditorSelection()
		{
			avalonEditTextEditor.Text = "text";
			avalonEditTextEditor.TextArea.Caret.Column = 1;
			avalonEditTextEditor.SelectionLength = 0;
			
			Selection expectedSelection = Selection.Create(avalonEditTextEditor.TextArea, 1, 1);
			AssertSelectionsAreEqual(expectedSelection, consoleTextEditor);
		}

		[Test]
		public void Line_TextEditorCaretLineIsTwo_ConsoleTextEditorLineEqualsOneLessThanAvalonTextEditorCaretLine()
		{
			avalonEditTextEditor.Text = "abc\r\ndef";
			avalonEditTextEditor.TextArea.Caret.Line = 2;
			int line = consoleTextEditor.Line;
			int expectedLine = 1;
			Assert.AreEqual(expectedLine, line);		
		}
		
		[Test]
		public void Line_SettingConsoleTextEditorToLineOne_AvalonTextEditorCaretLineIsSetToTwo()
		{
			avalonEditTextEditor.Text = "abc\r\ndef";
			avalonEditTextEditor.TextArea.Caret.Line = 0;
			consoleTextEditor.Line = 1;
			int line = avalonEditTextEditor.TextArea.Caret.Line;
			int expectedLine = 2;
			Assert.AreEqual(expectedLine, line);		
		}
		
		[Test]
		public void TotalLines_ThreeLinesInTextEditor_ReturnsThreeLines()
		{
			avalonEditTextEditor.Text = "abc\r\ndef\r\nghi";
			int total = consoleTextEditor.TotalLines;
			int expected = 3;
			Assert.AreEqual(expected, total);
		}
		
		[Test]
		public void GetLine_GetLineForZerothLine_ReturnsFirstLineInAvalonTextEditor()
		{
			avalonEditTextEditor.Text = "abc\r\ndef\r\nghi";
			string line = consoleTextEditor.GetLine(0);
			string expectedLine = "abc";
			Assert.AreEqual(expectedLine, line);
		}
		
		[Test]
		public void Replace_ReplaceSingleCharacterOnSecondLine_ReplacesCorrectTextInAvalonEditTextEditor()
		{
			avalonEditTextEditor.Text = 
				"abc\r\n" +
				"def";
			avalonEditTextEditor.TextArea.Caret.Line = 2;
			
			int lineOffset = 1;
			int length = 1;
			consoleTextEditor.Replace(lineOffset, length, "test");
			
			string text = avalonEditTextEditor.Text;
			string expectedText = 
				"abc\r\n" +
				"dtestf";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Clear_TextEditorHasText_RemovesAllTextFromTextEditor()
		{
			avalonEditTextEditor.Text = "abc";
			consoleTextEditor.Clear();
			
			string text = avalonEditTextEditor.Text;
			string expectedText = String.Empty;
			
			Assert.AreEqual(expectedText, text);
		}
	}
}
