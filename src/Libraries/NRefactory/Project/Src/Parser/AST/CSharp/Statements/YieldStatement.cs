using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class YieldStatement : Statement
	{
		Statement statement;
		
		public Statement Statement {
			get {
				return statement;
			}
			set {
				statement = Statement.CheckNull(value);
			}
		}
		
		public bool IsYieldReturn()
		{
			return Statement is ReturnStatement;
		}
		
		public bool IsYieldBreak()
		{
			return Statement is BreakStatement;
		}
		
		public YieldStatement(Statement statement)
		{
			this.Statement = statement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[YieldSatement: Statement={0}]", 
			                     statement);
		}
	}
}
