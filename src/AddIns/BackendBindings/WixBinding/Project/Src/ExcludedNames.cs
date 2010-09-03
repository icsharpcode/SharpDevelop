// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// A list of names (filenames or directory names) that are to 
	/// be excluded.
	/// </summary>
	/// <remarks>
	/// A name can include the wildcards (*, ?) and these will be
	/// used when calling the IsExcluded method.
	/// </remarks>
	public class ExcludedNames : CollectionBase
	{
		public ExcludedNames()
		{
		}
		
		/// <summary>
		/// Adds a name to the excluded list.
		/// </summary>
		public void Add(string name)
		{
			List.Add(name);
		}
		
		/// <summary>
		/// Adds another ExcludedNames collection to this collection.
		/// </summary>
		public void Add(ExcludedNames names)
		{
			foreach (string name in names) {
				Add(name);
			}
		}
		
		/// <summary>
		/// Adds a list of names to the excluded list.
		/// </summary>
		public void AddRange(string[] names)
		{
			foreach (string name in names) {
				Add(name);
			}
		}
		
		/// <summary>
		/// Checks whether the specified name is in the excluded list.
		/// If any items in the list contain wildcards then these will
		/// be expanded. This method does not just do a simple Contains
		/// on the collection of names.
		/// </summary>
		public bool IsExcluded(string name)
		{
			foreach (string excludedName in List) {
				if (IsMatch(excludedName, name)) {
				    return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks whether the specified name matches the 
		/// excluded name. The excluded name can include wildcards.
		/// </summary>
		bool IsMatch(string excludedName, string name)
		{
			if (ContainsWildcard(excludedName)) {
				return IsWildcardMatch(excludedName, name);
			} else {
				return excludedName.Equals(name, StringComparison.OrdinalIgnoreCase);
			}
		}
		
		/// <summary>
		/// Compares the input string against the wildcard pattern.
		/// </summary>
		bool IsWildcardMatch(string pattern, string input)
		{
			string regexPattern = GetRegexFromWildcardPattern(pattern);
			Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Match match = regex.Match(input);
			return match.Success;
		}
		
		/// <summary>
		/// Converts the wildcard string into a regex string.
		/// </summary>
		string GetRegexFromWildcardPattern(string pattern)
		{
			StringBuilder regex = new StringBuilder();
			regex.Append("^");
			foreach (char ch in pattern) {
				switch (ch) {
					case '*':
						regex.Append(".*?");
						break;
					case '?':
						regex.Append(".");
						break;
					default:
						regex.Append(Regex.Escape(ch.ToString()));
						break;
				}
			}
			regex.Append("$");
			return regex.ToString();
		}
		
		bool ContainsWildcard(string input)
		{
			return input.IndexOf('*') >= 0 || input.IndexOf('?') >= 0;
		}
	}
}
