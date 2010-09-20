// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// A line inside a <see cref="IDocument"/>.
	/// </summary>
	public interface IDocumentLine
	{
		/// <summary>
		/// Gets the starting offset of the line in the document's text.
		/// </summary>
		int Offset { get; }
		
		/// <summary>
		/// Gets the length of this line (=the number of characters on the line).
		/// </summary>
		int Length { get; }
		
		/// <summary>
		/// Gets the ending offset of the line in the document's text (= Offset + Length).
		/// </summary>
		int EndOffset { get; }
		
		/// <summary>
		/// Gets the length of this line, including the line delimiter.
		/// </summary>
		int TotalLength { get; }
		
		/// <summary>
		/// Gets the length of the line terminator.
		/// Returns 1 or 2; or 0 at the end of the document.
		/// </summary>
		int DelimiterLength { get; }
		
		/// <summary>
		/// Gets the number of this line.
		/// The first line has the number 1.
		/// </summary>
		int LineNumber { get; }
		
		/// <summary>
		/// Gets the text on this line.
		/// </summary>
		string Text { get; }
	}
}
