// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Contains information relevant for text run creation.
	/// </summary>
	public interface ITextRunConstructionContext
	{
		/// <summary>
		/// Gets the text document.
		/// </summary>
		TextDocument Document { get; }
		
		/// <summary>
		/// Gets the text view for which the construction runs.
		/// </summary>
		TextView TextView { get; }
		
		/// <summary>
		/// Gets the visual line that is currently being constructed.
		/// </summary>
		VisualLine VisualLine { get; }
		
		/// <summary>
		/// Gets the global text run properties.
		/// </summary>
		TextRunProperties GlobalTextRunProperties { get; }
	}
}
