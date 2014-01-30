//
// PolymorphicFieldLikeEventInvocationIssue.cs
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription (
		"Invocation of polymorphic field event",
		Description = "Invocation of polymorphic field event leads to unpredictable result since invocation lists are not virtual",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "PolymorphicFieldLikeEventInvocation")]
	public class PolymorphicFieldLikeEventInvocationIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<PolymorphicFieldLikeEventInvocationIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var rr = ctx.Resolve(invocationExpression.Target) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var evt = rr.Member as IEvent;
				if (evt == null || !evt.IsOverride)
					return;
				if (evt.AddAccessor.HasBody) {
					AddIssue(new CodeIssue(
						invocationExpression.Target,
						string.Format(ctx.TranslateString("The event `{0}' can only appear on the left hand side of `+=' or `-=' operator"), evt.Name)
					));
					return;
				}
				AddIssue(new CodeIssue(
					invocationExpression,
					ctx.TranslateString("Invocation of polymorphic field like event")));
			}
		}
	}
}

