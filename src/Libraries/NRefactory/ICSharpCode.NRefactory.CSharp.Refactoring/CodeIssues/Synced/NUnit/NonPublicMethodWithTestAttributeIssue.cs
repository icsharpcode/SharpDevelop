//
// NonPublicMethodWithTestAttributeIssue.cs
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
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("NUnit Test methods should have public visibility",
	                  Description = "Non public methods are not found by NUnit.",
	                  Category = IssueCategories.NUnit,
	                  Severity = Severity.Hint,
	                  AnalysisDisableKeyword = "NUnit.NonPublicMethodWithTestAttribute")]
	public class NonPublicMethodWithTestAttributeIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<NonPublicMethodWithTestAttributeIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				var result = ctx.Resolve(methodDeclaration) as MemberResolveResult; 
				if (result == null || result.IsError)
					return;

				var member = result.Member;
				if (member.IsOverride || member.IsStatic || member.IsPublic)
					return;

				if (!member.Attributes.Any(attr => attr.AttributeType.Name == "TestAttribute" && attr.AttributeType.Namespace == "NUnit.Framework"))
					return;
				AddIssue(new CodeIssue(
					methodDeclaration.NameToken,
					ctx.TranslateString("NUnit test methods should be public"),
					ctx.TranslateString("Make method public"),
					script => {
						script.ChangeModifier(methodDeclaration, Modifiers.Public);
					}
				));
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// Empty
			}
		}
	}
}

