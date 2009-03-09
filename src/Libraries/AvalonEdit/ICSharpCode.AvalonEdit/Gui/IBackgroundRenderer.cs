// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Background renderers draw in the background of a known layer.
	/// You can use background renderers to draw non-interactive elements on the TextView
	/// without introducing new UIElements.
	/// </summary>
	/// <remarks>Background renderer will draw only if their associated known
	/// layer chooses to draw them. For example, background renderers in the caret
	/// layer will be invisible when the caret is hidden.</remarks>
	public interface IBackgroundRenderer
	{
		/// <summary>
		/// Gets the layer on which this background renderer should draw.
		/// </summary>
		KnownLayer Layer { get; }
		
		/// <summary>
		/// Causes the background renderer to draw.
		/// </summary>
		void Draw(DrawingContext drawingContext);
	}
}
