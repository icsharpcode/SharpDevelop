// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A highlighting rule.
	/// </summary>
	public class HighlightingRule
	{
		/// <summary>
		/// Gets/Sets the regular expression for the rule.
		/// </summary>
		public Regex Regex { get; set; }
		
		/// <summary>
		/// Gets/Sets the highlighting color.
		/// </summary>
		public HighlightingColor Color { get; set; }
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " " + Regex + "]";
		}
	}
}
