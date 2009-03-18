// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Gui.InsightWindow;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Interface for text editors.
	/// </summary>
	public interface ITextEditor : IServiceProvider
	{
		ICSharpCode.TextEditor.TextAreaControl ActiveTextAreaControl { get; }
		
		IDocument Document { get; }
		ITextEditorCaret Caret { get; }
		
		string FileName { get; }
		void ShowInsightWindow(IInsightDataProvider provider);
		void ShowCompletionWindow(ICompletionDataProvider provider, char ch);
		string GetWordBeforeCaret();
	}
	
	public interface ITextEditorCaret
	{
		/// <summary>
		/// Gets/Sets the caret offset;
		/// </summary>
		int Offset { get; set; }
		
		/// <summary>
		/// Gets/Sets the caret line number.
		/// Line numbers are counted starting from 0 (but this will change soon).
		/// </summary>
		int Line { get; set; }
		
		/// <summary>
		/// Gets/Sets the caret column number.
		/// Column numbers are counted starting from 0 (but this will change soon).
		/// </summary>
		int Column { get; set; }
		
		/// <summary>
		/// Gets/sets the caret position.
		/// </summary>
		Location Position { get; set; }
	}
}
