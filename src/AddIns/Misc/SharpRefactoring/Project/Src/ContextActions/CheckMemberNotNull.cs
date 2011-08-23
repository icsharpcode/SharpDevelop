// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// For expressions like "a.b.c" adds checks for null "if (a != null && a.b != null)".
	/// </summary>
	public class CheckMemberNotNull : ContextAction
	{
		public override string Title {
			get { return "Add null checks"; }
		}
		
		/// <summary>
		/// Expression at caret.
		/// </summary>
		Expression currentExpr;
		/// <summary>
		/// Expression to be checked for null.
		/// </summary>
		Expression targetExpr;
		
		public override bool IsAvailable(EditorContext context)
		{
			this.currentExpr = context.GetContainingElement<Expression>();
			if (currentExpr is InvocationExpression) {
				// InvocationExpression (e.g. "e.Foo()") has MemberReferenceExpression as TargetObject (e.g. "e.Foo")
				// "e.Foo() -> e"
				this.targetExpr = GetTarget(((InvocationExpression)currentExpr).TargetObject);
			} else {
				// "a.b" -> "a"
				this.targetExpr = GetTarget(currentExpr);
			}
			return 
				this.targetExpr is MemberReferenceExpression ||
				this.targetExpr is CastExpression ||
				this.targetExpr is ParenthesizedExpression;
				//this.targetExpr is IdentifierExpression;		// "don't offer the action for just a.b, only for a.b.c"
		}
		
		public override void Execute(EditorContext context)
		{
			var conditionExpr = BuildCondition(this.targetExpr);
			if (conditionExpr == null)
				return;
			var ifExpr = new IfElseStatement(conditionExpr, new BlockStatement());
			
			context.Editor.InsertCodeBefore(this.currentExpr, ifExpr);
		}
		
		/// <summary>
		/// Gets "a.b" from "a.b.c"
		/// </summary>
		Expression GetTarget(Expression memberReferenceExpr)
		{
			if (memberReferenceExpr is MemberReferenceExpression) {
				return ((MemberReferenceExpression)memberReferenceExpr).TargetObject;
			}
			return null;
		}
		
		/// <summary>
		/// Turns "(a.b as T).d.e" into "(a != null) && (a.b is T) && ((a.b as T).d != null)"
		/// </summary>
		Expression BuildCondition(Expression targetExpr)
		{
			var parts = GetConditionParts(targetExpr);
			Expression condition = null;
			foreach (var part in parts) {
				if (condition == null) {
					// first
					condition = new ParenthesizedExpression(part);
				} else {
					condition = new BinaryOperatorExpression(new ParenthesizedExpression(part), BinaryOperatorType.LogicalAnd, condition);
				}
			}
			return condition;
		}
		
		List<Expression> GetConditionParts(Expression targetExpr)
		{
			var result = new List<Expression>();
			Expression current = targetExpr;
			while (current != null)
			{
				// process current expr
				if (current is MemberReferenceExpression) {
					var memberExpr = (MemberReferenceExpression)current;
					// expr != null
					result.Add(new BinaryOperatorExpression(memberExpr, BinaryOperatorType.InEquality, new PrimitiveExpression(null)));
				} else if (current is CastExpression) {
					var castExpr = (CastExpression)current;
					// expr is T
					result.Add(new TypeOfIsExpression(castExpr.Expression, castExpr.CastTo));
					// no need to check (a != null) && (a is T): (a is T) is enough - skip the "a"
					current = StepIn(current);
				}
				else if (current is IdentifierExpression) {
					result.Add(new BinaryOperatorExpression((IdentifierExpression)current, BinaryOperatorType.InEquality, new PrimitiveExpression(null)));
				}
				
				// step down the AST (e.g. from "a as T" to "a")
				current = StepIn(current);
			}
			return result;
		}
		
		Expression StepIn(Expression expr)
		{
			if (expr is MemberReferenceExpression) {
				return ((MemberReferenceExpression)expr).TargetObject;
			} else if (expr is CastExpression) {
				return ((CastExpression)expr).Expression;
			}
			else if (expr is ParenthesizedExpression) {
				return ((ParenthesizedExpression)expr).Expression;
			}
			else if (expr is IdentifierExpression) {
				return null;
			} else {
				return null;
			}
		}
	}
}