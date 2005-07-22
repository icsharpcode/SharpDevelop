// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class LabelStatement : Statement
	{
		string label;
		
		public string Label {
			get {
				return label;
			}
			set {
				label = value == null ? String.Empty : value;
			}
		}
		
		public LabelStatement(string label)
		{
			this.Label = label;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[LabelStatement: Label={0}]",
			                     label);
		}
	}
}
