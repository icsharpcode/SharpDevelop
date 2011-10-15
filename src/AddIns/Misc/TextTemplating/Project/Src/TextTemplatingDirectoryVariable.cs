// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;

namespace ICSharpCode.TextTemplating
{
	public static class TextTemplatingDirectoryVariable
	{
		public static bool IsDirectoryVariable(string name)
		{
			return EndsWithDir(name) || IsAddInPath(name);
		}
		
		static bool EndsWithDir(string name)
		{
			return name.EndsWith("Dir", StringComparison.OrdinalIgnoreCase);
		}
		
		static bool IsAddInPath(string name)
		{
			return name.StartsWith("addinpath:", StringComparison.OrdinalIgnoreCase);
		}
		
		public static string AppendTrailingSlashIfMissing(string variableValue)
		{
			if (!String.IsNullOrEmpty(variableValue)) {
				if (variableValue.Last() == '\\') {
					return variableValue;
				}
				return variableValue + "\\";
			}
			return String.Empty;
		}
	}
}
