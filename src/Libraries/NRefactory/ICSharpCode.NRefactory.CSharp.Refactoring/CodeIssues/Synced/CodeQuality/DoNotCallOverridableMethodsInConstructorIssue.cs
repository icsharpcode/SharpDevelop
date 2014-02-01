//
// DoNotCallOverridableMethodsInConstructorIssue.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Virtual member call in constructor",
	                  Description = "Warns about calls to virtual member functions occuring in the constructor.",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "DoNotCallOverridableMethodsInConstructor")]
	public class DoNotCallOverridableMethodsInConstructorIssue : CodeIssueProvider
	{
		public override IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context, string subIssue)
		{
			var gv = new GatherVisitor(context);
			context.RootNode.AcceptVisitor(gv);
			return gv.CallFinder.FoundIssues;
		}
		
		class GatherVisitor : GatherVisitorBase<DoNotCallOverridableMethodsInConstructorIssue>
		{
			internal readonly VirtualCallFinderVisitor CallFinder;

			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
				CallFinder = new VirtualCallFinderVisitor(context);
			}

			bool isSealedType;

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration.ClassType != ClassType.Class && typeDeclaration.ClassType != ClassType.Struct)
					return;
				bool oldIsSealedType = isSealedType;
				isSealedType = typeDeclaration.Modifiers.HasFlag(Modifiers.Sealed);
				CallFinder.CurrentType = typeDeclaration;
				base.VisitTypeDeclaration(typeDeclaration);
				isSealedType = oldIsSealedType;
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				if (isSealedType)
					return;
				var body = constructorDeclaration.Body;
				if (body == null || body.IsNull)
					return;
				body.AcceptVisitor(CallFinder);
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				// nothing
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				// nothing
			}

			public override void VisitIndexerExpression(IndexerExpression indexerExpression)
			{
				// nothing
			}

			public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
			{
				// nothing
			}

			public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			{
				// nothing
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				// nothing
			}

			public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			{
				// nothing
			}
		}

		class VirtualCallFinderVisitor: GatherVisitorBase<DoNotCallOverridableMethodsInConstructorIssue>
		{
			readonly BaseRefactoringContext context;
			public TypeDeclaration CurrentType;
			public VirtualCallFinderVisitor(BaseRefactoringContext context) : base(context)
			{
				this.context = context;
			}

			public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
			{
				base.VisitMemberReferenceExpression(memberReferenceExpression);
				var targetMember = context.Resolve(memberReferenceExpression) as MemberResolveResult;
				if (targetMember != null && targetMember.IsVirtualCall && targetMember.TargetResult is ThisResolveResult) {
					CreateIssue(memberReferenceExpression);
				}
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var targetMethod = context.Resolve(invocationExpression) as InvocationResolveResult;
				if (targetMethod != null && targetMethod.IsVirtualCall && targetMethod.TargetResult is ThisResolveResult) {
					CreateIssue(invocationExpression);
				}
			}

			void CreateIssue(AstNode node)
			{
				AddIssue(new CodeIssue(
					node,
					context.TranslateString("Virtual member call in constructor"),
					new CodeAction(string.Format(context.TranslateString("Make class '{0}' sealed"), CurrentType.Name),
					               script => script.ChangeModifier(CurrentType, CurrentType.Modifiers | Modifiers.Sealed),
					               node)));
			}
			
			public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
			{
				// ignore lambdas
			}
			
			public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
			{
				// ignore anonymous methods
			}
		}
	}
}