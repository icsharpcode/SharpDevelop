using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public enum ContinueType {
		None,
		Do,
		For,
		While
	}
	public class ContinueStatement : Statement
	{
		ContinueType continueType;
		
		public ContinueType ContinueType {
			get {
				return continueType;
			}
			set {
				continueType = value;
			}
		}
		
		public ContinueStatement() : this(ContinueType.None)
		{
		}
		
		public ContinueStatement(ContinueType continueType)
		{
			this.continueType = continueType;
		}
		
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ContinueStatement]");
		}
	}
}
