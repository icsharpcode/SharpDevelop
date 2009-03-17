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
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.PythonBinding
{
	public class TextEditor : ITextEditor
	{
		delegate string GetLineInvoker(int index);
		delegate void WriteInvoker(string text, Color color);

		TextEditorControl textEditorControl;
		TextArea textArea;
		Color customLineColour = Color.LightGray;
		TextMarker readOnlyMarker;

		CodeCompletionWindow completionWindow;
		
		public TextEditor(TextEditorControl textEditorControl)
		{
			this.textEditorControl = textEditorControl;
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
				WriteInvoker invoker = new WriteInvoker(Write);
				textEditorControl.Invoke(invoker, new object[] {text, backgroundColour});
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
		
		/// <summary>
		/// Replaces the text at the specified index on the current line with the specified text.
		/// </summary>
		public void Replace(int index, int length, string text)
		{
			int currentLine = textEditorControl.ActiveTextAreaControl.Caret.Line;
			LineSegment lineSegment = textEditorControl.Document.GetLineSegment(currentLine);
			textEditorControl.Document.Replace(lineSegment.Offset + index, length, text);
		}
	
		/// <summary>
		/// Makes the current text read only. Text can still be entered at the end.
		/// </summary>
		public void MakeCurrentContentReadOnly()
		{
			IDocument doc = textEditorControl.Document;
			if (readOnlyMarker == null) {
				readOnlyMarker = new TextMarker(0, doc.TextLength, TextMarkerType.Invisible);
				readOnlyMarker.IsReadOnly = true;
				doc.MarkerStrategy.AddMarker(readOnlyMarker);
			}
			readOnlyMarker.Offset = 0;
			readOnlyMarker.Length = doc.TextLength;
			doc.UndoStack.ClearAll();
		}
		
		public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider)
		{
			completionWindow = CodeCompletionWindow.ShowCompletionWindow(textEditorControl.ParentForm, textEditorControl, String.Empty, completionDataProvider, ' ');
			if (completionWindow != null) {
				completionWindow.Closed += CompletionWindowClosed;
			}
		}
		
		public bool IsCompletionWindowDisplayed {
			get { return completionWindow != null; }
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
		
		void CompletionWindowClosed(object source, EventArgs e)
		{
			if (completionWindow != null) {
				completionWindow.Closed -= CompletionWindowClosed;
				completionWindow.Dispose();
				completionWindow = null;
			}
		}
	}
}
