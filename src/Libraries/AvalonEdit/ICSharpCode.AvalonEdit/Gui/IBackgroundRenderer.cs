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
	/// Background renderers allow drawing behind the text layer.
	/// </summary>
	public interface IBackgroundRenderer
	{
		/// <summary>
		/// Causes the background renderer to draw.
		/// </summary>
		void Draw(DrawingContext dc);
	}
}
