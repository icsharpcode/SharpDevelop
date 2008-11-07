// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	/// <summary>
	/// A rule set in a XSHD file.
	/// </summary>
	public class XshdRuleSet : XshdElement
	{
		/// <summary>
		/// Gets/Sets the name of the rule set.
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets/sets whether the case is ignored in expressions inside this rule set.
		/// </summary>
		public bool? IgnoreCase { get; set; }
		
		readonly NullSafeCollection<XshdElement> elements = new NullSafeCollection<XshdElement>();
		
		/// <summary>
		/// Gets the collection of elements.
		/// </summary>
		public IList<XshdElement> Elements {
			get { return elements; }
		}
		
		/// <summary>
		/// Applies the visitor to all elements.
		/// </summary>
		public void AcceptElements(IXshdVisitor visitor)
		{
			foreach (XshdElement element in Elements) {
				element.AcceptVisitor(visitor);
			}
		}
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return visitor.VisitRuleSet(this);
		}
	}
}
