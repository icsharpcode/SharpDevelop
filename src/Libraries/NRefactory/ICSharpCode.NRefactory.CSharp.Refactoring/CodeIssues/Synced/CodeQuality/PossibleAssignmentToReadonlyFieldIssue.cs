//
// PossibleAssignmentToReadonlyFieldIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription (
		"Possible assignment to readonly field",
		Description = "Check if a readonly field is used as assignment target",
		Category = IssueCategories.CodeQualityIssues,
		Severity = Severity.Warning,
		AnalysisDisableKeyword = "PossibleAssignmentToReadonlyField")]
	public class PossibleAssignmentToReadonlyFieldIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<PossibleAssignmentToReadonlyFieldIssue>
		{
			bool inConstructor;

			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				inConstructor = true;
				base.VisitConstructorDeclaration(constructorDeclaration);
				inConstructor = false;
			}

			void Check(Expression expr)
			{
				var mr = expr as MemberReferenceExpression;
				if (mr != null) {
					if (inConstructor && mr.Descendants.Any(d => d.Parent is MemberReferenceExpression && d is ThisReferenceExpression))
						return;
					Check(mr.Target);
				}
				if (inConstructor && expr is IdentifierExpression)
					return;

				var rr = ctx.Resolve(expr) as MemberResolveResult;

				if (rr == null || rr.IsError)
					return;
				var field = rr.Member as IField;

				if (field == null || !field.IsReadOnly)
					return;

				if (field.Type.Kind == TypeKind.TypeParameter) {
					var param = (ITypeParameter)field.Type;
					if (param.HasReferenceTypeConstraint)
						return;
					// TODO: Add resolve actions: Add class constraint + remove readonly modifier
					AddIssue(new CodeIssue(expr,
						ctx.TranslateString("Assignment to a property of a readonly field can be useless. Type parameter is not known to be a reference type.")));
					return;
				}
				if (field.Type.Kind == TypeKind.Struct) {
					// TODO: Add resolve actions: Remove readonly modifier
					AddIssue(new CodeIssue(expr, ctx.TranslateString("Readonly field can not be used as assignment target.")));
				}
			}

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression (assignmentExpression);
				Check(assignmentExpression.Left);
			}

			public override void VisitDirectionExpression(DirectionExpression directionExpression)
			{
				base.VisitDirectionExpression (directionExpression);
				Check(directionExpression.Expression);
			}

			public override void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
			{
				base.VisitUnaryOperatorExpression (unaryOperatorExpression);
				if (unaryOperatorExpression.Operator == UnaryOperatorType.Increment || unaryOperatorExpression.Operator == UnaryOperatorType.Decrement ||
					unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement || unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement) {
					Check(unaryOperatorExpression.Expression);
				}
			}


		}
	}
}

