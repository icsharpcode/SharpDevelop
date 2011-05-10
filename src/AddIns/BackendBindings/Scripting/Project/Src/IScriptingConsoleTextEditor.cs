// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
