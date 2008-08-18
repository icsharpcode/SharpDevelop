// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	public class TextEditor : ITextEditor
	{
		delegate string GetLineInvoker(int index);

		TextEditorControl textEditorControl;
		TextArea textArea;
		Color customLineColour = Color.LightGray;
		
		public TextEditor(TextEditorControl textEditorControl)
		{
			this.textEditorControl = textEditorControl;
			this.textEditorControl.Document.LineCountChanged += LineCountChanged;
			this.textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			textEditorControl.TextEditorProperties.SupportReadOnlySegments = true;
		}
		
		public IndentStyle IndentStyle {
			get { return textEditorControl.IndentStyle; }
			set { SetIndentStyle(value); }
		}
		
		public event KeyEventHandler KeyPress { 
			add { textArea.KeyEventHandler += value; }
			remove { textArea.KeyEventHandler -= value; }
		}
		
		public event DialogKeyProcessor DialogKeyPress {
			add { textArea.DoProcessDialogKey += value; }
			remove { textArea.DoProcessDialogKey -= value; }
		}
		
		public Color CustomLineColour {
			get { return customLineColour; }
		}
		
		public void Write(string text)
		{
			Write(text, Color.Empty);
		}
		
		public void Write(string text, Color backgroundColour)
		{
			if (textEditorControl.InvokeRequired) {
				Action<string, Color> action = Write;
				textEditorControl.Invoke(action, new object[] {text, backgroundColour});
			} else {
				int offset = textEditorControl.Document.PositionToOffset(new TextLocation(Column, Line));
				textEditorControl.ActiveTextAreaControl.TextArea.InsertString(text);
				
				if (!backgroundColour.IsEmpty) {
					TextMarker marker = new TextMarker(offset, text.Length, TextMarkerType.SolidBlock, backgroundColour);
					textEditorControl.Document.MarkerStrategy.AddMarker(marker);
					textEditorControl.Refresh();
				}
			}
		}

		void AddCustomLine(int line)
		{
			//textEditorControl.Document.CustomLineManager.AddCustomLine(line, customLineColour, false);
		}
		
		public int Column {
			get { return textEditorControl.ActiveTextAreaControl.Caret.Column; }
			set { textEditorControl.ActiveTextAreaControl.Caret.Column = value; }
		}
		
		public int SelectionStart {
			get { 
				ColumnRange range = GetSelectionRange();
				if (range != ColumnRange.NoColumn) {
					return range.StartColumn;
				}
				return Column;
			}
		}
		
		public int SelectionLength {
			get {
				ColumnRange range = GetSelectionRange();
				return range.EndColumn - range.StartColumn;
			}
		}

		/// <summary>
		/// Gets the current cursor line.
		/// </summary>
		public int Line {
			get { return textArea.Caret.Line; }
		}

		/// <summary>
		/// Gets the total number of lines in the text editor.
		/// </summary>
		public int TotalLines {
			get { return textEditorControl.Document.TotalNumberOfLines; }
		}

		/// <summary>
		/// Gets the text for the specified line.
		/// </summary>
		public string GetLine(int index)
		{
			if (textEditorControl.InvokeRequired) {
				GetLineInvoker invoker = new GetLineInvoker(GetLine);
				return (string)textEditorControl.Invoke(invoker, new object[] {index});
			} else {
				LineSegment lineSegment = textEditorControl.Document.GetLineSegment(index);
				return textEditorControl.Document.GetText(lineSegment);
			}
		}
		
		public void ShowCompletionWindow()
		{
		}
		
		/// <summary>
		/// Gets the range of the currently selected text.
		/// </summary>
		ColumnRange GetSelectionRange()
		{
			return textArea.SelectionManager.GetSelectionAtLine(textArea.Caret.Line);
		}
		
		void SetIndentStyle(IndentStyle style)
		{
			if (textEditorControl.InvokeRequired) {
				Action <IndentStyle> action = SetIndentStyle;
				textEditorControl.Invoke(action, new object[] {style});
			} else {
				textEditorControl.IndentStyle = style;
			}
		}
		
		void LineCountChanged(object source, LineCountChangeEventArgs e)
		{
			IDocument doc = textEditorControl.Document;
			int totalLines = doc.TotalNumberOfLines;
			//doc.CustomLineManager.Clear();
			for (int line = 0; line < totalLines - 1; ++line) {
			//	doc.CustomLineManager.AddCustomLine(line, customLineColour, false);
			}
		}
	}
}
