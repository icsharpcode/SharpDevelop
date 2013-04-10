// 
// RedundantConstructorIssue.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("An empty constructor is not necessary",
	                   Description = "An alone empty constructor is not necessary.",
                       Category = IssueCategories.Redundancies,
                       Severity = Severity.Suggestion,
	     			   ResharperDisableKeyword = "RedundantConstructor",
                       IssueMarker = IssueMarker.GrayOut)]
	public class RedundantConstructorIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			var unit = context.RootNode as SyntaxTree;
			if (unit == null)
				return Enumerable.Empty<CodeIssue>();
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<RedundantConstructorIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
                : base(ctx)
			{
			}

			public override void VisitTypeDeclaration(TypeDeclaration typedeclaration)
			{
				bool hasEmptyConstructor = false;
				bool hasUnemptyConstructor = false;
				ConstructorDeclaration emptyContructorNode = new ConstructorDeclaration();

				foreach (var child in typedeclaration.Children.OfType<ConstructorDeclaration>()) {
					if (child.HasModifier(Modifiers.Static)) 
						continue;
					if (child.Body.Count() > 0 || (child.Parameters.Count > 0)) {
						hasUnemptyConstructor = true;
					} else if (child.HasModifier(Modifiers.Public)) {
						hasEmptyConstructor = true;
						emptyContructorNode = child;
					}
				}

				if (!hasUnemptyConstructor && hasEmptyConstructor) {
					AddIssue(emptyContructorNode, ctx.TranslateString("Remove redundant constructor"), script =>
					{
						script.Remove(emptyContructorNode);
					});   
				}
				return;   
			}
		}
	}
}
