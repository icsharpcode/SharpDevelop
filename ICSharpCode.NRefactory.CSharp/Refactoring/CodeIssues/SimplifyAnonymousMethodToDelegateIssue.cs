//
// SimplifyAnonymousMethodToDelegateIssue.cs
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
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Simplify anonymous method to delegate",
	                  Description = "Shows anonymous methods that can be simplified.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning)]
	public class SimplifyAnonymousMethodToDelegateIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static Pattern pattern = new Choice {
				new BlockStatement {
					new ReturnStatement (new AnyNode ("invoke")) 
				},
				new AnyNode ("invoke")
			};

			static InvocationExpression AnalyzeBody(AstNode body)
			{
				var m = pattern.Match(body);
				if (m.Success)
					return m.Get("invoke").Single () as InvocationExpression;
				return null;
			}

			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				base.VisitLambdaExpression(lambdaExpression);
				var invocation = AnalyzeBody(lambdaExpression.Body);
				if (invocation == null)
					return;

				var lambdaParameters = lambdaExpression.Parameters.ToList();
				if (lambdaParameters.Count != invocation.Arguments.Count)
					return;
				int i = 0;
				foreach (var param in invocation.Arguments) {
					var id = param as IdentifierExpression;
					if (id == null)
						return;
					if (lambdaParameters [i].Name != id.Identifier)
						return;
					i++;
				}
				
				AddIssue(
					lambdaExpression, 
					ctx.TranslateString("Expression can be reduced to delegate"),
					script => { script.Replace (lambdaExpression, invocation.Target.Clone ()); });
			}

			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				base.VisitAnonymousMethodExpression(anonymousMethodExpression);

				var invocation = AnalyzeBody(anonymousMethodExpression.Body);
				if (invocation == null)
					return;

				var lambdaParameters = anonymousMethodExpression.Parameters.ToList();
				if (lambdaParameters.Count != invocation.Arguments.Count)
					return;
				int i = 0;
				foreach (var param in invocation.Arguments) {
					var id = param as IdentifierExpression;
					if (id == null)
						return;
					if (lambdaParameters [i].Name != id.Identifier)
						return;
					i++;
				}
				
				AddIssue(
					anonymousMethodExpression, 
					ctx.TranslateString("Expression can be reduced to delegate"),
					script => { script.Replace (anonymousMethodExpression, invocation.Target.Clone ()); });
			}
		}
	}
}

