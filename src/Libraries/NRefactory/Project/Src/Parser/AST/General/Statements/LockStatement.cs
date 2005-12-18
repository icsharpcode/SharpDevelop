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
	public class LockStatement : StatementWithEmbeddedStatement
	{
		Expression lockExpression;
		
		public Expression LockExpression {
			get {
				return lockExpression;
			}
			set {
				lockExpression = Expression.CheckNull(value);
			}
		}
		
		public LockStatement(Expression lockExpression, Statement embeddedStatement)
		{
			this.LockExpression    = lockExpression;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[LockStatement: LockExpression={0}, EmbeddedStatement={1}]", 
			                     lockExpression,
			                     EmbeddedStatement);
		}
	}
}
