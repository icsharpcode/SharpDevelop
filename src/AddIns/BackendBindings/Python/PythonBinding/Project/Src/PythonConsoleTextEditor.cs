// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsoleTextEditor : IConsoleTextEditor
	{
		delegate string GetLineInvoker(int index);

		TextEditor textEditor;
		Color customLineColour = Color.LightGray;
		BeginReadOnlySectionProvider readOnlyRegion;
		IControlDispatcher dispatcher;
		CompletionWindow completionWindow;
		
		public PythonConsoleTextEditor(TextEditor textEditor)
			: this(textEditor, new ControlDispatcher(textEditor))
		{
		}
		
		public PythonConsoleTextEditor(TextEditor textEditor, IControlDispatcher dispatcher)
		{
			this.textEditor = textEditor;
			this.dispatcher = dispatcher;
			readOnlyRegion = new BeginReadOnlySectionProvider();
			textEditor.TextArea.ReadOnlySectionProvider = readOnlyRegion;
			textEditor.PreviewKeyDown += OnTextEditorPreviewKeyDown;
		}
		
		void OnTextEditorPreviewKeyDown(object source, KeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, new PythonConsoleTextEditorKeyEventArgs(e));
			}
		}
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public Color CustomLineColour {
			get { return customLineColour; }
		}
		
		public void Write(string text)
		{
			if (dispatcher.CheckAccess()) {
				TextLocation location = GetCurrentCursorLocation();
				int offset = textEditor.Document.GetOffset(location);
				textEditor.Document.Insert(offset, text);
			} else {
				Action<string> action = Write;
				dispatcher.Invoke(action, new object[] {text});
			}
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

		/// <summary>
		/// Gets the current cursor line.
		/// </summary>
		public int Line {
			get { return textEditor.TextArea.Caret.Line - 1; }
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
			if (dispatcher.CheckAccess()) {
				DocumentLine line = textEditor.Document.GetLineByNumber(index + 1);
				return textEditor.Document.GetText(line);
			} else {
				GetLineInvoker invoker = new GetLineInvoker(GetLine);
				return (string)dispatcher.Invoke(invoker, new object[] {index});
			}
		}
		
		/// <summary>
		/// Replaces the text at the specified index on the current line with the specified text.
		/// </summary>
		public void Replace(int index, int length, string text)
		{
			if (dispatcher.CheckAccess()) {
				DocumentLine line = textEditor.Document.GetLineByNumber(textEditor.TextArea.Caret.Line);
				int offset = line.Offset + index;
				textEditor.Document.Replace(offset, length, text);
			} else {
				Action<int, int, string> action = Replace;
				dispatcher.Invoke(action, new object[] {index, length, text});
			}
		}
	
		/// <summary>
		/// Makes the current text read only. Text can still be entered at the end.
		/// </summary>
		public void MakeCurrentContentReadOnly()
		{
			if (dispatcher.CheckAccess()) {
				readOnlyRegion.EndOffset = textEditor.Document.TextLength;
			} else {
				Action action = MakeCurrentContentReadOnly;
				dispatcher.Invoke(action);
			}
		}
		
		public void ShowCompletionWindow(PythonConsoleCompletionDataProvider completionDataProvider)
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
			Action<CompletionWindow> action = ShowCompletionWindow;
			completionWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action, completionWindow);
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
	}
}
