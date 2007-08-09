//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//
//namespace SearchAndReplace
//{
//	/// <summary>
//	/// Implements the Knuth, Morris, Pratt searching algorithm.
//	/// </summary>
//	public class KMPSearchStrategy : ISearchStrategy
//	{
//		string searchPattern;
//		int[]  overlap;
//		
//		public void CompilePattern()
//		{
//			if (searchPattern != (options.IgnoreCase ? options.SearchPattern.ToUpper() : options.SearchPattern)) {
//				searchPattern = options.IgnoreCase ? options.SearchPattern.ToUpper() : options.SearchPattern;
//				overlap = new int[searchPattern.Length + 1];
//				Preprocessing();
//			}
//		}
//		
//		void Preprocessing()
//		{
//			overlap[0] = -1;
//			for (int i = 0, j = -1; i < searchPattern.Length;) {
//				while (j >= 0 && searchPattern[i] != searchPattern[j]) {
//					j = overlap[j];
//				}
//				++i;
//				++j;
//				overlap[i] = j;
//			}
//		}
//		
//		int InternalFindNext(ITextIterator textIterator)
//		{
//			int j = 0;
//			if (!textIterator.MoveAhead(1)) {
//				return -1;
//			}
//			while (true) { // until pattern found or Iterator finished
//				while (j >= 0 && searchPattern[j] != (options.IgnoreCase ? Char.ToUpper(textIterator.GetCharRelative(j)) : textIterator.GetCharRelative(j))) {
//					if (!textIterator.MoveAhead(j - overlap[j])) {
//						return -1;
//					}
//					j = overlap[j];
//				}
//				if (++j >= searchPattern.Length) {
//					if ((!options.SearchWholeWordOnly || SearchReplaceUtilities.IsWholeWordAt(textIterator.TextBuffer, textIterator.Position, searchPattern.Length))) {
//						return textIterator.Position;
//					}
//					if (!textIterator.MoveAhead(j - overlap[j])) {
//						return -1;
//					}
//					j = overlap[j];
//				}
//			}			
//		}
//		
//		public SearchResult FindNext(ITextIterator textIterator)
//		{
//			int offset = InternalFindNext(textIterator, options);
//			
//			if (offset + searchPattern.Length >= textIterator.TextBuffer.Length) {
//				return FindNext(textIterator, options);
//			}
//			
//			return offset >= 0 ? new SearchResult(offset, searchPattern.Length) : null;
//		}
//	}
//}
