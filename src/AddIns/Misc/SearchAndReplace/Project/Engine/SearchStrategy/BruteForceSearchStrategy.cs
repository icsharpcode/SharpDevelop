// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using ICSharpCode.SharpDevelop.Editor;

namespace SearchAndReplace
{
	/// <summary>
	///  Only for fallback purposes.
	/// </summary>
	public class BruteForceSearchStrategy : ISearchStrategy
	{
		string searchPattern;
		
		bool MatchCaseSensitive(IDocument document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.TextLength || document.GetCharAt(offset + i) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool MatchCaseInsensitive(IDocument document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.TextLength || Char.ToUpper(document.GetCharAt(offset + i)) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool IsWholeWordAt(IDocument document, int offset, int length)
		{
			return (offset - 1 < 0 || !Char.IsLetterOrDigit(document.GetCharAt(offset - 1))) &&
			       (offset + length + 1 >= document.TextLength || !Char.IsLetterOrDigit(document.GetCharAt(offset + length)));
		}
		
		int InternalFindNext(ITextIterator textIterator)
		{
			while (textIterator.MoveAhead(1)) {
				if (SearchOptions.MatchCase ? MatchCaseSensitive(textIterator.Document, textIterator.Position, searchPattern) : MatchCaseInsensitive(textIterator.Document, textIterator.Position, searchPattern)) {
					if (!SearchOptions.MatchWholeWord || IsWholeWordAt(textIterator.Document, textIterator.Position, searchPattern.Length)) {
						return textIterator.Position;
					}
				}
			}
			return -1;
		}
		
		int InternalFindNext(ITextIterator textIterator, int offset, int length)
		{
			while (textIterator.MoveAhead(1)) {
				if (textIterator.Position >= offset + length) {
					textIterator.Position = offset;
				}
				if (SearchOptions.MatchCase ? MatchCaseSensitive(textIterator.Document, textIterator.Position, searchPattern) : MatchCaseInsensitive(textIterator.Document, textIterator.Position, searchPattern)) {
					if (!SearchOptions.MatchWholeWord || IsWholeWordAt(textIterator.Document, textIterator.Position, searchPattern.Length)) {
						if (TextSelection.IsInsideRange(textIterator.Position + searchPattern.Length - 1, offset, length)) {
							return textIterator.Position;
						} else {
							return -1;
						}
					}
				}
			}
			return -1;
		}
		
		public bool CompilePattern(ICSharpCode.SharpDevelop.Gui.IProgressMonitor monitor)
		{
			searchPattern = SearchOptions.MatchCase ? SearchOptions.FindPattern : SearchOptions.FindPattern.ToUpper();
			return true;
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator)
		{
			int offset = InternalFindNext(textIterator);
			return GetSearchResult(offset);
		}
		
		public SearchResultMatch FindNext(ITextIterator textIterator, int offset, int length)
		{
			int foundOffset = InternalFindNext(textIterator, offset, length);
			return GetSearchResult(foundOffset);
		}
		
		SearchResultMatch GetSearchResult(int offset)
		{
			return offset >= 0 ? new SearchResultMatch(offset, searchPattern.Length) : null;
		}
	}
}
