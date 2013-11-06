//
// UseMethodIsInstanceOfType.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Threading;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System;
using System.Diagnostics;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Use method IsInstanceOfType",
	                  Description = "Use method IsInstanceOfType",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "UseMethodIsInstanceOfType")]
	public class UseMethodIsInstanceOfTypeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<UseMethodIsInstanceOfTypeIssue>
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			static readonly Expression pattern = new InvocationExpression(
				new MemberReferenceExpression(new AnyNode("target"), "IsAssignableFrom"),
				new InvocationExpression(
					new MemberReferenceExpression(new AnyNode("object"), "GetType")
				)
			);

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var match = pattern.Match(invocationExpression);
				if (match.Success) {
					AddIssue(new CodeIssue(
						invocationExpression,
						ctx.TranslateString("Use method IsInstanceOfType (...)"),
						ctx.TranslateString("Replace with call to IsInstanceOfType"),
						s => {
							s.Replace(invocationExpression, new InvocationExpression(
								new MemberReferenceExpression(match.Get<Expression>("target").Single().Clone(), "IsInstanceOfType"),
								match.Get<Expression>("object").Single().Clone()
							));
						}
					));
				}
			}
		}
	}
}

