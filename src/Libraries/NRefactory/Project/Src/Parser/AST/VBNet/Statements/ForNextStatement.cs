// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ForNextStatement : StatementWithEmbeddedStatement
	{
		Expression start;
		Expression end;
		Expression step;
		
//		List<Expression> nextExpressions;
		ArrayList nextExpressions;
		TypeReference typeReference;
		string        variableName;
		
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
		
		public ArrayList NextExpressions {
			get {
				return nextExpressions;
			}
			set {
				nextExpressions = value == null ? new ArrayList(1) : value;
			}
		}
		
		public Expression Start {
			get {
				return start;
			}
			set {
				start = Expression.CheckNull(value);
			}
		}
		
		public Expression End {
			get {
				return end;
			}
			set {
				end = Expression.CheckNull(value);
			}
		}
		
		public Expression Step {
			get {
				return step;
			}
			set {
				step = Expression.CheckNull(value);
			}
		}
		
		public ForNextStatement(TypeReference typeReference, string variableName, Expression start, Expression end, Expression step, Statement embeddedStatement, ArrayList nextExpressions)
		{
			this.TypeReference     = typeReference;
			this.VariableName      = variableName;
			this.Start = start;
			this.NextExpressions = nextExpressions;
			this.End = end;
			this.Step = step;
			this.EmbeddedStatement = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ForNextStatement: start = {0}, end = {1}, step = {2}, embeddedStatement = {3}, nextExpressions={4}, name={5}, typeRef={6}]",
			                     start,
			                     end,
			                     step,
			                     EmbeddedStatement,
			                     GetCollectionString(nextExpressions),
			                     VariableName,
			                     TypeReference
			                    );
		}
	}
}
