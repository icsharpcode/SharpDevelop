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
	/// A visitor over the XSHD element tree.
	/// </summary>
	public interface IXshdVisitor
	{
		/// <summary/>
		object VisitRuleSet(XshdRuleSet ruleSet);
		
		/// <summary/>
		object VisitColor(XshdColor color);
		
		/// <summary/>
		object VisitKeywords(XshdKeywords keywords);
		
		/// <summary/>
		object VisitSpan(XshdSpan span);
		
		/// <summary/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "A VB programmer implementing a visitor?")]
		object VisitImport(XshdImport import);
		
		/// <summary/>
		object VisitRule(XshdRule rule);
	}
}
