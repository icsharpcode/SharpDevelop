// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
	/// <summary>
	/// &lt;Import&gt; element.
	/// </summary>
	public class XshdImport : XshdElement
	{
		/// <summary>
		/// Gets/sets the referenced rule set.
		/// </summary>
		public XshdReference<XshdRuleSet> RuleSetReference { get; set; }
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return visitor.VisitImport(this);
		}
	}
}
