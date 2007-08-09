// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public class Find : AbstractMenuCommand
	{
		public static void SetSearchPattern()
		{
			// Get Highlighted value and set it to FindDialog.searchPattern
			TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
			if (textArea != null) {
				string selectedText = textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
				if (selectedText != null && selectedText.Length > 0 && !IsMultipleLines(selectedText)) {
					SearchOptions.CurrentFindPattern = selectedText;
				}
			}
		}
		
		public override void Run()
		{
			SetSearchPattern();
			SearchAndReplaceDialog.ShowSingleInstance(SearchAndReplaceMode.Search);
		}
		
		public static bool IsMultipleLines(string text)
		{
			return text.IndexOf('\n') != -1;
		}
	}
	
	public class FindNext : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SearchOptions.CurrentFindPattern.Length > 0) {
				SearchReplaceManager.FindNext(null);
			} else {
				Find find = new Find();
				find.Run();
			}
		}
	}
	
	/// <summary>
	/// Finds the next text match based on the text currently
	/// selected. It uses the currently active search options and only changes
	/// the current find pattern. If the currently active search is inside the 
	/// current text selection then the quick find will change the search so it is
	/// across the active document, otherwise it will not change the current setting.
	/// </summary>
	/// <remarks>
	/// If there is a piece of text selected on a single line then the quick
	/// find will search for that. If multiple lines of text are selected then
	/// the word at the start of the selection is determined and searche for.
	/// If no text is selected then the word next to the caret is used. If
	/// no text is selected, but the caret is immediately surrounded by whitespace
	/// then quick find does nothing.
	/// </remarks>
	public class FindNextSelected : AbstractMenuCommand
	{
		public override void Run()
		{
			TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
			if (textArea == null) {
				return;
			}
			
			// Determine what text we should search for.
			string textToFind;
			
			string selectedText = textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
			if (selectedText.Length > 0) {
				if (Find.IsMultipleLines(selectedText)) {
					// Locate the nearest word at the selection start.
					textToFind = TextUtilities.GetWordAt(textArea.Document, textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].Offset);
				} else {
					// Search for selected text.
					textToFind = selectedText;		
				}
			} else {
				textToFind = TextUtilities.GetWordAt(textArea.Document, textArea.ActiveTextAreaControl.Caret.Offset);
			}
			
			if (textToFind != null && textToFind.Length > 0) {
				SearchOptions.CurrentFindPattern = textToFind;
				if (SearchOptions.DocumentIteratorType == DocumentIteratorType.CurrentSelection) {
					SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
				}
				SearchReplaceManager.FindNext(null);
			}
		}
	}
	
	public class Replace : AbstractMenuCommand
	{
		public override void Run()
		{
			Find.SetSearchPattern();
			SearchAndReplaceDialog.ShowSingleInstance(SearchAndReplaceMode.Replace);
		}
	}
}
