using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class UsingStatement : Statement
	{
		Statement  resourceAcquisition;
		Statement  embeddedStatement;
		
		public Statement ResourceAcquisition {
			get {
				return resourceAcquisition;
			}
			set {
				resourceAcquisition = Statement.CheckNull(value);
			}
		}
		
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = Statement.CheckNull(value);
			}
		}
		
		public UsingStatement(Statement resourceAcquisition, Statement embeddedStatement)
		{
			this.ResourceAcquisition = resourceAcquisition;
			this.EmbeddedStatement   = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UsingStatement: ResourceAcquisition={0}, EmbeddedStatement={1}]", 
			                     resourceAcquisition,
			                     embeddedStatement);
		}
	}
}
