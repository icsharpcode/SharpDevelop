//
// SimplifyConditionalTernaryExpressionIssue.cs
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
	[IssueDescription("Simplify conditional expression",
	                  Description = "Conditional expression can be simplified",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "SimplifyConditionalTernaryExpression")]
	public class SimplifyConditionalTernaryExpressionIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<SimplifyConditionalTernaryExpressionIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static bool? GetBool(Expression trueExpression)
			{
				var pExpr = trueExpression as PrimitiveExpression;
				if (pExpr == null || !(pExpr.Value is bool))
					return null;
				return (bool)pExpr.Value;
			}

			public override void VisitConditionalExpression(ConditionalExpression conditionalExpression)
			{
				base.VisitConditionalExpression(conditionalExpression);

				bool? trueBranch = GetBool(CSharpUtil.GetInnerMostExpression(conditionalExpression.TrueExpression));
				bool? falseBranch = GetBool(CSharpUtil.GetInnerMostExpression(conditionalExpression.FalseExpression));

				if (trueBranch == falseBranch || 
				    trueBranch == true && falseBranch == false) // Handled by RedundantTernaryExpressionIssue
					return;

				AddIssue(new CodeIssue(
					conditionalExpression.QuestionMarkToken.StartLocation,
					conditionalExpression.FalseExpression.EndLocation,
					ctx.TranslateString("Simplify conditional expression"),
					ctx.TranslateString("Simplify conditional expression"),
					script => {
						if (trueBranch == false && falseBranch == true) {
							script.Replace(conditionalExpression, CSharpUtil.InvertCondition(conditionalExpression.Condition));
							return;
						}
						if (trueBranch == true) {
							script.Replace(
								conditionalExpression,
								new BinaryOperatorExpression(
									conditionalExpression.Condition.Clone(), 
									BinaryOperatorType.ConditionalOr,
									conditionalExpression.FalseExpression.Clone()
								)
							);
							return;
						}

						if (trueBranch == false) {
							script.Replace(
								conditionalExpression,
								new BinaryOperatorExpression(
									CSharpUtil.InvertCondition(conditionalExpression.Condition), 
									BinaryOperatorType.ConditionalAnd,
									conditionalExpression.FalseExpression.Clone()
								)
							);
							return;
						}
						
						if (falseBranch == true) {
							script.Replace(
								conditionalExpression,
								new BinaryOperatorExpression(
									CSharpUtil.InvertCondition(conditionalExpression.Condition), 
									BinaryOperatorType.ConditionalOr,
									conditionalExpression.TrueExpression.Clone()
								)
							);
							return;
						}

						if (falseBranch == false) {
							script.Replace(
								conditionalExpression,
								new BinaryOperatorExpression(
									conditionalExpression.Condition.Clone(), 
									BinaryOperatorType.ConditionalAnd,
									conditionalExpression.TrueExpression.Clone()
								)
							);
							return;
						}

						// Should never happen
					}
				));
			}

		}
	}
}

