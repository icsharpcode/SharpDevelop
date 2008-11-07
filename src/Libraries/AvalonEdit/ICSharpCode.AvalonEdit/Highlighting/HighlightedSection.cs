// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A text section with syntax highlighting information.
	/// </summary>
	public class HighlightedSection : ISegment
	{
		/// <summary>
		/// Gets/sets the document offset of the section.
		/// </summary>
		public int Offset { get; set; }
		
		/// <summary>
		/// Gets/sets the length of the section.
		/// </summary>
		public int Length { get; set; }
		
		/// <summary>
		/// Gets the highlighting color associated with the highlighted section.
		/// </summary>
		public HighlightingColor Color { get; set; }
	}
}
