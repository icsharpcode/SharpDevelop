// 
// RedundantCheckBeforeAssignmentIssue.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
//      Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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

using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant condition check before assignment",
	                   Description = "Check for inequality before assignment is redundant if (x != value) x = value;",
	                   Category = IssueCategories.RedundanciesInCode,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "RedundantCheckBeforeAssignment")]
	public class RedundantCheckBeforeAssignmentIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantCheckBeforeAssignmentIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			static readonly AstNode pattern = 
				new IfElseStatement(
					PatternHelper.CommutativeOperatorWithOptionalParentheses(new AnyNode("a"), BinaryOperatorType.InEquality, new AnyNode("b")),
					PatternHelper.EmbeddedStatement(new AssignmentExpression(new Backreference("a"), PatternHelper.OptionalParentheses(new Backreference("b"))))
				);

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				var m = pattern.Match(ifElseStatement);
				if (!m.Success)
					return;
				AddIssue(new CodeIssue(
					ifElseStatement.Condition,
					ctx.TranslateString("Redundant condition check before assignment"),
					ctx.TranslateString("Remove redundant check"),
					script => {
						var stmt = ifElseStatement.TrueStatement;
						var block = stmt as BlockStatement;
						if (block != null)
							stmt = block.Statements.First();
						script.Replace(ifElseStatement, stmt.Clone());
					}
				));
			}
		}
	}
}
