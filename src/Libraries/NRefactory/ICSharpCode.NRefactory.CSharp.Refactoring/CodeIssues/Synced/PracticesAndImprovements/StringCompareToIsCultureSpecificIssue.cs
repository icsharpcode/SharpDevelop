//
// StringCompareToIsCultureSpecificIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'string.CompareTo' is culture-aware",
	                  Description = "Warns when a culture-aware 'string.CompareTo' call is used by default.",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "StringCompareToIsCultureSpecific")]
	public class StringCompareToIsCultureSpecificIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<StringCompareToIsCultureSpecificIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				var rr = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (rr == null || rr.IsError)
					return;

				if (rr.Member.Name != "CompareTo" || 
				    !rr.Member.DeclaringType.IsKnownType (KnownTypeCode.String) ||
				    rr.Member.Parameters.Count != 1 ||
				    !rr.Member.Parameters[0].Type.IsKnownType(KnownTypeCode.String)) {
					return;
				}
				AddIssue(new CodeIssue(
					invocationExpression,
					ctx.TranslateString("'string.CompareTo' is culture-aware"), 
					new CodeAction(ctx.TranslateString("Use ordinal comparison"), script => AddArgument(script, invocationExpression, "Ordinal"), invocationExpression),
					new CodeAction(ctx.TranslateString("Use culture-aware comparison"), script => AddArgument(script, invocationExpression, "CurrentCulture"), invocationExpression)
				));

			}

			void AddArgument(Script script, InvocationExpression invocationExpression, string ordinal)
			{
				var mr = invocationExpression.Target as MemberReferenceExpression;
				if (mr == null)
					return;

				var astBuilder = ctx.CreateTypeSystemAstBuilder(invocationExpression);
				var newArgument = astBuilder.ConvertType(new TopLevelTypeName("System", "StringComparison")).Member(ordinal);

				var newInvocation = new PrimitiveType("string").Invoke(
					"Compare",
					mr.Target.Clone(),
					invocationExpression.Arguments.First().Clone(),
					newArgument
				);
				script.Replace(invocationExpression, newInvocation);
			}
		}
	}
}