// RedundantBlockInDifferentBranchesIssue.cs
//
// Author:
//      Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("RedundantBlockInDifferentBranches",
	                  Description = "Blocks in if/else can be simplified to any of the branches if they have the same block.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "RedundantBlockInDifferentBranches")]
	public class RedundantBlockInDifferentBranchesIssue : GatherVisitorCodeIssueProvider
	{
		
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context, this);
		}
		
		class GatherVisitor : GatherVisitorBase<RedundantBlockInDifferentBranchesIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx, RedundantBlockInDifferentBranchesIssue issueProvider) : base (ctx, issueProvider)
			{
			}
			
			static readonly AstNode pattern = new Choice{
				new IfElseStatement(
					new AnyNode("c"), 
					new AnyNode("s"), 
					new BlockStatement{new Backreference("s")}),
				new IfElseStatement(
					new AnyNode("c"), 
					new AnyNode("s"), 
					new Backreference("s")),
				new IfElseStatement(
					new AnyNode("c"), 
					new BlockStatement{new AnyNode("s")}, 
					new Backreference("s"))
			};

			bool IsSafeExpression(Expression expression, BaseRefactoringContext context)
			{
				var components = expression.DescendantsAndSelf;
				foreach (var c in components) {
					if (c is AssignmentExpression)
						return false;
					else if (c is UnaryOperatorExpression) {
						var ope = ((UnaryOperatorExpression)c).Operator;
						if (ope == UnaryOperatorType.Decrement || ope == UnaryOperatorType.Increment 
							|| ope == UnaryOperatorType.PostDecrement || ope == UnaryOperatorType.PostIncrement)
							return false;
					} else if (c is IdentifierExpression) {
						var result = context.Resolve(c);
						if (result.IsError)
							return false;
						if (!(result is LocalResolveResult))
							return false;
						if ((((LocalResolveResult)result).IsParameter))
							return false;
					}
				}
				return true;
			}

			public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);
				var m = pattern.Match(ifElseStatement);
				
				if (!m.Success)
					return;

				if (!IsSafeExpression(ifElseStatement.Condition, ctx))
					return;
			
				AddIssue(new CodeIssue(ifElseStatement.ElseToken, ctx.TranslateString("Blocks in if/else or switch branches can be simplified to any of the branches if they have the same block."))
				{ IssueMarker = IssueMarker.WavedLine });
			}
		}
	}
}