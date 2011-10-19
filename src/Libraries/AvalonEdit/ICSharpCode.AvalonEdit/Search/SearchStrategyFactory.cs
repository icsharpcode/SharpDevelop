// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.Search
{
	/// <summary>
	/// Provides factory methods for ISearchStrategies.
	/// </summary>
	public class SearchStrategyFactory
	{
		/// <summary>
		/// Creates a default ISearchStrategy with the given parameters.
		/// </summary>
		public static ISearchStrategy Create(string searchPattern, bool ignoreCase, bool useRegularExpressions, bool matchWholeWords)
		{
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;
			if (ignoreCase)
				options |= RegexOptions.IgnoreCase;
			if (!useRegularExpressions)
				searchPattern = Regex.Escape(searchPattern);
			if (matchWholeWords)
				searchPattern = "\\b" + searchPattern + "\\b";
			try {
				Regex pattern = new Regex(searchPattern, options);
				return new RegexSearchStrategy(pattern);
			} catch (ArgumentException ex) {
				throw new SearchPatternException(ex.Message, ex);
			}
		}
	}
}
