// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;

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
		/// Retrieves the HighlightingColor with the specified name. Returns null if no color matching the name is found.
		/// </summary>
		HighlightingColor GetNamedColor(string name);
		
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
		
		/// <summary>
		/// Invalidates a line, causing it to be re-highlighted.
		/// </summary>
		/// <remarks>
		/// This method is intended to be called by additional highlighters that process external information
		/// (e.g. semantic highlighting).
		/// </remarks>
		void InvalidateLine(IDocumentLine line);
		
		/// <summary>
		/// Invalidates all lines, causing-them to be re-highlighted.
		/// </summary>
		/// <remarks>
		/// This method is intended to be called by additional highlighters that process external information
		/// (e.g. semantic highlighting).
		/// </remarks>
		void InvalidateAll();
		
		/// <summary>
		/// Gets the document lines that are currently visible in the editor.
		/// </summary>
		IEnumerable<IDocumentLine> GetVisibleDocumentLines();
		
		/// <summary>
		/// Raised when the set of visible document lines has changed.
		/// </summary>
		event EventHandler VisibleDocumentLinesChanged;
		
		/// <summary>
		/// Creates a <see cref="HighlightedInlineBuilder"/> for a specified line.
		/// </summary>
		HighlightedInlineBuilder BuildInlines(int lineNumber);
		
		/// <summary>
		/// Gets the default text color.
		/// </summary>
		HighlightingColor DefaultTextColor { get; }
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
