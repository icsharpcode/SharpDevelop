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
	public interface IHighlighter : IDisposable
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
		
		/// <summary>
		/// Enforces a highlighting state update (triggering the HighlightingStateChanged event if necessary)
		/// for all lines up to (and inclusive) the specified line number.
		/// </summary>
		void UpdateHighlightingState(int lineNumber);
		
		/// <summary>
		/// Notification when the highlighter detects that the highlighting state at the end of a line
		/// has changed.
		/// This event gets raised for each line as it is processed by the highlighter
		/// unless the highlighting state for the line is equal to the old state (when the same line was highlighted previously).
		/// </summary>
		/// <remarks>
		/// For implementers: there is the requirement that, if there was no state changed reported at line X,
		/// and there were no document changes between line X and Y (with Y > X), then
		/// this event must not be raised for any line between X and Y.
		/// 
		/// Equal input state + unchanged line = Equal output state.
		/// 
		/// See the comment in the HighlightingColorizer.OnHighlightStateChanged implementation
		/// for details about the requirements for a correct custom IHighlighter.
		/// </remarks>
		event HighlightingStateChangedEventHandler HighlightingStateChanged;
		
		/// <summary>
		/// Opens a group of <see cref="HighlightLine"/> calls.
		/// It is not necessary to call this method before calling <see cref="HighlightLine"/>,
		/// however, doing so can make the highlighting much more performant in some cases
		/// (e.g. the C# semantic highlighter in SharpDevelop will re-use the resolver within a highlighting group).
		/// </summary>
		/// <remarks>
		/// The group is closed by either a <see cref="EndHighlighting"/> or a <see cref="IDisposable.Dispose"/> call.
		/// Nested groups are not allowed.
		/// </remarks>
		void BeginHighlighting();
		
		/// <summary>
		/// Closes the currently opened group of <see cref="HighlightLine"/> calls.
		/// </summary>
		/// <seealso cref="BeginHighlighting"/>.
		void EndHighlighting();
		
		/// <summary>
		/// Retrieves the HighlightingColor with the specified name. Returns null if no color matching the name is found.
		/// </summary>
		HighlightingColor GetNamedColor(string name);
		
		/// <summary>
		/// Gets the default text color.
		/// </summary>
		HighlightingColor DefaultTextColor { get; }
	}
	
	/// <summary>
	/// Event handler for <see cref="IHighlighter.HighlightingStateChanged"/>
	/// </summary>
	public delegate void HighlightingStateChangedEventHandler(IHighlighter sender, int fromLineNumber, int toLineNumber);
}
