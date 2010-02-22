// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Represents an active element that allows the snippet to stay interactive after insertion.
	/// </summary>
	public interface IActiveElement
	{
		/// <summary>
		/// Called when the all snippet elements have been inserted.
		/// </summary>
		void OnInsertionCompleted();
		
		/// <summary>
		/// Called when the interactive mode is deactivated.
		/// </summary>
		void Deactivate();
		
		/// <summary>
		/// Gets whether this element is editable (the user will be able to select it with Tab).
		/// </summary>
		bool IsEditable { get; }
		
		/// <summary>
		/// Gets the segment associated with this element. May be null.
		/// </summary>
		ISegment Segment { get; }
	}
}
