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
	/// A highlighting span is a region with start+end expression that has a different RuleSet inside
	/// and colors the region.
	/// </summary>
	[Serializable]
	public class HighlightingSpan
	{
		/// <summary>
		/// Gets/Sets the start expression.
		/// </summary>
		public Regex StartExpression { get; set; }
		
		/// <summary>
		/// Gets/Sets the end expression.
		/// </summary>
		public Regex EndExpression { get; set; }
		
		/// <summary>
		/// Gets/Sets the rule set that applies inside this span.
		/// </summary>
		public HighlightingRuleSet RuleSet { get; set; }
		
		/// <summary>
		/// Gets the color used for the text matching the start expression.
		/// </summary>
		public HighlightingColor StartColor { get; set; }
		
		/// <summary>
		/// Gets the color used for the text between start and end.
		/// </summary>
		public HighlightingColor SpanColor { get; set; }
		
		/// <summary>
		/// Gets the color used for the text matching the end expression.
		/// </summary>
		public HighlightingColor EndColor { get; set; }
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " Start=" + StartExpression + ", End=" + EndExpression + "]";
		}
	}
}
