//
// SealedMemberInSealedClassIssue.cs
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Sealed member in sealed class",
	                  Description = "'sealed' modifier is redundant in sealed classes",
	                  Category = IssueCategories.RedundanciesInDeclarations,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "SealedMemberInSealedClass")]
	public class SealedMemberInSealedClassIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<SealedMemberInSealedClassIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			void CheckNode(EntityDeclaration node)
			{
				if (!node.HasModifier(Modifiers.Override))
					return;
				var type = node.Parent as TypeDeclaration;
				if (type == null || !type.HasModifier(Modifiers.Sealed))
					return;
				foreach (var token_ in node.ModifierTokens) {
					var token = token_;
					if (token.Modifier == Modifiers.Sealed) {
						AddIssue(new CodeIssue(
							token, 
							ctx.TranslateString("Keyword 'sealed' is redundant in sealed classes."), 
							ctx.TranslateString("Remove redundant 'sealed' modifier"), 
							script => script.ChangeModifier(node, node.Modifiers & ~Modifiers.Sealed)
						) { IssueMarker = IssueMarker.GrayOut });
					}
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				base.VisitMethodDeclaration(methodDeclaration);
				CheckNode(methodDeclaration);
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				base.VisitFieldDeclaration(fieldDeclaration);
				CheckNode(fieldDeclaration);
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				base.VisitPropertyDeclaration(propertyDeclaration);
				CheckNode(propertyDeclaration);
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
				base.VisitIndexerDeclaration(indexerDeclaration);
				CheckNode(indexerDeclaration);
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
				base.VisitEventDeclaration(eventDeclaration);
				CheckNode(eventDeclaration);
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				base.VisitCustomEventDeclaration(eventDeclaration);
				CheckNode(eventDeclaration);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				base.VisitConstructorDeclaration(constructorDeclaration);
				CheckNode(constructorDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				base.VisitOperatorDeclaration(operatorDeclaration);
				CheckNode(operatorDeclaration);
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
				base.VisitFixedFieldDeclaration(fixedFieldDeclaration);
				CheckNode(fixedFieldDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				// SKIP
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration.Parent is TypeDeclaration) {
					CheckNode(typeDeclaration);
				}
				base.VisitTypeDeclaration(typeDeclaration);
			}
		}
	}
}

