// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// The interface that the text editor used by the RubyConsole needs to be implement. Note that
	/// all the methods will be called on another thread not the main UI thread and will therefore need to
	/// be invoked.
	/// </summary>
	public interface IConsoleTextEditor
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
		/// Inserts text at the current cursor location.
		/// </summary>
		void Write(string text);
		
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
		int Line {get;}
		
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
		void ShowCompletionWindow(RubyConsoleCompletionDataProvider completionDataProvider);
	
		/// <summary>
		/// Indicates whether the completion window is currently being displayed.
		/// </summary>
		bool IsCompletionWindowDisplayed {get;}
		
		/// <summary>
		/// Makes the current text content read only. Text can be entered at the end.
		/// </summary>
		void MakeCurrentContentReadOnly();
	}
}
