// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

using ICSharpCode.NRefactory;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeConsoleTextEditor : IScriptingConsoleTextEditor
	{
		public bool IsDisposed;
		public bool IsAppendCalled;
		
		public bool IsShowCompletionWindowCalled;
		public bool IsMakeCurrentContentReadOnlyCalled;
		public ScriptingConsoleCompletionDataProvider CompletionProviderPassedToShowCompletionWindow;
		public string TextPassedToAppend;
		public string TextPassedToReplace;
		public int LengthPassedToReplace = -1;
		public int IndexPassedToReplace = -1;
		public bool IsColumnChangedBeforeTextWritten;
		
		public StringBuilder PreviousLines = new StringBuilder();
		public StringBuilder LineBuilder = new StringBuilder();
		
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public FakeConsoleTextEditor()
		{
			TotalLines = 1;
		}
		
		public void Dispose()
		{
			IsDisposed = true;
		}
		
		public void Append(string text)
		{
			TextPassedToAppend = text;
			IsAppendCalled = true;
			LineBuilder.Append(text);
			Column += text.Length;
		}
		
		public string Text {
			get { return PreviousLines.ToString() + LineBuilder.ToString(); }
			set {
				PreviousLines = new StringBuilder();
				LineBuilder = new StringBuilder();
				TotalLines = 1;
				foreach (char ch in value) {
					LineBuilder.Append(ch);
					if (ch == '\n') {
						TotalLines++;
						PreviousLines.Append(LineBuilder.ToString());
						LineBuilder = new StringBuilder();
					}
				}
				Column = LineBuilder.Length;
				SelectionStart = Column;
			}
		}
		
		public bool RaisePreviewKeyDownEvent(Key key, ModifierKeys modifiers = ModifierKeys.None)
		{
			FakeConsoleTextEditorKeyEventArgs e = new FakeConsoleTextEditorKeyEventArgs(key, modifiers);
			OnPreviewKeyDown(e);
			if (!e.Handled) {
				KeyConverter converter = new KeyConverter();
				string text = converter.ConvertToString(key);
				if (IsCursorAtEnd) {
					LineBuilder.Append(text);
				} else {
					LineBuilder.Insert(Column, text);
				}
				Column++;
				SelectionStart = Column;
			}
			return e.Handled;
		}
		
		void OnPreviewKeyDown(FakeConsoleTextEditorKeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, e);
			}
		}
		
		public void RaisePreviewKeyDownEvent(FakeConsoleTextEditorKeyEventArgs e)
		{
			OnPreviewKeyDown(e);
		}
		
		public bool RaisePreviewKeyDownEventForDialogKey(Key key)
		{
			FakeConsoleTextEditorKeyEventArgs e = new FakeConsoleTextEditorKeyEventArgs(key);
			OnPreviewKeyDown(e);
			if (!e.Handled) {
				switch (key) {
					case Key.Enter: {
						if (IsCursorAtEnd) {
							LineBuilder.Append(Environment.NewLine);
							PreviousLines.Append(LineBuilder.ToString());
							LineBuilder = new StringBuilder();
							Column = 0;
							SelectionStart = Column;
						} else {
							int length = LineBuilder.Length;
							string currentLine = LineBuilder.ToString();
							PreviousLines.Append(currentLine.Substring(0, Column) + Environment.NewLine);
							LineBuilder = new StringBuilder();
							LineBuilder.Append(currentLine.Substring(Column));
							Column = length - Column;
							SelectionStart = Column;
						}
						TotalLines++;
						Line++;
					}
					break;
					case Key.Back: {
						OnBackspaceKeyPressed();
					}
					break;
					case Key.Left: {
						Column--;
						SelectionStart = Column;
					}
					break;
					case Key.Right: {
						Column++;
						SelectionStart = Column;
					}
					break;
				}
			}
			return e.Handled;
		}
		
		public bool IsCompletionWindowDisplayed { get; set; }
		public int Column { get; set; }		
		public int SelectionStart { get; set; }
		public int SelectionLength { get; set; }
		public int Line { get; set; }
		public int TotalLines { get; set; }
		
		public string GetLine(int index)
		{
			if (index == TotalLines - 1) {
				return LineBuilder.ToString();
			}
			return "aaaa";
		}
		
		public void Replace(int index, int length, string text)
		{
			TextPassedToReplace = text;
			IndexPassedToReplace = index;
			LengthPassedToReplace = length;
			
			LineBuilder.Remove(index, length);
			LineBuilder.Insert(index, text);
		}
		
		public void ShowCompletionWindow(ScriptingConsoleCompletionDataProvider completionDataProvider)
		{
			IsShowCompletionWindowCalled = true;
			IsCompletionWindowDisplayed = true;
			this.CompletionProviderPassedToShowCompletionWindow = completionDataProvider;
		}
		
		public void MakeCurrentContentReadOnly()
		{
			IsMakeCurrentContentReadOnlyCalled = true;
		}
		
		bool IsCursorAtEnd {
			get { return Column == LineBuilder.ToString().Length; }
		}
		
		void OnBackspaceKeyPressed()
		{
			if (SelectionLength == 0) {
				// Remove a single character to the left of the cursor position.
				LineBuilder.Remove(Column - 1, 1);
			} else {
				LineBuilder.Remove(SelectionStart, SelectionLength);
			}
		}
		
		public bool IsScrollToEndCalled;
		
		public void ScrollToEnd()
		{
			IsScrollToEndCalled = true;
		}
		
		public int MaximumVisibleColumns;
	
		public int GetMaximumVisibleColumns()
		{
			return MaximumVisibleColumns;
		}
		
		public bool IsClearCalled;
		
		public void Clear()
		{
			IsClearCalled = true;
		}
	}
}
