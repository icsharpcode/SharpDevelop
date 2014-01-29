// 
// StringIsNullOrEmptyInspector.cs
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
	/// Checks for str == null &amp;&amp; str == " "
	/// Converts to: string.IsNullOrEmpty (str)
	/// </summary>
	[IssueDescription("Use 'String.IsNullOrEmpty'",
	       Description = "Uses shorter string.IsNullOrEmpty call instead of a longer condition.",
           Category = IssueCategories.PracticesAndImprovements,
	       Severity = Severity.Suggestion,
           AnalysisDisableKeyword = "ReplaceWithStringIsNullOrEmpty")]
	public class ReplaceWithStringIsNullOrEmptyIssue : GatherVisitorCodeIssueProvider
	{
		static readonly Pattern pattern = new Choice {
			// str == null || str == ""
			// str == null || str.Length == 0
			new BinaryOperatorExpression (
				PatternHelper.CommutativeOperatorWithOptionalParentheses (new AnyNode ("str"), BinaryOperatorType.Equality, new NullReferenceExpression ()),
				BinaryOperatorType.ConditionalOr,
				new Choice {
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new Backreference ("str"), BinaryOperatorType.Equality, new PrimitiveExpression ("")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new Backreference ("str"), BinaryOperatorType.Equality,
				                                       new PrimitiveType("string").Member("Empty")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (
						new MemberReferenceExpression (new Backreference ("str"), "Length"),
						BinaryOperatorType.Equality,
						new PrimitiveExpression (0)
					)
				}
			),
			// str == "" || str == null
			new BinaryOperatorExpression (
				new Choice {
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new AnyNode ("str"), BinaryOperatorType.Equality, new PrimitiveExpression ("")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new AnyNode ("str"), BinaryOperatorType.Equality,
				                                       new PrimitiveType("string").Member("Empty"))
				},
				BinaryOperatorType.ConditionalOr,
				PatternHelper.CommutativeOperator(new Backreference ("str"), BinaryOperatorType.Equality, new NullReferenceExpression ())
			)
		};
		static readonly Pattern negPattern = new Choice {
			// str != null && str != ""
			// str != null && str.Length != 0
			// str != null && str.Length > 0
			new BinaryOperatorExpression (
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode ("str"), BinaryOperatorType.InEquality, new NullReferenceExpression ()),
				BinaryOperatorType.ConditionalAnd,
				new Choice {
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new Backreference ("str"), BinaryOperatorType.InEquality, new PrimitiveExpression ("")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new Backreference ("str"), BinaryOperatorType.InEquality,
				                                   	   new PrimitiveType("string").Member("Empty")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (
						new MemberReferenceExpression (new Backreference ("str"), "Length"),
						BinaryOperatorType.InEquality,
						new PrimitiveExpression (0)
					),
					new BinaryOperatorExpression (
						new MemberReferenceExpression (new Backreference ("str"), "Length"),
						BinaryOperatorType.GreaterThan,
						new PrimitiveExpression (0)
					)
				}
			),
			// str != "" && str != null
			new BinaryOperatorExpression (
				new Choice {
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new AnyNode ("str"), BinaryOperatorType.InEquality, new PrimitiveExpression ("")),
					PatternHelper.CommutativeOperatorWithOptionalParentheses (new AnyNode ("str"), BinaryOperatorType.Equality,
				                                   	   new PrimitiveType("string").Member("Empty"))
				},
				BinaryOperatorType.ConditionalAnd,
				PatternHelper.CommutativeOperatorWithOptionalParentheses(new Backreference ("str"), BinaryOperatorType.InEquality, new NullReferenceExpression ())
			)
		};

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ReplaceWithStringIsNullOrEmptyIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
			{
				base.VisitBinaryOperatorExpression(binaryOperatorExpression);
				Match m = pattern.Match(binaryOperatorExpression);
				bool isNegated = false;
				if (!m.Success) {
					m = negPattern.Match(binaryOperatorExpression);
					isNegated = true;
				}
				if (m.Success) {
					var str = m.Get<Expression>("str").Single();
					var def = ctx.Resolve(str).Type.GetDefinition();
					if (def == null || def.KnownTypeCode != ICSharpCode.NRefactory.TypeSystem.KnownTypeCode.String)
						return;
					AddIssue(new CodeIssue(
						binaryOperatorExpression,
						isNegated ? ctx.TranslateString("Expression can be replaced with !string.IsNullOrEmpty") : ctx.TranslateString("Expression can be replaced with string.IsNullOrEmpty"),
						new CodeAction (
							isNegated ? ctx.TranslateString("Use !string.IsNullOrEmpty") : ctx.TranslateString("Use string.IsNullOrEmpty"),
							script => {
								Expression expr = new PrimitiveType("string").Invoke("IsNullOrEmpty", str.Clone());
								if (isNegated)
									expr = new UnaryOperatorExpression(UnaryOperatorType.Not, expr);
								script.Replace(binaryOperatorExpression, expr);
							},
							binaryOperatorExpression
						)
					));
					return;
				}
			}
		}
	}
}
