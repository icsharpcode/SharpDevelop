// RedundantPartialTypeIssue.cs
// 
// Author:
//      Luís Reis <luiscubal@gmail.com>
// 
// Copyright (c) 2013 Luís Reis
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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.CSharp;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{

	[IssueDescription ("Redundant 'partial' modifier in type declaration",
	                   Description = "Class is declared partial but has only one part",
	                   Category = IssueCategories.RedundanciesInDeclarations,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "PartialTypeWithSinglePart")]
	public class PartialTypeWithSinglePartIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<PartialTypeWithSinglePartIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (!typeDeclaration.HasModifier(Modifiers.Partial)) {
					//We still need to visit the children in search of partial nested types.
					base.VisitTypeDeclaration(typeDeclaration);
					return;
				}

				var resolveResult = ctx.Resolve(typeDeclaration) as TypeResolveResult;
				if (resolveResult == null)
					return;

				var typeDefinition = resolveResult.Type.GetDefinition();
				if (typeDefinition == null)
					return;

				if (typeDefinition.Parts.Count == 1) {
					var partialModifierToken = typeDeclaration.ModifierTokens.Single(modifier => modifier.Modifier == Modifiers.Partial);
					// there may be a disable comment before the partial token somewhere
					foreach (var child in typeDeclaration.Children.TakeWhile (child => child != partialModifierToken)) {
						child.AcceptVisitor(this);
					}
					AddIssue(new CodeIssue(partialModifierToken,
					         ctx.TranslateString("Partial class with single part"),
						GetFixAction(typeDeclaration, partialModifierToken)) { IssueMarker = IssueMarker.GrayOut });
				}
				base.VisitTypeDeclaration(typeDeclaration);
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				//We never need to visit the children of block statements
			}

			CodeAction GetFixAction(TypeDeclaration typeDeclaration, CSharpModifierToken partialModifierToken)
			{
				return new CodeAction(ctx.TranslateString("Remove 'partial'"), script => {
					script.ChangeModifier (typeDeclaration, typeDeclaration.Modifiers & ~Modifiers.Partial);
				}, partialModifierToken);
			}
		}
	}
}

