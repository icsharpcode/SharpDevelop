// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ErrorStatement : Statement
	{
		Expression expression;
		
		public Expression Expression {
			get {
				return expression;
			} set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public ErrorStatement(Expression expression)
		{
			this.Expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ErrorStatement: Expression = {0}]",
			                     Expression);
		}
	}
}
