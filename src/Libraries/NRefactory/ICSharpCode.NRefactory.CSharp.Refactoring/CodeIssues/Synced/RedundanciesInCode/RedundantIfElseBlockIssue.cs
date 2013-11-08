// 
// RedundantIfElseBlockIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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

using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Redundant 'else' keyword",
	                  Description = "Redundant 'else' keyword.",
	                  Category = IssueCategories.RedundanciesInCode,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "RedundantIfElseBlock")]
	public class RedundantIfElseBlockIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<RedundantIfElseBlockIssue>
		{
			readonly LocalDeclarationSpaceVisitor declarationSpaceVisitor;

			public GatherVisitor (BaseRefactoringContext ctx)
				: base(ctx)
			{
				this.declarationSpaceVisitor = new LocalDeclarationSpaceVisitor();
			}

			public override void VisitSyntaxTree(SyntaxTree syntaxTree)
			{
				syntaxTree.AcceptVisitor(declarationSpaceVisitor);
				base.VisitSyntaxTree(syntaxTree);
			}

			bool ElseIsRedundantControlFlow(IfElseStatement ifElseStatement)
			{
				if (ifElseStatement.FalseStatement.IsNull || ifElseStatement.Parent is IfElseStatement)
					return false;
				var blockStatement = ifElseStatement.FalseStatement as BlockStatement;
				if (blockStatement != null && blockStatement.Statements.Count == 0)
					return true;
				var reachability = ctx.CreateReachabilityAnalysis(ifElseStatement.TrueStatement);
				return !reachability.IsEndpointReachable(ifElseStatement.TrueStatement);
			}

			bool HasConflictingNames(AstNode targetContext, AstNode currentContext)
			{
				var targetSpace = declarationSpaceVisitor.GetDeclarationSpace(targetContext);
				var currentSpace = declarationSpaceVisitor.GetDeclarationSpace(currentContext);
				foreach (var name in currentSpace.DeclaredNames) {
					var isUsed = targetSpace.GetNameDeclarations(name).Any(node => node.Ancestors.Any(n => n == currentContext));
					if (isUsed)
						return true;
				}
				return false;
			}
			public override void VisitIfElseStatement (IfElseStatement ifElseStatement)
			{
				base.VisitIfElseStatement(ifElseStatement);

				if (!ElseIsRedundantControlFlow(ifElseStatement) || HasConflictingNames(ifElseStatement.Parent, ifElseStatement.FalseStatement))
					return;

				AddIssue(new CodeIssue(ifElseStatement.ElseToken, ctx.TranslateString("Redundant 'else' keyword"), ctx.TranslateString("Remove redundant 'else'"), script =>  {
					int start = script.GetCurrentOffset(ifElseStatement.ElseToken.GetPrevNode(n => !(n is NewLineNode)).EndLocation);
					int end;
					var blockStatement = ifElseStatement.FalseStatement as BlockStatement;
					if (blockStatement != null) {
						if (blockStatement.Statements.Count == 0) {
							// remove empty block
							end = script.GetCurrentOffset(blockStatement.LBraceToken.StartLocation);
							script.Remove(blockStatement);
						}
						else {
							// remove block braces
							end = script.GetCurrentOffset(blockStatement.LBraceToken.EndLocation);
							script.Remove(blockStatement.RBraceToken);
						}
					}
					else {
						end = script.GetCurrentOffset(ifElseStatement.ElseToken.EndLocation);
					}
					if (end > start)
						script.RemoveText(start, end - start);
					script.FormatText(ifElseStatement.Parent);
				}) { IssueMarker = IssueMarker.GrayOut });
			}
		}
	}
}
