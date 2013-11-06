//
// PartialMethodParameterNameMismatchIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription(
		"Parameter name differs in partial method declaration",
		Description = "Parameter name differs in partial method declaration",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "PartialMethodParameterNameMismatch")]
	public class PartialMethodParameterNameMismatchIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<PartialMethodParameterNameMismatchIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				// skip
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				// skip
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				// skip
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				// skip
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				// skip
			}
		
			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (!methodDeclaration.HasModifier(Modifiers.Partial))
					return;
				var rr = ctx.Resolve(methodDeclaration) as MemberResolveResult;
				if (rr == null || rr.IsError)
					return;
				var method = rr.Member as IMethod;
				if (method == null)
					return;

				int arg = 0;
				foreach (var param in methodDeclaration.Parameters) {
					var pr = ctx.Resolve(param) as LocalResolveResult;
					if (pr == null)
						continue;
					foreach (var part in method.Parts) {
						if (param.Name != part.Parameters[arg].Name) {
							List<CodeAction> fixes = new List<CodeAction>();
							foreach (var p2 in method.Parts) {
								if (param.Name != p2.Parameters[arg].Name) {
									int _arg = arg;
									fixes.Add(new CodeAction (
										string.Format(ctx.TranslateString("Rename to '{0}'"), p2.Parameters[_arg].Name),
										s => {
											s.Rename(pr.Variable, p2.Parameters[_arg].Name);
										},
										param
									)); 
								}
							}
							// TODO: Atm I think it makes only sense to offer a fix if the issue disappears
							// which might not be the case here.
							if (fixes.Count > 1) {
								fixes.Clear();
							}
							AddIssue(new CodeIssue(
								param.NameToken,
								ctx.TranslateString("Parameter name differs in partial method declaration"),
								fixes
							));
							break;
						}
					}
					arg++;
				}
			}
		}
	}
}

