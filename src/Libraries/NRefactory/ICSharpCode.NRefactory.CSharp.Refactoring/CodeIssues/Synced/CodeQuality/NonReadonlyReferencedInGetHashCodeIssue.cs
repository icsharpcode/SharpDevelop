//
// NonReadonlyReferencedInGetHashCodeIssue.cs
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

using System.Linq;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Non-readonly field referenced in 'GetHashCode()'",
		Description= "Non-readonly field referenced in 'GetHashCode()'",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "NonReadonlyReferencedInGetHashCode")]
	public class NonReadonlyReferencedInGetHashCodeIssue : GatherVisitorCodeIssueProvider
	{	
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		class GatherVisitor : GatherVisitorBase<NonReadonlyReferencedInGetHashCodeIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx) : base(ctx)
			{
			}

			#region Skipped declarations
			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
			}

			public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			{
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
			}

			public override void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
			{
			}
			#endregion

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (methodDeclaration.Name != "GetHashCode" || !methodDeclaration.HasModifier(Modifiers.Override) || methodDeclaration.Parameters.Any())
					return;
				if (!ctx.Resolve(methodDeclaration.ReturnType).Type.IsKnownType(KnownTypeCode.Int32))
					return;
				base.VisitMethodDeclaration(methodDeclaration);
			}

			public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
			{
				base.VisitMemberReferenceExpression(memberReferenceExpression);
				CheckNode(memberReferenceExpression, memberReferenceExpression.MemberNameToken);
			}

			public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
			{
				base.VisitIdentifierExpression(identifierExpression);
				CheckNode(identifierExpression, identifierExpression);
			}

			void CheckNode(AstNode expr, AstNode nodeToMark)
			{
				var resolvedResult = ctx.Resolve(expr);
				var mrr = resolvedResult as MemberResolveResult;
				if (mrr == null)
					return;
				var member = mrr.Member;
				var field = member as IField;
				if (field != null) {
					if (!field.IsReadOnly && !field.IsConst)
						AddIssue(new CodeIssue(nodeToMark, "Non-readonly field referenced in 'GetHashCode()'"));
				}
			}
		}
	}
}