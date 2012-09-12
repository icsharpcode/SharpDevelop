// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.SharpDevelop.Editor
{
	public static class HighlighterKnownSpanNames
	{
		public const string Comment = "Comment";
		public const string String = "String";
		public const string Char = "Char";
		
		public static bool IsLineStartInsideComment(this IHighlighter highligher, int lineNumber)
		{
			return highligher.GetColorStack(lineNumber).Any(c => c.Name == Comment);
		}
		
		public static bool IsLineStartInsideString(this IHighlighter highligher, int lineNumber)
		{
			return highligher.GetColorStack(lineNumber).Any(c => c.Name == String);
		}
	}
}
