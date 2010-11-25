// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.VBNetBinding
{
	public static class LanguageUtils
	{
		public static string TrimComments(this string line)
		{
			if (string.IsNullOrEmpty(line))
				return string.Empty;
			
			bool inStr = false;
			
			for (int i = 0; i < line.Length; i++) {
				if (line[i] == '"')
					inStr = !inStr;
				if (line[i] == '\'' && !inStr)
					return line.Substring(0, i).Trim();
			}
			
			return line;
		}
		
		public static string TrimLine(this string line)
		{
			if (string.IsNullOrEmpty(line))
				return string.Empty;
			// remove string content
			MatchCollection matches = Regex.Matches(line, "\"[^\"]*?\"", RegexOptions.Singleline);
			foreach (Match match in matches) {
				line = line.Remove(match.Index, match.Length).Insert(match.Index, new string('-', match.Length));
			}
			// remove comments
			return TrimComments(line);
		}
	}
}
