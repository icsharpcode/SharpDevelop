// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.VBNetBinding
{
	public static class LanguageUtils
	{
		public static string TrimComments(this string argument)
		{
			if (string.IsNullOrEmpty(argument))
				return string.Empty;
			
			bool inStr = false;
			
			for (int i = 0; i < argument.Length; i++) {
				if (argument[i] == '"')
					inStr = !inStr;
				if (argument[i] == '\'' && !inStr)
					return argument.Substring(0, i).Trim();
			}
			
			return argument;
		}
	}
}
