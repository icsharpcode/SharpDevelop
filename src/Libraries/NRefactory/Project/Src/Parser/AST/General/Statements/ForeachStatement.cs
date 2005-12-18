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
	public class ForeachStatement : StatementWithEmbeddedStatement
	{
		TypeReference typeReference;
		string        variableName;
		Expression    expression;
		Expression    nextExpression;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public string VariableName {
			get {
				return variableName;
			}
			set {
				variableName = value == null ? String.Empty : value;
			}
		}
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = Expression.CheckNull(value);
			}
		}
		
		public Expression NextExpression {
			get {
				return nextExpression;
			}
			set {
				nextExpression = Expression.CheckNull(value);
			}
		}
		
		public ForeachStatement(TypeReference typeReference, string variableName, Expression expression, Statement embeddedStatement)
		{
			this.TypeReference     = typeReference;
			this.VariableName      = variableName;
			this.Expression        = expression;
			this.EmbeddedStatement = embeddedStatement;
			this.NextExpression    = null;
		}
		
		public ForeachStatement(TypeReference typeReference, string variableName, Expression expression, Statement embeddedStatement, Expression nextExpression)
		{
			this.typeReference = typeReference;
			this.variableName = variableName;
			this.expression = expression;
			this.nextExpression = nextExpression;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString() {
			return String.Format("[ForeachStatement: embeddedStatement = {0}, expression = {1}, nextExpression = {2}, typeReference = {3}, variableName = {4}]",
			                     EmbeddedStatement,
			                     expression,
			                     nextExpression,
			                     typeReference,
			                     variableName);
		}
	}
}
