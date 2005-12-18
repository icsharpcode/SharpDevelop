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
	public enum ConditionType
	{
		None,
		Until,
		While,
		DoWhile
	}
	
	public enum ConditionPosition
	{
		None,
		Start,
		End
	}
	
	public class DoLoopStatement : StatementWithEmbeddedStatement
	{
		Expression        condition;
		ConditionType     conditionType;
		ConditionPosition conditionPosition;
		
		public ConditionPosition ConditionPosition {
			get {
				return conditionPosition;
			}
			set {
				conditionPosition = value;
			}
		}
		
		public ConditionType ConditionType {
			get {
				return conditionType;
			}
			set {
				conditionType = value;
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
		
		public DoLoopStatement(Expression condition, Statement embeddedStatement, ConditionType conditionType, ConditionPosition conditionPosition)
		{
			this.Condition         = condition;
			this.EmbeddedStatement = embeddedStatement;
			this.ConditionType     = conditionType;
			this.ConditionPosition = conditionPosition;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[DoLoopStatement: ConditionPosition = {0}, ConditionType = {1}, Condition = {2}, EmbeddedStatement = {3}]",
			                     ConditionPosition,
			                     ConditionType,
			                     Condition,
			                     EmbeddedStatement);
		}
	}
}
