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
	/// A list of keywords.
	/// </summary>
	public class XshdKeywords : XshdElement
	{
		/// <summary>
		/// The color.
		/// </summary>
		public XshdReference<XshdColor> ColorReference { get; set; }
		
		readonly NullSafeCollection<string> words = new NullSafeCollection<string>();
		
		/// <summary>
		/// Gets the list of key words.
		/// </summary>
		public IList<string> Words {
			get { return words; }
		}
		
		/// <inheritdoc/>
		public override object AcceptVisitor(IXshdVisitor visitor)
		{
			return visitor.VisitKeywords(this);
		}
	}
}
