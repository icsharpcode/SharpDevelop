//
// BaseMethodCallWithDefaultParameterIssue.cs
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
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace ICSharpCode.NRefactory.CSharp
{
	[IssueDescription("Call to base member with implicit default parameters",
	                  Description="Call to base member with implicit default parameters",
	                  Category = IssueCategories.CodeQualityIssues,
	                  Severity = Severity.Warning,
	                  AnalysisDisableKeyword = "BaseMethodCallWithDefaultParameter")]
	public class BaseMethodCallWithDefaultParameterIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<BaseMethodCallWithDefaultParameterIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				// skip
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				// skip
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				// skip
			}

			public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			{
				// skip
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				// skip
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				var mr = invocationExpression.Target as MemberReferenceExpression;
				if (mr == null || !(mr.Target is BaseReferenceExpression))
					return;

				var invocationRR = ctx.Resolve(invocationExpression) as InvocationResolveResult;
				if (invocationRR == null)
					return;

				var parentEntity = invocationExpression.GetParent<EntityDeclaration>();
				if (parentEntity == null)
					return;
				var rr = ctx.Resolve(parentEntity) as MemberResolveResult;
				if (rr == null)
					return;

				if (invocationExpression.Arguments.Count >= invocationRR.Member.Parameters.Count ||
					invocationRR.Member.Parameters.Count == 0 || 
				    !invocationRR.Member.Parameters.Last().IsOptional)
					return;

				if (!InheritanceHelper.GetBaseMembers(rr.Member, false).Any(m => m == invocationRR.Member))
					return;
				AddIssue(new CodeIssue(
					invocationExpression.RParToken,
					ctx.TranslateString("Call to base member with implicit default parameters")
				));
			}
		
			public override void VisitIndexerExpression(IndexerExpression indexerExpression)
			{
				base.VisitIndexerExpression(indexerExpression);
				if (!(indexerExpression.Target is BaseReferenceExpression))
					return;
				var invocationRR = ctx.Resolve(indexerExpression) as InvocationResolveResult;
				if (invocationRR == null)
					return;

				var parentEntity = indexerExpression.GetParent<IndexerDeclaration>();
				if (parentEntity == null)
					return;
				var rr = ctx.Resolve(parentEntity) as MemberResolveResult;
				if (rr == null)
					return;

				if (indexerExpression.Arguments.Count >= invocationRR.Member.Parameters.Count ||
				    invocationRR.Member.Parameters.Count == 0 || 
				    !invocationRR.Member.Parameters.Last().IsOptional)
					return;

				if (!InheritanceHelper.GetBaseMembers(rr.Member, false).Any(m => m == invocationRR.Member))
					return;
				AddIssue(new CodeIssue(
					indexerExpression.RBracketToken,
					ctx.TranslateString("Call to base member with implicit default parameters")
				));
			}
		}
	}
}

