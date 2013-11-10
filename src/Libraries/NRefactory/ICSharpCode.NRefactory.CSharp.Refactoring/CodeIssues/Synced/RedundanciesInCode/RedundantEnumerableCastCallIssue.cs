//
// RedundantEnumerableCastCallIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	// OfType -> Underline (+suggest to compare to null)
	[IssueDescription (
		"Redundant 'IEnumerable.Cast<T>' or 'IEnumerable.OfType<T>' call",
		Description = "Redundant 'IEnumerable.Cast<T>' or 'IEnumerable.OfType<T>' call",
		Category = IssueCategories.RedundanciesInCode,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "RedundantEnumerableCastCall")]
	public class RedundantEnumerableCastCallIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantEnumerableCastCallIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var mt = invocationExpression.Target as MemberReferenceExpression;
				if (mt == null)
					return;
				var rr = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (rr == null || rr.IsError)
					return;
				if (rr.Member.DeclaringType.Name != "Enumerable" || rr.Member.DeclaringType.Namespace != "System.Linq")
					return;
				bool isCast = rr.Member.Name == "Cast";
				if (!isCast && rr.Member.Name != "OfType")
					return;
				var tr = ctx.Resolve(mt.Target);
				if (tr.Type.Equals(rr.Type) || tr.Type.GetAllBaseTypes().Any (bt=> bt.Equals(rr.Type))) {
					if (isCast) {
						AddIssue(new CodeIssue(
							mt.DotToken.StartLocation,
							mt.EndLocation,
							ctx.TranslateString("Redundant 'IEnumerable.Cast<T>' call"),
							ctx.TranslateString("Remove 'Cast<T>' call"),
							s => s.Replace(invocationExpression, mt.Target.Clone())
						) { IssueMarker = IssueMarker.GrayOut });
					} else {
						AddIssue(new CodeIssue(
							mt.DotToken.StartLocation,
							mt.EndLocation,
							ctx.TranslateString("Redundant 'IEnumerable.OfType<T>' call."),
							new [] {
								new CodeAction(
									ctx.TranslateString("Compare items with null"),
									s => {
										var name = ctx.GetNameProposal("i", mt.StartLocation);
										s.Replace(invocationExpression, 
											new InvocationExpression(
												new MemberReferenceExpression(mt.Target.Clone(), "Where"), 
												new LambdaExpression {
													Parameters = { new ParameterDeclaration(name) },
													Body = new BinaryOperatorExpression(new IdentifierExpression(name), BinaryOperatorType.InEquality, new NullReferenceExpression())
												}
											)
										);
									},
									mt
								),
								new CodeAction(
									ctx.TranslateString("Remove 'OfType<T>' call"),
									s => s.Replace(invocationExpression, mt.Target.Clone()),
									mt
								),
							}
						));
					}
				}
			}
		}
	}
}

