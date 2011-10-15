// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Search
{
	class RegexSearchStrategy : ISearchStrategy
	{
		readonly Regex searchPattern;
		
		public RegexSearchStrategy(Regex searchPattern)
		{
			this.searchPattern = searchPattern;
		}
		
		public IEnumerable<ISearchResult> FindAll(ITextSource document)
		{
			foreach (Match result in searchPattern.Matches(document.Text)) {
				yield return new SearchResult { StartOffset = result.Index, Length = result.Length };
			}
		}
	}
	
	class SearchResult : TextSegment, ISearchResult
	{
	}
}
