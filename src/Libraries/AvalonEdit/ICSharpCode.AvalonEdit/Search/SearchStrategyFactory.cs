// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
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
		public static ISearchStrategy Create(string searchPattern, bool ignoreCase, bool matchWholeWords, SearchType searchType)
		{
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			RegexOptions options = RegexOptions.Compiled | RegexOptions.Multiline;
			if (ignoreCase)
				options |= RegexOptions.IgnoreCase;
			
			switch (searchType) {
				case SearchType.Normal:
					searchPattern = Regex.Escape(searchPattern);
					break;
				case SearchType.Wildcard:
					searchPattern = ConvertWildcardsToRegex(searchPattern);
					break;
			}

			if (matchWholeWords)
				searchPattern = "\\b" + searchPattern + "\\b";
			try {
				Regex pattern = new Regex(searchPattern, options);
				return new RegexSearchStrategy(pattern);
			} catch (ArgumentException ex) {
				throw new SearchPatternException(ex.Message, ex);
			}
		}
		
		static string ConvertWildcardsToRegex(string searchPattern)
		{
			if (string.IsNullOrEmpty(searchPattern))
				return "";
			
			StringBuilder builder = new StringBuilder();
			
			foreach (char ch in searchPattern) {
				switch (ch) {
					case '?':
						builder.Append(".");
						break;
					case '*':
						builder.Append(".*");
						break;
					default:
						builder.Append(Regex.Escape(ch.ToString()));
						break;
				}
			}
			
			return builder.ToString();
		}
	}
}
