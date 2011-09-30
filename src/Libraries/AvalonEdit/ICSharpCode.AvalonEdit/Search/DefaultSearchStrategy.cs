// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	class DefaultSearchStrategy : ISearchStrategy
	{
		readonly Regex searchPattern;
		
		public DefaultSearchStrategy(Regex searchPattern)
		{
			this.searchPattern = searchPattern;
		}
	
		public static ISearchStrategy Create(string searchPattern, bool ignoreCase, bool useRegularExpressions, bool matchWholeWords)
		{
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			RegexOptions options = RegexOptions.Compiled;
			if (ignoreCase)
				options |= RegexOptions.IgnoreCase;
			if (!useRegularExpressions)
				searchPattern = Regex.Escape(searchPattern);
			if (matchWholeWords)
				searchPattern = "\\b" + searchPattern + "\\b";
			try {
				Regex pattern = new Regex(searchPattern, options);
				return new DefaultSearchStrategy(pattern);
			} catch (ArgumentException ex) {
				throw new SearchPatternException(ex.Message, ex);
			}
		}
		
		public IEnumerable<ISearchResult> FindAll(ITextSource document)
		{
			foreach (Match result in searchPattern.Matches(document.Text)) {
				yield return new SearchResult { StartOffset = result.Index, Length = result.Length };
			}
		}
	}
}
