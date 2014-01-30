// EmptyDestructorIssue.cs
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

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription ("Empty destructor",
	                   Description = "Empty destructor is redundant",
	                   Category = IssueCategories.RedundanciesInDeclarations,
	                   Severity = Severity.Warning,
	                   AnalysisDisableKeyword = "EmptyDestructor"
	                   )]
	public class EmptyDestructorIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<EmptyDestructorIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				if (IsEmpty (destructorDeclaration.Body)) {
					AddIssue(new CodeIssue(destructorDeclaration.NameToken,
					         ctx.TranslateString("Empty destructor is redundant"),
						GetFixAction(destructorDeclaration)) { IssueMarker = IssueMarker.GrayOut });
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
			}

			public override void VisitUsingDeclaration(UsingDeclaration usingDeclaration)
			{
			}

			public override void VisitUsingAliasDeclaration(UsingAliasDeclaration usingDeclaration)
			{
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
			}

			bool IsEmpty(AstNode node)
			{
				if (node is EmptyStatement) {
					return true;
				}

				BlockStatement block = node as BlockStatement;
				return block != null && block.Statements.All(IsEmpty);
			}

			CodeAction GetFixAction(DestructorDeclaration destructorDeclaration)
			{
				return new CodeAction(ctx.TranslateString("Remove redundant destructor"), script =>
				{
					script.Remove(destructorDeclaration);
				}, destructorDeclaration.NameToken);
			}
		}
	}
}

