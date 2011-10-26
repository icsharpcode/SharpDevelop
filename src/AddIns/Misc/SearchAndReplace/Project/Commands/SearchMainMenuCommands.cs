// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace SearchAndReplace
{
	public class Find : AbstractMenuCommand
	{
		public static void SetSearchPattern()
		{
			// Get Highlighted value and set it to FindDialog.searchPattern
			ITextEditor textArea = SearchManager.GetActiveTextEditor();
			if (textArea != null) {
				string selectedText = textArea.SelectedText;
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
				var result = SearchManager.FindNext(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchMode, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
				SearchManager.SelectResult(result);
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
			ITextEditor textArea = SearchManager.GetActiveTextEditor();
			if (textArea == null) {
				return;
			}
			
			// Determine what text we should search for.
			string textToFind;
			
			string selectedText = textArea.SelectedText;
			if (selectedText.Length > 0) {
				if (Find.IsMultipleLines(selectedText)) {
					// Locate the nearest word at the selection start.
					textToFind = textArea.Document.GetWordAt(textArea.SelectionStart);
				} else {
					// Search for selected text.
					textToFind = selectedText;
				}
			} else {
				textToFind = textArea.Document.GetWordAt(textArea.Caret.Offset);
			}
			
			if (textToFind != null && textToFind.Length > 0) {
				SearchOptions.CurrentFindPattern = textToFind;
				if (SearchOptions.SearchTarget == SearchTarget.CurrentSelection) {
					SearchOptions.SearchTarget = SearchTarget.CurrentDocument;
				}
				var result = SearchManager.FindNext(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchMode, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
				SearchManager.SelectResult(result);
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
