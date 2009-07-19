// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// A highlighting rule set describes a set of spans that are valid at a given code location.
	/// </summary>
	public class HighlightingRuleSet
	{
		/// <summary>
		/// Creates a new RuleSet instance.
		/// </summary>
		public HighlightingRuleSet()
		{
			this.Spans = new NullSafeCollection<HighlightingSpan>();
			this.Rules = new NullSafeCollection<HighlightingRule>();
		}
		
		/// <summary>
		/// Gets/Sets the name of the rule set.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets the list of spans.
		/// </summary>
		public IList<HighlightingSpan> Spans { get; private set; }
		
		/// <summary>
		/// Gets the list of rules.
		/// </summary>
		public IList<HighlightingRule> Rules { get; private set; }
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[" + GetType().Name + " " + Name + "]";
		}
	}
}
