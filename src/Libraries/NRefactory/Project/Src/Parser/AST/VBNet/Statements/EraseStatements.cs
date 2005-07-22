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
	public class EraseStatement : Statement
	{
//		List<Expression> expressions;
		ArrayList expressions;
		
		public ArrayList Expressions {
			get {
				return expressions;
			} set {
				expressions = value == null ? new ArrayList(1) : value;
			}
		}
		
		public EraseStatement(ArrayList expressions)
		{
			this.Expressions = expressions;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[EraseStatement: Expressions = {0}]",
			                     Expressions);
		}
	}
}
