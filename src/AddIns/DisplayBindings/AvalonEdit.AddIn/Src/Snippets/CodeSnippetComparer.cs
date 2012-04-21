// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// Compares code snippets.
	/// </summary>
	public class CodeSnippetComparer : IEqualityComparer<CodeSnippet>
	{
		public static readonly CodeSnippetComparer Instance = new CodeSnippetComparer();
		
		public bool Equals(CodeSnippet x, CodeSnippet y)
		{
			if (x == y)
				return true;
			if (x == null || y == null)
				return false;
			return x.Name == y.Name && x.Description == y.Description && x.Text == y.Text && x.Keyword == y.Keyword;
		}
		
		public int GetHashCode(CodeSnippet obj)
		{
			return obj != null ? obj.Name.GetHashCode() : 0;
		}
	}
}
