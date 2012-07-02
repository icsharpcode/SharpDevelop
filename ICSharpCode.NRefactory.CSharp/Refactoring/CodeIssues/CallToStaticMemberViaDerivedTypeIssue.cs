//
// CallToStaticMemberViaDerivedTypeIssue.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Call to static member via a derived class",
	                   Description = "Suggests using the class declaring a static function when calling it.",
	                   Category = IssueCategories.CodeQualityIssues,
	                   Severity = Severity.Suggestion)]
	public class CallToStaticMemberViaDerivedTypeIssue : ICodeIssueProvider
	{
		#region ICodeIssueProvider implementation
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase
		{
			readonly BaseRefactoringContext context;
			
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
				this.context = context;
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				if (invocationExpression.Target is IdentifierExpression)
					// Call within current class scope without 'this' or 'base'
					return;
				var expression = invocationExpression.Target as MemberReferenceExpression;
				if (expression == null || expression.Target is ThisReferenceExpression)
					// Call within current class scope using 'this' or 'base'
					return;
				var invocationResolveResult = context.Resolve(invocationExpression) as InvocationResolveResult;
				if (invocationResolveResult == null)
					return;
				if (!invocationResolveResult.Member.IsStatic)
					return;
				var targetResolveResult = invocationResolveResult.TargetResult as TypeResolveResult;
				if (targetResolveResult == null)
					return;
				if (targetResolveResult.Type.Equals(invocationResolveResult.Member.DeclaringType))
					return;
				AddIssue(invocationExpression.Target, context.TranslateString("Static method invoked via derived type"),
				         GetActions(context, invocationExpression, invocationResolveResult.Member));
			}

			IEnumerable<CodeAction> GetActions(BaseRefactoringContext context, InvocationExpression invocationExpression,
			                                   IMember member)
			{
				var csResolver = context.Resolver.GetResolverStateBefore(invocationExpression);
				var builder = new TypeSystemAstBuilder(csResolver);
				var newType = builder.ConvertType(member.DeclaringType);
				string description = string.Format("{0} '{1}'", context.TranslateString("Use base class"), newType.GetText());
				yield return new CodeAction(description, script => {
					script.Replace(((MemberReferenceExpression)invocationExpression.Target).Target, newType);
				});
			}
		}
		#endregion
	}
}

