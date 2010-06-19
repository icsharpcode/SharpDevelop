// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Text;

using ICSharpCode.RubyBinding;

namespace RubyBinding.Tests.Utils
{
	public class MockConsoleTextEditor : IConsoleTextEditor
	{
		StringBuilder previousLines = new StringBuilder();
		StringBuilder lineBuilder = new StringBuilder();
		bool writeCalled;
		int column;
		int selectionStart;
		int selectionLength;
		int line;
		int totalLines = 1;
		bool showCompletionWindowCalled;
		bool completionWindowDisplayed;
		bool makeReadOnlyCalled;
		RubyConsoleCompletionDataProvider completionProvider;
		
		public MockConsoleTextEditor()
		{
		}
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public void Write(string text)
		{
			writeCalled = true;
			lineBuilder.Append(text);
			column += text.Length;
		}
		
		public bool IsWriteCalled {
			get { return writeCalled; }
			set { writeCalled = value; }
		}

		public bool IsShowCompletionWindowCalled {
			get { return showCompletionWindowCalled; }
		}
		
		public bool IsMakeCurrentContentReadOnlyCalled {
			get { return makeReadOnlyCalled; }
		}
		
		/// <summary>
		/// Returns the code completion data provider passed to the ShowCompletionWindow method.
		/// </summary>
		public RubyConsoleCompletionDataProvider CompletionDataProvider {
			get { return completionProvider; }
		}

		public string Text {
			get { return previousLines.ToString() + lineBuilder.ToString(); }
			set {
				previousLines = new StringBuilder();
				lineBuilder = new StringBuilder();
				totalLines = 1;
				foreach (char ch in value) {
					lineBuilder.Append(ch);
					if (ch == '\n') {
						totalLines++;
						previousLines.Append(lineBuilder.ToString());
						lineBuilder = new StringBuilder();
					}
				}
				column = lineBuilder.Length;
				selectionStart = column;
			}
		}
		
		public bool RaisePreviewKeyDownEvent(Key key)
		{
			MockConsoleTextEditorKeyEventArgs e = new MockConsoleTextEditorKeyEventArgs(key);
			OnPreviewKeyDown(e);
			if (!e.Handled) {
				KeyConverter converter = new KeyConverter();
				string text = converter.ConvertToString(key);
				if (IsCursorAtEnd) {
					lineBuilder.Append(text);
				} else {
					lineBuilder.Insert(column, text);
				}
				column++;
				selectionStart = column;
			}
			return e.Handled;
		}
		
		void OnPreviewKeyDown(MockConsoleTextEditorKeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, e);
			}
		}
		
		public bool RaisePreviewKeyDownEventForDialogKey(Key key)
		{
			MockConsoleTextEditorKeyEventArgs e = new MockConsoleTextEditorKeyEventArgs(key);
			OnPreviewKeyDown(e);
			if (!e.Handled) {
				switch (key) {
					case Key.Enter: {
						if (IsCursorAtEnd) {
							lineBuilder.Append(Environment.NewLine);
							previousLines.Append(lineBuilder.ToString());
							lineBuilder = new StringBuilder();
							column = 0;
							selectionStart = column;
						} else {
							int length = lineBuilder.Length;
							string currentLine = lineBuilder.ToString();
							previousLines.Append(currentLine.Substring(0, column) + Environment.NewLine);
							lineBuilder = new StringBuilder();
							lineBuilder.Append(currentLine.Substring(column));
							column = length - column;
							selectionStart = column;
						}
						totalLines++;
						line++;
					}
					break;
					case Key.Back: {
						OnBackspaceKeyPressed();
					}
					break;
					case Key.Left: {
						column--;
						selectionStart = column;
					}
					break;
					case Key.Right: {
						column++;
						selectionStart = column;
					}
					break;
				}
			}
			return e.Handled;
		}
		
		public int Column {
			get { return column; }
			set { column = value; }
		}
		
		public int SelectionStart {
			get { return selectionStart; }
			set { selectionStart = value; }
		}
		
		public int SelectionLength {
			get { return selectionLength; }
			set { selectionLength = value; }
		}
		
		public int Line {
			get { return line; }
			set { line = value; }
		}

		public int TotalLines {
			get { return totalLines; }
		}
		
		public string GetLine(int index)
		{
			if (index == totalLines - 1) {
				return lineBuilder.ToString();
			}
			return "aaaa";
		}
		
		public void Replace(int index, int length, string text)
		{
			lineBuilder.Remove(index, length);
			lineBuilder.Insert(index, text);
		}
		
		public void ShowCompletionWindow(RubyConsoleCompletionDataProvider completionDataProvider)
		{
			showCompletionWindowCalled = true;
			completionWindowDisplayed = true;
			this.completionProvider = completionDataProvider;
		}
		
		public bool IsCompletionWindowDisplayed {
			get { return completionWindowDisplayed; }
			set { completionWindowDisplayed = value; }
		}
		
		public void MakeCurrentContentReadOnly()
		{
			makeReadOnlyCalled = true;
		}
		
		bool IsCursorAtEnd {
			get { return column == lineBuilder.ToString().Length; }
		}
		
		void OnBackspaceKeyPressed()
		{
			if (SelectionLength == 0) {
				// Remove a single character to the left of the cursor position.
				lineBuilder.Remove(Column - 1, 1);
			} else {
				lineBuilder.Remove(SelectionStart, SelectionLength);
			}
		}
	}
}
