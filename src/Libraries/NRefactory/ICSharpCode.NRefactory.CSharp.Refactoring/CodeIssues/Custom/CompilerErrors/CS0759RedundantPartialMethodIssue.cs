// RedundantPartialMethodIssue.cs
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
	[IssueDescription ("CS0759: A partial method implementation is missing a partial method declaration",
	                   Description = "A partial method must have a defining declaration that defines the signature (name, return type and parameters) of the method. The implementation or method body is optional.",
	                   Category = IssueCategories.CompilerErrors,
	                   Severity = Severity.Error)]
	public class CS0759RedundantPartialMethodIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS0759RedundantPartialMethodIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (!methodDeclaration.HasModifier(Modifiers.Partial))
					return;

				var resolveResult = ctx.Resolve(methodDeclaration) as MemberResolveResult;
				if (resolveResult == null)
					return;

				var method = (IMethod) resolveResult.Member;
				if (method == null)
					return;

				if (!method.HasBody)
					return;

				if (method.Parts.Count == 1) {
					AddIssue(new CodeIssue(methodDeclaration.NameToken,
					         string.Format(ctx.TranslateString("CS0759: A partial method `{0}' implementation is missing a partial method declaration"), method.FullName),
						GetFixAction(methodDeclaration)));
				}
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				//We never need to visit the children of block statements
			}

			CodeAction GetFixAction(MethodDeclaration methodDeclaration)
			{
				return new CodeAction(ctx.TranslateString("Remove 'partial'"), script => {
					script.ChangeModifier (methodDeclaration, methodDeclaration.Modifiers & ~Modifiers.Partial);
				}, methodDeclaration);
			}
		}
	}
}

