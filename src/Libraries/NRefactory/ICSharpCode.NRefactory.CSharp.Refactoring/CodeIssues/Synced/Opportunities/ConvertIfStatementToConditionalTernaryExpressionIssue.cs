//
// ConvertIfStatementToConditionalTernaryExpressionIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'if' statement can be re-written as '?:' expression",
	                  Description = "Convert 'if' to '?:'",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "ConvertIfStatementToConditionalTernaryExpression")]
	public class ConvertIfStatementToConditionalTernaryExpressionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		public static bool IsComplexExpression(AstNode expr)
		{
			return expr.StartLocation.Line != expr.EndLocation.Line ||
			expr is ConditionalExpression ||
			expr is BinaryOperatorExpression;
		}

		public static bool IsComplexCondition(Expression expr)
		{
			if (expr.StartLocation.Line != expr.EndLocation.Line)
				return true;

			if (expr is PrimitiveExpression || expr is IdentifierExpression || expr is MemberReferenceExpression || expr is InvocationExpression)
				return false;

			var pexpr = expr as ParenthesizedExpression;
			if (pexpr != null)
				return IsComplexCondition(pexpr.Expression);

			var uOp = expr as UnaryOperatorExpression;
			if (uOp != null)
				return IsComplexCondition(uOp.Expression);

			var bop = expr as BinaryOperatorExpression;
			if (bop == null)
				return true;
			return !(bop.Operator == BinaryOperatorType.GreaterThan ||
			bop.Operator == BinaryOperatorType.GreaterThanOrEqual ||
			bop.Operator == BinaryOperatorType.Equality ||
			bop.Operator == BinaryOperatorType.InEquality ||
			bop.Operator == BinaryOperatorType.LessThan ||
			bop.Operator == BinaryOperatorType.LessThanOrEqual);
		}

		class GatherVisitor : GatherVisitorBase<ConvertIfStatementToConditionalTernaryExpressionIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				Match match;
				if (!ConvertIfStatementToConditionalTernaryExpressionAction.GetMatch(ifElseStatement, out match))
					return;
//				var target = match.Get<Expression>("target").Single();
				var condition = match.Get<Expression>("condition").Single();
				var trueExpr = match.Get<Expression>("expr1").Single();
				var falseExpr = match.Get<Expression>("expr2").Single();

				if (IsComplexCondition(condition) || IsComplexExpression(trueExpr) || IsComplexExpression(falseExpr))
					return;
				AddIssue(new CodeIssue(
					ifElseStatement.IfToken,
					ctx.TranslateString("Convert to '?:' expression")
				){ IssueMarker = IssueMarker.DottedLine, ActionProvider = { typeof(ConvertIfStatementToConditionalTernaryExpressionAction) } });
			}
		}
	}
}

