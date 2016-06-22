// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows.Input;

namespace ICSharpCode.Scripting
{
	/// <summary>
	/// The interface that the text editor used by the Python and Ruby Consoles needs to be implement. Note that
	/// all the methods will be called on another thread not the main UI thread and will therefore need to
	/// be invoked.
	/// </summary>
	public interface IScriptingConsoleTextEditor : IDisposable
	{
		/// <summary>
		/// Fired when a key is pressed but before any text has been added to the text editor.
		/// </summary>
		/// <remarks>
		/// The handler should set the ConsoleTextEditorKeyEventArgs.Handled to true if the text editor should not 
		/// process the key and not insert any text.
		/// </remarks>
		event ConsoleTextEditorKeyEventHandler PreviewKeyDown;
		
		/// <summary>
		/// Appends text to the end of the text editor.
		/// </summary>
		void Append(string text);
		
		/// <summary>
		/// Clears all the text in the text editor.
		/// </summary>
		void Clear();
		
		/// <summary>
		/// Replaces the text at the specified index on the current line with the specified text.
		/// </summary>
		void Replace(int index, int length, string text);
		
		/// <summary>
		/// Gets or sets the current column position of the cursor on the current line.  This is zero based.
		/// </summary>
		int Column {get; set;}
		
		/// <summary>
		/// Gets the length of the currently selected text.
		/// </summary>
		int SelectionLength {get;}
		
		/// <summary>
		/// Gets the start position of the currently selected text.
		/// </summary>
		int SelectionStart {get;}
		
		/// <summary>
		/// Gets the current line the cursor is on. This is zero based.
		/// </summary>
		int Line {get; set;}
		
		/// <summary>
		/// Gets the total number of lines in the text editor.
		/// </summary>
		int TotalLines {get;}
		
		/// <summary>
		/// Gets the text for the specified line.
		/// </summary>
		string GetLine(int index);
		
		/// <summary>
		/// Shows the code completion window.
		/// </summary>
		void ShowCompletionWindow(ScriptingConsoleCompletionDataProvider completionDataProvider);
	
		/// <summary>
		/// Indicates whether the completion window is currently being displayed.
		/// </summary>
		bool IsCompletionWindowDisplayed {get;}
		
		/// <summary>
		/// Makes the current text content read only. Text can be entered at the end.
		/// </summary>
		void MakeCurrentContentReadOnly();
		
		void ScrollToEnd();
		
		/// <summary>
		/// Returns the total number of columns/characters that are visible without scrolling.
		/// </summary>
		int GetMaximumVisibleColumns();
	}
}
