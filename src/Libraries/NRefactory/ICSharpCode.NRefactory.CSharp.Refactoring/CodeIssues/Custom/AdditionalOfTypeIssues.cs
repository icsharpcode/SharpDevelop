//
// AdditionalOfTypeIssues.cs
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
	[IssueDescription("Replace with OfType<T> (extended)",
	                  Description = "Replace with call to OfType<T> (extended cases)",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Hint)]
	public class AdditionalOfTypeIssues : GatherVisitorCodeIssueProvider
	{
		static readonly AstNode whereSimpleCase =
			new InvocationExpression(
				new MemberReferenceExpression(new AnyNode("target"), "Where"),
				new NamedNode("lambda", 
					new LambdaExpression {
						Parameters = { PatternHelper.NamedParameter("param1", PatternHelper.AnyType("paramType", true), Pattern.AnyString) },
						Body = PatternHelper.OptionalParentheses(
								new IsExpression(PatternHelper.OptionalParentheses(new NamedNode("expr1", new IdentifierExpression(Pattern.AnyString))), new AnyNode("type"))
						)
					}
				)
			);

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<AdditionalOfTypeIssues>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitInvocationExpression (InvocationExpression anyInvoke)
			{
				var match = ReplaceWithOfTypeIssue.selectNotNullPattern.Match (anyInvoke);
				if (match.Success)
					return;

				match = ReplaceWithOfTypeIssue.wherePatternCase1.Match (anyInvoke);
				if (match.Success)
					return;

				match = ReplaceWithOfTypeIssue.wherePatternCase2.Match (anyInvoke); 
				if (match.Success)
					return;

				// Warning: The simple case is not 100% equal in semantic, but it's one common code smell
				match = whereSimpleCase.Match (anyInvoke); 
				if (!match.Success)
					return;
				var lambda = match.Get<LambdaExpression>("lambda").Single();
				var expr = match.Get<IdentifierExpression>("expr1").Single();
				if (lambda.Parameters.Count != 1)
					return;
				if (expr.Identifier != lambda.Parameters.Single().Name)
					return;
				AddIssue (new CodeIssue(
					anyInvoke,
					ctx.TranslateString("Replace with OfType<T>"),
					ctx.TranslateString("Replace with call to OfType<T>"),
					script => {
						var target = match.Get<Expression>("target").Single().Clone ();
						var type = match.Get<AstType>("type").Single().Clone();
						script.Replace(anyInvoke, new InvocationExpression(new MemberReferenceExpression(target, "OfType", type)));
					}
				));
			}
		}
	}
}

