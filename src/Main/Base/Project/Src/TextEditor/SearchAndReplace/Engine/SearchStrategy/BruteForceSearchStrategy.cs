// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Undo;
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
			return (offset - 1 < 0 || Char.IsWhiteSpace(document.GetCharAt(offset - 1))) &&
			       (offset + length + 1 >= document.Length || Char.IsWhiteSpace(document.GetCharAt(offset + length)));
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
		
		public void CompilePattern()
		{
			searchPattern = SearchOptions.MatchCase ? SearchOptions.FindPattern : SearchOptions.FindPattern.ToUpper();
		}
		
		public SearchResult FindNext(ITextIterator textIterator)
		{
			int offset = InternalFindNext(textIterator);
			return offset >= 0 ? new SearchResult(offset, searchPattern.Length) : null;
		}
	}
}
