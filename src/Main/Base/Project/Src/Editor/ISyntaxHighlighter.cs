// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Represents the syntax highlighter inside the text editor.
	/// </summary>
	public interface ISyntaxHighlighter
	{
		/// <summary>
		/// Retrieves the names of the spans that are active at the start of the specified line.
		/// Nested spans are returned in inside-out order (first element of result enumerable is the innermost span).
		/// </summary>
		IEnumerable<string> GetSpanColorNamesFromLineStart(int lineNumber);
		
		/// <summary>
		/// Gets the highlighting definition that is being used.
		/// </summary>
		IHighlightingDefinition HighlightingDefinition { get; }
		
		/// <summary>
		/// Adds an additional highlighting engine that runs in addition to the XSHD-based highlighting.
		/// </summary>
		void AddAdditionalHighlighter(IHighlighter highlighter);
		
		/// <summary>
		/// Removes an additional highlighting engine.
		/// </summary>
		void RemoveAdditionalHighlighter(IHighlighter highlighter);
	}
	
	public static class SyntaxHighligherKnownSpanNames
	{
		public const string Comment = "Comment";
		public const string String = "String";
		public const string Char = "Char";
		
		public static bool IsLineStartInsideComment(this ISyntaxHighlighter highligher, int lineNumber)
		{
			return highligher.GetSpanColorNamesFromLineStart(lineNumber).Contains(Comment);
		}
		
		public static bool IsLineStartInsideString(this ISyntaxHighlighter highligher, int lineNumber)
		{
			return highligher.GetSpanColorNamesFromLineStart(lineNumber).Contains(String);
		}
	}
}
