// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search
{
	class RegexSearchStrategy : ISearchStrategy
	{
		readonly Regex searchPattern;
		
		public RegexSearchStrategy(Regex searchPattern)
		{
			this.searchPattern = searchPattern;
		}
		
		public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
		{
			foreach (Match result in searchPattern.Matches(document.Text)) {
				if (offset <= result.Index && (offset + length) >= (result.Index + result.Length))
					yield return new SearchResult { StartOffset = result.Index, Length = result.Length };
			}
		}
		
		public ISearchResult FindNext(ITextSource document, int offset, int length)
		{
			var result = searchPattern.Match(document.Text, offset, length);
			if (result != null && result.Success)
				return new SearchResult { StartOffset = result.Index, Length = result.Length };
			
			return null;
		}
	}
	
	class SearchResult : TextSegment, ISearchResult
	{
	}
}
