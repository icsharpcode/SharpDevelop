//
// UseIsOperatorIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;
using System;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Use 'is' operator",
	                  Description = "'is' operator can be used",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "UseIsOperator")]
	public class UseIsOperatorIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<UseIsOperatorIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			static readonly Expression pattern1 = new InvocationExpression(
				new MemberReferenceExpression(new TypeOfExpression(new AnyNode("Type")), "IsAssignableFrom"),
				new InvocationExpression(
				new MemberReferenceExpression(new AnyNode("object"), "GetType")
			)
			);
			static readonly Expression pattern2 = new InvocationExpression(
				new MemberReferenceExpression(new TypeOfExpression(new AnyNode("Type")), "IsInstanceOfType"),
				new AnyNode("object")
			);



			void AddIssue(AstNode invocationExpression, Match match, bool negate = false)
			{
				AddIssue(new CodeIssue(
					invocationExpression,
					ctx.TranslateString("Use 'is' operator"),
					ctx.TranslateString("Replace with 'is' operator"), 
					s => {
						Expression expression = new IsExpression(CSharpUtil.AddParensForUnaryExpressionIfRequired(match.Get<Expression>("object").Single().Clone()), match.Get<AstType>("Type").Single().Clone());
						if (negate)
							expression = new UnaryOperatorExpression (UnaryOperatorType.Not, new ParenthesizedExpression(expression));
						s.Replace(invocationExpression, expression);
					}
				));
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var match = pattern1.Match(invocationExpression);
				if (!match.Success)
					match = pattern2.Match(invocationExpression);
				if (match.Success) {
					AddIssue(invocationExpression, match);
				}
			}

			/* Unfortunately not quite the same :/
			static readonly AstNode equalityComparePattern =
				PatternHelper.CommutativeOperator(
					PatternHelper.OptionalParentheses(new TypeOfExpression(new AnyNode("Type"))),
					BinaryOperatorType.Equality,
					PatternHelper.OptionalParentheses(new InvocationExpression(
						new MemberReferenceExpression(new AnyNode("object"), "GetType")
					))
				);
			static readonly AstNode inEqualityComparePattern =
				PatternHelper.CommutativeOperator(
					PatternHelper.OptionalParentheses(new TypeOfExpression(new AnyNode("Type"))),
					BinaryOperatorType.InEquality,
					PatternHelper.OptionalParentheses(new InvocationExpression(
					new MemberReferenceExpression(new AnyNode("object"), "GetType")
					))
					);
			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				var match = equalityComparePattern.Match(binaryOperatorExpression);
				if (match.Success) {
					AddIssue(new CodeIssue(binaryOperatorExpression, match);
					return;
				}

				match = inEqualityComparePattern.Match(binaryOperatorExpression);
				if (match.Success) {
					AddIssue(new CodeIssue(binaryOperatorExpression, match, true);
					return;
				}
			}*/
		}
	}
}

