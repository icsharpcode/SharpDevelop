// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	/// <summary>
	///  Only for fallback purposes.
	/// </summary>
	public class BruteForceSearchStrategy : ISearchStrategy
	{
		string searchPattern;
		
		bool MatchCaseSensitive(ITextBufferStrategy document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.Length || document.GetCharAt(offset + i) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool MatchCaseInsensitive(ITextBufferStrategy document, int offset, string pattern)
		{
			for (int i = 0; i < pattern.Length; ++i) {
				if (offset + i >= document.Length || Char.ToUpper(document.GetCharAt(offset + i)) != pattern[i]) {
					return false;
				}
			}
			return true;
		}
		
		bool IsWholeWordAt(ITextBufferStrategy document, int offset, int length)
		{
			return (offset - 1 < 0 || !Char.IsLetterOrDigit(document.GetCharAt(offset - 1))) &&
			       (offset + length + 1 >= document.Length || !Char.IsLetterOrDigit(document.GetCharAt(offset + length)));
		}
		
		int InternalFindNext(ITextIterator textIterator)
		{
			while (textIterator.MoveAhead(1)) {
				if (SearchOptions.MatchCase ? MatchCaseSensitive(textIterator.TextBuffer, textIterator.Position, searchPattern) : MatchCaseInsensitive(textIterator.TextBuffer, textIterator.Position, searchPattern)) {
					if (!SearchOptions.MatchWholeWord || IsWholeWordAt(textIterator.TextBuffer, textIterator.Position, searchPattern.Length)) {
						return textIterator.Position;
					}
				}
			}
			return -1;
		}
		
		int InternalFindNext(ITextIterator textIterator, int offset, int length)
		{
			while (textIterator.MoveAhead(1) && TextSelection.IsInsideRange(textIterator.Position, offset, length)) {
				if (SearchOptions.MatchCase ? MatchCaseSensitive(textIterator.TextBuffer, textIterator.Position, searchPattern) : MatchCaseInsensitive(textIterator.TextBuffer, textIterator.Position, searchPattern)) {
					if (!SearchOptions.MatchWholeWord || IsWholeWordAt(textIterator.TextBuffer, textIterator.Position, searchPattern.Length)) {
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
