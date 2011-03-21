// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleTextEditor : IScriptingConsoleTextEditor
	{
		TextEditor textEditor;
		Color customLineColour = Color.LightGray;
		BeginReadOnlySectionProvider readOnlyRegion;
		CompletionWindow completionWindow;
		
		public ScriptingConsoleTextEditor(TextEditor textEditor)
		{
			this.textEditor = textEditor;
			readOnlyRegion = new BeginReadOnlySectionProvider();
			textEditor.TextArea.ReadOnlySectionProvider = readOnlyRegion;
			textEditor.PreviewKeyDown += OnTextEditorPreviewKeyDown;
		}
		
		void OnTextEditorPreviewKeyDown(object source, KeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, new ScriptingConsoleTextEditorKeyEventArgs(e));
			}
		}
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public void Dispose()
		{
			textEditor.PreviewKeyDown -= OnTextEditorPreviewKeyDown;
		}
		
		public Color CustomLineColour {
			get { return customLineColour; }
		}
		
		public void Write(string text)
		{
			TextLocation location = GetCurrentCursorLocation();
			int offset = textEditor.Document.GetOffset(location);
			textEditor.Document.Insert(offset, text);
		}
		
		TextLocation GetCurrentCursorLocation()
		{
			return new TextLocation(Line + 1, Column + 1);
		}
		
		public int Column {
			get { return textEditor.TextArea.Caret.Column - 1; }
			set { textEditor.TextArea.Caret.Column = value + 1; }
		}
		
		public int SelectionStart {
			get { return textEditor.SelectionStart; }
		}
		
		public int SelectionLength {
			get { return textEditor.SelectionLength; }
		}

		public int Line {
			get { return textEditor.TextArea.Caret.Line - 1; }
			set { textEditor.TextArea.Caret.Line = value + 1; }
		}

		/// <summary>
		/// Gets the total number of lines in the text editor.
		/// </summary>
		public int TotalLines {
			get { return textEditor.Document.LineCount; }
		}

		/// <summary>
		/// Gets the text for the specified line.
		/// </summary>
		public string GetLine(int index)
		{
			DocumentLine line = textEditor.Document.GetLineByNumber(index + 1);
			return textEditor.Document.GetText(line);
		}
		
		/// <summary>
		/// Replaces the text at the specified index on the current line with the specified text.
		/// </summary>
		public void Replace(int index, int length, string text)
		{
			DocumentLine line = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
			int offset = line.Offset + index;
			textEditor.Document.Replace(offset, length, text);
		}
				
		/// <summary>
		/// Makes the current text read only. Text can still be entered at the end.
		/// </summary>
		public void MakeCurrentContentReadOnly()
		{
			readOnlyRegion.EndOffset = textEditor.Document.TextLength;
		}
		
		public void ShowCompletionWindow(ScriptingConsoleCompletionDataProvider completionDataProvider)
		{
			ICompletionData[] items = completionDataProvider.GenerateCompletionData(this);
			if (items.Length > 0) {
				ShowCompletionWindow(items);
			}
		}
		
		void ShowCompletionWindow(ICompletionData[] items)
		{
			completionWindow = new CompletionWindow(textEditor.TextArea);
			completionWindow.Closed += CompletionWindowClosed;
			foreach (ICompletionData item in items) {
				completionWindow.CompletionList.CompletionData.Add(item);
			}
			completionWindow.ExpectInsertionBeforeStart = true;
			completionWindow.Show();
		}
		
		void ShowCompletionWindow(CompletionWindow window)
		{
			if (completionWindow == window) {
				window.Show();
			}
		}
		
		public bool IsCompletionWindowDisplayed {
			get { return completionWindow != null; }
		}	
		
		void CompletionWindowClosed(object source, EventArgs e)
		{
			if (completionWindow != null) {
				completionWindow.Closed -= CompletionWindowClosed;
				completionWindow = null;
			}
		}
		
		public void ScrollToEnd()
		{
			textEditor.ScrollToEnd();
		}
	}
}
