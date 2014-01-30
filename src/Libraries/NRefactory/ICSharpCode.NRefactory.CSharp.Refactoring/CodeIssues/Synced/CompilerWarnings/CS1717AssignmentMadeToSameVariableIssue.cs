// 
// CS1717AssignmentMadeToSameVariableIssue.cs
// 
// Author:
//      Mansheng Yang <lightyang0@gmail.com>
// 
// Copyright (c) 2012 Mansheng Yang <lightyang0@gmail.com>
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
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
    [IssueDescription("CS1717:Assignment made to same variable",
					   Description = "CS1717:Assignment made to same variable.",
					   Category = IssueCategories.CompilerWarnings,
					   Severity = Severity.Warning,
                       PragmaWarning = 1717,
                       AnalysisDisableKeyword = "CSharpWarnings::CS1717")]
    public class CS1717AssignmentMadeToSameVariableIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<CS1717AssignmentMadeToSameVariableIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx)
				: base (ctx)
			{
			}

			public override void VisitAssignmentExpression (AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression (assignmentExpression);

				if (assignmentExpression.Operator != AssignmentOperatorType.Assign)
					return;
				if (!(assignmentExpression.Left is IdentifierExpression) && 
					!(assignmentExpression.Left is MemberReferenceExpression))
					return;

				var resolveResult = ctx.Resolve (assignmentExpression.Left);
				var memberResolveResult = resolveResult as MemberResolveResult;
				if (memberResolveResult != null) {
					var memberResolveResult2 = ctx.Resolve (assignmentExpression.Right) as MemberResolveResult;
					if (memberResolveResult2 == null || !AreEquivalent(memberResolveResult, memberResolveResult2))
						return;
				} else if (resolveResult is LocalResolveResult) {
					if (!assignmentExpression.Left.Match (assignmentExpression.Right).Success)
						return;
				} else {
					return;
				}

				AstNode node;
				Action<Script> action;
				if (assignmentExpression.Parent is ExpressionStatement) {
					node = assignmentExpression.Parent;
					action = script => script.Remove (assignmentExpression.Parent);
				} else {
					node = assignmentExpression;
					action = script => script.Replace (assignmentExpression, assignmentExpression.Left.Clone ());
				}
				AddIssue (new CodeIssue(node, ctx.TranslateString ("CS1717:Assignment made to same variable"),
					new [] { new CodeAction (ctx.TranslateString ("Remove assignment"), action, node) })
					{ IssueMarker = IssueMarker.GrayOut }
				);
			}

			static bool AreEquivalent(ResolveResult first, ResolveResult second)
			{
				var firstPath = AccessPath.FromResolveResult(first);
				var secondPath = AccessPath.FromResolveResult(second);
				return firstPath != null && firstPath.Equals(secondPath) && !firstPath.MemberPath.Any(m => !(m is IField));
			}
		}
	}
}
