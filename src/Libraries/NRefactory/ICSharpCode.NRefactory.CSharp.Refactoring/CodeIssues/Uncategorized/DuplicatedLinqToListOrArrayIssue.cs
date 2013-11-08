// 
// DuplicatedLinqToListOrArrayIssue.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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

using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Duplicated ToList() or ToArray() call",
	                   Description = "Duplicated call to ToList() or ToArray()",
	                   Category = IssueCategories.RedundanciesInCode,
	                   Severity = Severity.Warning)]
	public class DuplicatedLinqToListOrArrayIssue : GatherVisitorCodeIssueProvider
	{
		const string MemberTarget = "target";
		const string MemberReference = "member-reference";

		static readonly InvocationExpression NoArgumentPattern = new InvocationExpression {
			Target = new MemberReferenceExpression {
				Target = new AnyNode(MemberTarget),
				MemberName = Pattern.AnyString
			}.WithName(MemberReference)
		};

		static readonly string[] RelevantMethods = { "ToArray", "ToList" };

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<DuplicatedLinqToListOrArrayIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				int matches = 0;
				Expression currentExpression = invocationExpression;
				for (;;) {
					var match = NoArgumentPattern.Match(WithoutParenthesis(currentExpression));
					if (!match.Success) {
						break;
					}
					if (!RelevantMethods.Contains(match.Get<MemberReferenceExpression>(MemberReference).Single().MemberName)) {
						break;
					}
					++matches;
					currentExpression = match.Get<Expression>(MemberTarget).Single();
				}

				if (matches >= 2) {
					AddIssue(new CodeIssue(currentExpression.EndLocation,
					         invocationExpression.EndLocation,
					         ctx.TranslateString("Redundant Linq method invocations"),
					         ctx.TranslateString("Remove redundant method invocations"),
					         script => {

						string lastInvocation = ((MemberReferenceExpression)invocationExpression.Target).MemberName;
						var newMemberReference = new MemberReferenceExpression(currentExpression.Clone(),
						                                                       lastInvocation);
						var newInvocation = new InvocationExpression(newMemberReference);

						script.Replace(invocationExpression, newInvocation);

						}));
				}

				//Visit the children
				//We need to make sure currentExpression != invocationExpression
				//otherwise we get an infinite loop
				var expressionToVisit = currentExpression == invocationExpression ?
					invocationExpression.Target : currentExpression;
				expressionToVisit.AcceptVisitor(this);
			}

			static Expression WithoutParenthesis(Expression expr) {
				ParenthesizedExpression parenthesized;
				while ((parenthesized = expr as ParenthesizedExpression) != null) {
					expr = parenthesized.Expression;
				}
				return expr;
			}
		}
	}
}

