// 
// LocalVariableNotUsedIssue.cs
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

using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Unused local variable",
		Description = "Local variable is never used.",
		Category = IssueCategories.RedundanciesInDeclarations,
		Severity = Severity.Warning,
		PragmaWarning = 163,
		AnalysisDisableKeyword = "UnusedVariable.Compiler")]
	public class LocalVariableNotUsedIssue : GatherVisitorCodeIssueProvider
	{

		#region ICodeIssueProvider implementation

		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		#endregion

		class GatherVisitor : GatherVisitorBase<LocalVariableNotUsedIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitVariableInitializer(VariableInitializer variableInitializer)
			{
				base.VisitVariableInitializer(variableInitializer);

				// check if variable is assigned
				if (!variableInitializer.Initializer.IsNull)
					return;
				var decl = variableInitializer.Parent as VariableDeclarationStatement;
				if (decl == null)
					return;

				var resolveResult = ctx.Resolve(variableInitializer) as LocalResolveResult;
				if (resolveResult == null)
					return;

				if (IsUsed(decl.Parent, resolveResult.Variable, variableInitializer))
					return;

				AddIssue(new CodeIssue(variableInitializer.NameToken, 
					string.Format(ctx.TranslateString("Local variable '{0}' is never used"), resolveResult.Variable.Name), ctx.TranslateString("Remove unused local variable"),
					script => {
						if (decl.Variables.Count == 1) {
							script.Remove(decl);
						} else {
							var newDeclaration = (VariableDeclarationStatement)decl.Clone();
							newDeclaration.Variables.Remove(
								newDeclaration.Variables.FirstOrNullObject(v => v.Name == variableInitializer.Name));
							script.Replace(decl, newDeclaration);
						}
					}) { IssueMarker = IssueMarker.GrayOut });
			}

			public override void VisitForeachStatement(ForeachStatement foreachStatement)
			{
				base.VisitForeachStatement(foreachStatement);

				var resolveResult = ctx.Resolve(foreachStatement.VariableNameToken) as LocalResolveResult;
				if (resolveResult == null)
					return;

				if (IsUsed(foreachStatement, resolveResult.Variable, foreachStatement.VariableNameToken))
					return;

				AddIssue(new CodeIssue(foreachStatement.VariableNameToken, ctx.TranslateString("Local variable is never used")));
			}

			bool IsUsed(AstNode rootNode, IVariable variable, AstNode variableNode)
			{
				return ctx.FindReferences(rootNode, variable).Any(result => result.Node != variableNode);
			}
		}
	}
}
