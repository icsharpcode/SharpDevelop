using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class EmptyStatement : Statement
	{
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[EmptyStatement]");
		}
	}
}
