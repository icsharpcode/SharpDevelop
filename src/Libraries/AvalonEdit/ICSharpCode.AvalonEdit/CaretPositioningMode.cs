// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Specifies the mode for getting the next caret position.
	/// </summary>
	public enum CaretPositioningMode
	{
		/// <summary>
		/// Normal positioning (stop at every caret position)
		/// </summary>
		Normal,
		/// <summary>
		/// Stop only on word borders.
		/// </summary>
		WordBorder,
		/// <summary>
		/// Stop only at the beginning of words. This is used for Ctrl+Left/Ctrl+Right.
		/// </summary>
		WordStart,
		/// <summary>
		/// Stop only at the beginning of words, and anywhere in the middle of symbols.
		/// </summary>
		WordStartOrSymbol,
		/// <summary>
		/// Stop only on word borders, and anywhere in the middle of symbols.
		/// </summary>
		WordBorderOrSymbol
	}
}
