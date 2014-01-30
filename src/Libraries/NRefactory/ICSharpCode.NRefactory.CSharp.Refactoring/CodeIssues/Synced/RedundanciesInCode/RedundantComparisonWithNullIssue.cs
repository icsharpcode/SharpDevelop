//
// RedundantComparisonWithNullIssue.cs
//
// Author:
//	   Ji Kun <jikun.nus@gmail.com>
//
// Copyright (c) 2013 Ji Kun
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
// THE SOFTWARE.using System;

using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant comparison with 'null'",
	                  Description = "When 'is' keyword is used, which implicitly check null.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
                      AnalysisDisableKeyword = "RedundantComparisonWithNull")]
	public class RedundantComparisonWithNullIssue : GatherVisitorCodeIssueProvider
	{
		private static readonly Pattern pattern1
			= new Choice {
			//  a is Record && a != null
			new BinaryOperatorExpression(
				PatternHelper.OptionalParentheses(
					new IsExpression {
						Expression = new AnyNode("a"),
						Type = new AnyNode("t")
					}),
				BinaryOperatorType.ConditionalAnd,
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new Backreference("a"),
					                                  BinaryOperatorType.InEquality,
					                                  new NullReferenceExpression())
			),
			//  a != null && a is Record
			new BinaryOperatorExpression (
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode("a"),
					                                  BinaryOperatorType.InEquality,
					                                  new NullReferenceExpression()),
				BinaryOperatorType.ConditionalAnd,
				PatternHelper.OptionalParentheses(
					new IsExpression {
						Expression = new Backreference("a"),
						Type = new AnyNode("t")
					})
			)
		};

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantComparisonWithNullIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				Match m1 = pattern1.Match(binaryOperatorExpression);
				if (m1.Success) {
					AddIssue(new CodeIssue(binaryOperatorExpression,
					         ctx.TranslateString("Redundant comparison with 'null'"),
					         ctx.TranslateString("Remove expression"), 
					         script => {
					         	var isExpr = m1.Get<AstType>("t").Single().Parent;
					         	script.Replace(binaryOperatorExpression, isExpr);
					         }
					) { IssueMarker = IssueMarker.GrayOut });
					return;
				}
			}
		}
	}
}

