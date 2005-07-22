// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class GotoCaseStatement : Statement
	{
		Expression caseExpression;
		
		public Expression Expression {
			get {
				return caseExpression;
			}
			set {
				caseExpression = Expression.CheckNull(value);
			}
		}
		
		public bool IsDefaultCase {
			get {
				return caseExpression.IsNull;
			}
		}
		
		public GotoCaseStatement()
		{
			caseExpression = Expression.Null;
		}
		
		public GotoCaseStatement(Expression caseExpression)
		{
			this.Expression = caseExpression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[GotoCaseStatement: Expression={0}, IsDefaultCase={1}]", 
			                     caseExpression,
			                     IsDefaultCase);
		}
	}
}
