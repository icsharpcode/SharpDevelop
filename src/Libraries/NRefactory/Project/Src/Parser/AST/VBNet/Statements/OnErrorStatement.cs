using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class OnErrorStatement : Statement
	{
		Statement embeddedStatement;
		
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = Statement.CheckNull(value);
			}
		}
		
		public OnErrorStatement(Statement embeddedStatement)
		{
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[OnErrorStatement: EmbeddedStatement = {0}]",
			                     EmbeddedStatement);
		}
	}
}
