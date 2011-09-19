// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Represents a highlighted document.
	/// </summary>
	/// <remarks>This interface is used by the <see cref="HighlightingColorizer"/> to register the highlighter as a TextView service.</remarks>
	public interface IHighlighter
	{
		/// <summary>
		/// Gets the underlying text document.
		/// </summary>
		IDocument Document { get; }
		
		/// <summary>
		/// Gets the stack of active colors (the colors associated with the active spans) at the end of the specified line.
		/// -> GetColorStack(1) returns the colors at the start of the second line.
		/// </summary>
		/// <remarks>
		/// GetColorStack(0) is valid and will return the empty stack.
		/// The elements are returned in inside-out order (first element of result enumerable is the color of the innermost span).
		/// </remarks>
		IEnumerable<HighlightingColor> GetColorStack(int lineNumber);
		
		// Starting with SD 5.0, this interface exports GetColorStack() instead of GetSpanStack().
		// This was done because custom highlighter implementations might not use the HighlightingSpan class (AST-based highlighting).
		
		/// <summary>
		/// Highlights the specified document line.
		/// </summary>
		/// <param name="lineNumber">The line to highlight.</param>
		/// <returns>A <see cref="HighlightedLine"/> line object that represents the highlighted sections.</returns>
		HighlightedLine HighlightLine(int lineNumber);
	}
}
