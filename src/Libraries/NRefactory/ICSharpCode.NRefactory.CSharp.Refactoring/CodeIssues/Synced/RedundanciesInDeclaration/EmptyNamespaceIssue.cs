// EmptyNamespaceIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Empty namespace declaration",
	                   Description = "Empty namespace declaration is redundant",
	                   Category = IssueCategories.RedundanciesInDeclarations,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "EmptyNamespace")]
	public class EmptyNamespaceIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<EmptyNamespaceIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				bool hasContents = false;
				foreach (var member in namespaceDeclaration.Members) {
					if (member is TypeDeclaration || member is DelegateDeclaration) {
						hasContents = true;
					}
					else if (member is NamespaceDeclaration) {
						hasContents = true;
						member.AcceptVisitor(this);
					}
				}

				if (!hasContents) {
					AddIssue(new CodeIssue(namespaceDeclaration.NamespaceToken, ctx.TranslateString("Empty namespace declaration is redundant"),
						GetFixAction(namespaceDeclaration)) { IssueMarker = IssueMarker.GrayOut });
				}
			}

			CodeAction GetFixAction(NamespaceDeclaration namespaceDeclaration)
			{
				return new CodeAction(ctx.TranslateString("Remove empty namespace"),
				                      script => script.Remove(namespaceDeclaration),
				                      namespaceDeclaration);
			}
		}
	}
}

