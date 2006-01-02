// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class ForStatement : StatementWithEmbeddedStatement
	{
		List<Statement> initializers; // EmbeddedStatement OR list of StatmentExpressions
		Expression      condition;
		List<Statement> iterator; // [Statement]
		
		public List<Statement> Initializers {
			get {
				return initializers;
			}
		}
		
		public List<Statement> Iterator {
			get {
				return iterator;
			}
		}
		
		public Expression Condition {
			get {
				return condition;
			}
			set {
				condition = Expression.CheckNull(value);
			}
		}
		
		public ForStatement(List<Statement> initializers, Expression condition, List<Statement> iterator, Statement embeddedStatement)
		{
			this.initializers = initializers ?? new List<Statement>(1);
			this.iterator = iterator ?? new List<Statement>(1);
			
			this.Condition         = condition;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ForStatement: Initializers{0}, Condition={1}, iterator{2}, EmbeddedStatement={3}]", 
			                     GetCollectionString(initializers),
			                     condition,
			                     GetCollectionString(iterator),
			                     EmbeddedStatement);
		}
	}
}
