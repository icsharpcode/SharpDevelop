// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;

using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;

namespace RubyBinding.Tests.Utils
{
	public class MockConsoleTextEditor : IConsoleTextEditor
	{
		public bool IsDisposed;
		public bool IsWriteCalled;
		
		public bool IsShowCompletionWindowCalled;
		public bool IsMakeCurrentContentReadOnlyCalled;
		public RubyConsoleCompletionDataProvider CompletionProviderPassedToShowCompletionWindow;
		public string TextPassedToWrite;
		public string TextPassedToReplace;
		public int LengthPassedToReplace = -1;
		public int IndexPassedToReplace = -1;
		public Location CursorLocationWhenWriteTextCalled;
		public bool IsColumnChangedBeforeTextWritten;
		
		public StringBuilder PreviousLines = new StringBuilder();
		public StringBuilder LineBuilder = new StringBuilder();
				
		public event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		public MockConsoleTextEditor()
		{
			TotalLines = 1;
		}
				
		public void Dispose()
		{
			IsDisposed = true;
		}
		
		public void Write(string text)
		{
			TextPassedToWrite = text;
			CursorLocationWhenWriteTextCalled = new Location(Column, Line);
			IsWriteCalled = true;
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
		
		public bool RaisePreviewKeyDownEvent(Key key)
		{
			MockConsoleTextEditorKeyEventArgs e = new MockConsoleTextEditorKeyEventArgs(key);
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
		
		void OnPreviewKeyDown(MockConsoleTextEditorKeyEventArgs e)
		{
			if (PreviewKeyDown != null) {
				PreviewKeyDown(this, e);
			}
		}
		
		public void RaisePreviewKeyDownEvent(MockConsoleTextEditorKeyEventArgs e)
		{
			OnPreviewKeyDown(e);
		}
		
		public bool RaisePreviewKeyDownEventForDialogKey(Key key)
		{
			MockConsoleTextEditorKeyEventArgs e = new MockConsoleTextEditorKeyEventArgs(key);
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
		
		public void ShowCompletionWindow(RubyConsoleCompletionDataProvider completionDataProvider)
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
	}
}
