// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Document;

namespace RubyBinding.Tests.Console
{
	public class MockTextEditor : ITextEditor
	{
		IndentStyle indentStyle = IndentStyle.Auto;
		StringBuilder previousLines = new StringBuilder();
		StringBuilder lineBuilder = new StringBuilder();
		bool writeCalled;
		int column;
		int selectionStart;
		int selectionLength;
		int line;
		int totalLines = 1;
		List<Color> textColors = new List<Color>();
		bool showCompletionWindowCalled;
		bool completionWindowDisplayed;
		bool makeReadOnlyCalled;
		ICompletionDataProvider	completionProvider;
		
		public MockTextEditor()
		{
		}
		
		public event ICSharpCode.TextEditor.KeyEventHandler KeyPress;
		public event DialogKeyProcessor DialogKeyPress;
		
		public IndentStyle IndentStyle {
			get { return indentStyle; }
			set { indentStyle = value; }
		}
		
		public void Write(string text)
		{
			writeCalled = true;
			lineBuilder.Append(text);
			column += text.Length;
		}
		
		public void Write(string text, Color backgroundColor)
		{
			textColors.Add(backgroundColor);
			Write(text);
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
		public ICompletionDataProvider CompletionDataProvider {
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
		
		public bool RaiseKeyPressEvent(char ch)
		{
			bool keyHandled = KeyPress(ch);
			if (!keyHandled) {
				if (IsCursorAtEnd) {
					lineBuilder.Append(ch);
				} else {
					lineBuilder.Insert(column, ch);
				}
				column++;
				selectionStart = column;
			}
			return keyHandled;
		}
		
		/// <summary>
		/// Calls RaiseKeyPressEvent for each character in the string.
		/// </summary>
		public void RaiseKeyPressEvents(string text)
		{
			foreach (char ch in text) {
				RaiseKeyPressEvent(ch);
			}
		}

		public bool RaiseDialogKeyPressEvent(Keys keyData)
		{
			bool keyHandled = DialogKeyPress(keyData);
			if (!keyHandled) {
				switch (keyData) {
					case Keys.Enter: {
						if (!KeyPress('\n')) {
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
					}
					break;
					case Keys.Back: {
						OnBackspaceKeyPressed();
					}
					break;
					case Keys.Left: {
						column--;
						selectionStart = column;
					}
					break;
					case Keys.Right: {
						column++;
						selectionStart = column;
					}
					break;
				}
			}
			return keyHandled;
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
		
		public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider)
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
		
		public List<Color> WrittenTextColors {
			get { return textColors; }
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
