//
// ConvertIfDoToWhileIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("'if-do-while' statement can be re-written as 'while' statement",
	                  Description = "Convert 'if-do-while' to 'while' statement",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ConvertIfDoToWhile")]
	public class ConvertIfDoToWhileIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<ConvertIfDoToWhileIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static readonly AstNode ifPattern = 
				new IfElseStatement(
					new AnyNode ("condition"),
					PatternHelper.EmbeddedStatement (
						new DoWhileStatement (new AnyNode("condition2"), new AnyNode ("EmbeddedStatement"))
					)
				);

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				var match = ifPattern.Match(ifElseStatement);
				if (match.Success) {
					var cond1 = match.Get<Expression>("condition").Single();
					var cond2 = match.Get<Expression>("condition2").Single();
					if (!CSharpUtil.AreConditionsEqual(cond1, cond2))
						return;
					AddIssue(new CodeIssue(
						ifElseStatement.IfToken,
						ctx.TranslateString("Statement can be simplified to 'while' statement"),
						ctx.TranslateString("Replace with 'while'"),
						script => {
							script.Replace(
								ifElseStatement, 
								new WhileStatement(
									cond1.Clone(),
									match.Get<Statement>("EmbeddedStatement").Single().Clone()
								)
							);
						}
					) { IssueMarker = IssueMarker.DottedLine });
				}
			}
		}
	}
}

