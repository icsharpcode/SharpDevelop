// 
// EmptyConstructorIssue.cs
// 
// Author:
//      Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun
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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SaHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Empty constructor",
	                   Description = "An empty public constructor without paramaters is redundant.",
	                   Category = IssueCategories.RedundanciesInDeclarations,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "EmptyConstructor")]
	public class EmptyConstructorIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			var unit = context.RootNode as SyntaxTree;
			if (unit == null)
				return null;
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<EmptyConstructorIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
                : base(ctx)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typedeclaration)
			{
				bool hasEmptyConstructor = false;
				bool hasUnemptyConstructor = false;
				ConstructorDeclaration emptyContructorNode = null;

				foreach (var child in typedeclaration.Children.OfType<ConstructorDeclaration>()) {
					if (child.HasModifier(Modifiers.Static)) 
						continue;
					if (child.Body.Any() || child.Parameters.Count > 0) {
						hasUnemptyConstructor = true;
					} else if (child.HasModifier(Modifiers.Public)) {
						if (child.Initializer.Arguments.Any())
							continue;
						hasEmptyConstructor = true;
						emptyContructorNode = child;
					}
				}

				if (!hasUnemptyConstructor && hasEmptyConstructor) {
					AddIssue(new CodeIssue(
						emptyContructorNode.NameToken,
						ctx.TranslateString("Empty constructor is redundant."), 
						new CodeAction(
							ctx.TranslateString("Remove redundant constructor"),
							script => script.Remove(emptyContructorNode),
							emptyContructorNode.NameToken
						)
					) { IssueMarker = IssueMarker.GrayOut });
				}
			}
		}
	}
}
