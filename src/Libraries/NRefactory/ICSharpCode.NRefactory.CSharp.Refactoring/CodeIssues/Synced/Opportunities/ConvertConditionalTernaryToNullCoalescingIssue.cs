// 
// ConditionalToNullCoalescingInspector.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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
	/// <summary>
	/// Checks for "a != null ? a : other"<expr>
	/// Converts to: "a ?? other"<expr>
	/// </summary>
	[IssueDescription("'?:' expression can be converted to '??' expression",
	                  Description="'?:' expression can be converted to '??' expression.",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ConvertConditionalTernaryToNullCoalescing")]
	public class ConvertConditionalTernaryToNullCoalescingIssue : GatherVisitorCodeIssueProvider
	{
		static readonly Pattern unequalPattern = new Choice {
			// a != null ? a : other
			new ConditionalExpression(
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode("a"), BinaryOperatorType.InEquality, new NullReferenceExpression()),
				new Backreference("a"),
				new AnyNode("other")
			),

			// obj != null ? (Type)obj : other
			new ConditionalExpression(
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode("obj"), BinaryOperatorType.InEquality, new NullReferenceExpression()),
				new NamedNode("a", new CastExpression(new AnyNode(), new Backreference("obj"))),
				new AnyNode("other")
			)

		};

		static readonly Pattern equalPattern = new Choice {
			// a == null ? other : a
			new ConditionalExpression(
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode("a"), BinaryOperatorType.Equality, new NullReferenceExpression()),
				new AnyNode("other"),
				new Backreference("a")
			)
		};

		
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<ConvertConditionalTernaryToNullCoalescingIssue>
		{

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitConditionalExpression(ConditionalExpression conditionalExpression)
			{
				Match m = unequalPattern.Match(conditionalExpression);
				bool isEqual = false;
				if (!m.Success) {
					isEqual = true;
					m = equalPattern.Match(conditionalExpression);
				}
				if (m.Success) {
					var a = m.Get<Expression>("a").Single();
					var other = m.Get<Expression>("other").Single();

					if (isEqual) {
						var castExpression = other as CastExpression;
						if (castExpression != null) {
							a = new CastExpression(castExpression.Type.Clone(), a.Clone());
							other = castExpression.Expression;
						}
					}

					AddIssue(new CodeIssue(conditionalExpression, ctx.TranslateString("'?:' expression can be re-written as '??' expression"), new CodeAction (
						ctx.TranslateString("Replace '?:'  operator with '??"), script => {
							var expr = new BinaryOperatorExpression (a.Clone (), BinaryOperatorType.NullCoalescing, other.Clone ());
							script.Replace (conditionalExpression, expr);
						}, conditionalExpression)));
				}
				base.VisitConditionalExpression (conditionalExpression);
			}
		}
	}
}
