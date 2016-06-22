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
using ICSharpCode.AvalonEdit.Document;
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
				var location = new SearchLocation(SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories, SearchOptions.SearchTarget == SearchTarget.CurrentSelection ? SearchManager.GetActiveSelection(true) : null);
				var strategy = SearchStrategyFactory.Create(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchMode);
				var result = SearchManager.FindNext(strategy, location);
				SearchManager.SelectResult(result);
			} else {
				Find find = new Find();
				find.Run();
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
