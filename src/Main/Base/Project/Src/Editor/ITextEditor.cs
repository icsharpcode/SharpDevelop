// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.SharpDevelop.Editor
{
	public interface ITextEditorProvider : IFileDocumentProvider
	{
		ITextEditor TextEditor {
			get;
		}
	}
	
	/// <summary>
	/// Interface for text editors.
	/// </summary>
	public interface ITextEditor : IServiceProvider
	{
		/// <summary>
		/// Gets the primary view if split-view is active.
		/// If split-view is disabled, the current ITextEditor instance is returned.
		/// This property never returns null.
		/// </summary>
		/// <example>bool isSecondaryView = (editor != editor.PrimaryView);</example>
		ITextEditor PrimaryView { get; }
		
		/// <summary>
		/// Gets the document that is being edited.
		/// </summary>
		IDocument Document { get; }
		
		/// <summary>
		/// Gets an object that represents the caret inside this text editor.
		/// This property never returns null.
		/// </summary>
		ITextEditorCaret Caret { get; }
		
		/// <summary>
		/// Gets the set of options used in the text editor.
		/// This property never returns null.
		/// </summary>
		ITextEditorOptions Options { get; }
		
		/// <summary>
		/// Gets the language binding attached to this text editor.
		/// This property never returns null.
		/// </summary>
		ILanguageBinding Language { get; }
		
		/// <summary>
		/// Gets the start offset of the selection.
		/// </summary>
		int SelectionStart { get; }
		
		/// <summary>
		/// Gets the length of the selection.
		/// </summary>
		int SelectionLength { get; }
		
		/// <summary>
		/// Gets/Sets the selected text.
		/// </summary>
		string SelectedText { get; set; }
		
		/// <summary>
		/// Sets the selection.
		/// </summary>
		/// <param name="selectionStart">Start offset of the selection</param>
		/// <param name="selectionLength">Length of the selection</param>
		void Select(int selectionStart, int selectionLength);
		
		/// <summary>
		/// Is raised when the selection changes.
		/// </summary>
		event EventHandler SelectionChanged;
		
		/// <summary>
		/// Is raised before a key is pressed.
		/// </summary>
		event KeyEventHandler KeyPress;
		
		/// <summary>
		/// Sets the caret to the specified line/column and brings the caret into view.
		/// </summary>
		void JumpTo(int line, int column);
		
		FileName FileName { get; }
		
		ICompletionListWindow ShowCompletionWindow(ICompletionItemList data);
		
		/// <summary>
		/// Gets the completion window that is currently open.
		/// </summary>
		ICompletionListWindow ActiveCompletionWindow { get; }
		
		/// <summary>
		/// Open a new insight window showing the specified insight items.
		/// </summary>
		/// <param name="items">The insight items to show in the window.
		/// If this property is null or an empty list, the insight window will not be shown.</param>
		/// <returns>The insight window; or null if no insight window was opened.</returns>
		IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items);
		
		/// <summary>
		/// Gets the insight window that is currently open.
		/// </summary>
		IInsightWindow ActiveInsightWindow { get; }
		
		/// <summary>
		/// Gets the list of available code snippets.
		/// </summary>
		IEnumerable<ICompletionItem> GetSnippets();
	}
	
	public interface ITextEditorOptions : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the text used for one indentation level.
		/// </summary>
		string IndentationString { get; }
		
		/// <summary>
		/// Gets whether a '}' should automatically be inserted when a block is opened.
		/// </summary>
		bool AutoInsertBlockEnd { get; }
		
		/// <summary>
		/// Gets if tabs should be converted to spaces.
		/// </summary>
		bool ConvertTabsToSpaces { get; }
		
		/// <summary>
		/// Gets the size of an indentation level.
		/// </summary>
		int IndentationSize { get; }
		
		/// <summary>
		/// Gets the column of the vertical ruler (line that signifies the maximum line length
		/// defined by the coding style)
		/// This property returns a valid value even if the vertical ruler is set to be invisible.
		/// </summary>
		int VerticalRulerColumn { get; }
		
		/// <summary>
		/// Gets whether errors should be underlined.
		/// </summary>
		bool UnderlineErrors { get; }
		
		/// <summary>
		/// Gets the name of the currently used font.
		/// </summary>
		string FontFamily { get; }
	}
	
	public interface ITextEditorCaret
	{
		/// <summary>
		/// Gets/Sets the caret offset;
		/// </summary>
		int Offset { get; set; }
		
		/// <summary>
		/// Gets/Sets the caret line number.
		/// Line numbers are counted starting from 1.
		/// </summary>
		int Line { get; set; }
		
		/// <summary>
		/// Gets/Sets the caret column number.
		/// Column numbers are counted starting from 1.
		/// </summary>
		int Column { get; set; }
		
		/// <summary>
		/// Gets/sets the caret position.
		/// </summary>
		Location Position { get; set; }
		
		/// <summary>
		/// Is raised whenever the position of the caret has changed.
		///	</summary>
		event EventHandler PositionChanged;
	}
}
