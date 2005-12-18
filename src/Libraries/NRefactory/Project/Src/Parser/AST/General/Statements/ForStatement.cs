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
	public class ForStatement : StatementWithEmbeddedStatement
	{
		ArrayList       initializers; // EmbeddedStatement OR list of StatmentExpressions
		Expression      condition;
		ArrayList iterator; // [Statement]
		
		public ArrayList Initializers {
			get {
				return initializers;
			}
		}
		
		public ArrayList Iterator {
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
		
		public ForStatement(ArrayList initializers, Expression condition, ArrayList iterator, Statement embeddedStatement)
		{
			this.initializers = initializers == null ? new ArrayList(1) : initializers;
			this.iterator = iterator == null ? new ArrayList(1) : iterator;
			
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
