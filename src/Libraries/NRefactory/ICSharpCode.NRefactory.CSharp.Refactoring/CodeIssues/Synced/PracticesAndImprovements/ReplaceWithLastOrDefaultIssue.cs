//
// ReplaceWithLastOrDefaultIssue.cs
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Replace with LastOrDefault<T>()",
	                  Description = "Replace with call to LastOrDefault<T>()",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ReplaceWithLastOrDefault")]
	public class ReplaceWithLastOrDefaultIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ReplaceWithLastOrDefaultIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			readonly AstNode pattern =
				new ConditionalExpression(
					new InvocationExpression(
						new MemberReferenceExpression(new AnyNode("expr"), "Any"),
						new AnyNodeOrNull("param")
					),
					new InvocationExpression(
						new MemberReferenceExpression(new Backreference("expr"), "Last"),
						new Backreference("param")
					),
					new Choice {
						new NullReferenceExpression(),
						new DefaultValueExpression(new AnyNode())
					}
				);

			public override void VisitConditionalExpression(ConditionalExpression conditionalExpression)
			{
				base.VisitConditionalExpression(conditionalExpression);
				var match = pattern.Match(conditionalExpression);
				if (!match.Success)
					return;
				var expression = match.Get<Expression>("expr").First();
				var param      = match.Get<Expression>("param").First();

				AddIssue(new CodeIssue(
					conditionalExpression,
					ctx.TranslateString("Expression can be simlified to 'LastOrDefault<T>()'"),
					ctx.TranslateString("Replace with 'LastOrDefault<T>()'"),
					script => {
						var invocation = new InvocationExpression(new MemberReferenceExpression(expression.Clone(), "LastOrDefault"));
						if (param != null && !param.IsNull)
							invocation.Arguments.Add(param.Clone());
						script.Replace(
							conditionalExpression,
							invocation
							);
					}
				));
			}
		}
	}
}

