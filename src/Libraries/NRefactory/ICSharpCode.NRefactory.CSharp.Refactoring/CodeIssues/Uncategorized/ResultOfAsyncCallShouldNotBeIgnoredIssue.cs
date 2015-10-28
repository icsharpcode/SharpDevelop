// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
//	[IssueDescription("Result of async call is ignored",
//	                  Description = "Warns when the task returned by an async call is ignored, which causes exceptions" +
//	                  " thrown by the call to be silently ignored.",
//	                  Category = IssueCategories.CodeQualityIssues,
//	                  Severity = Severity.Warning)]
	public class ResultOfAsyncCallShouldNotBeIgnoredIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}
		
		sealed class GatherVisitor : GatherVisitorBase<ResultOfAsyncCallShouldNotBeIgnoredIssue>
		{
			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
			}

			AstNode GetNodeToUnderline(Expression target)
			{
				if (target is IdentifierExpression)
					return target;
				if (target is MemberReferenceExpression)
					return ((MemberReferenceExpression)target).MemberNameToken;
				return target;
			}
			
			public override void VisitExpressionStatement(ExpressionStatement expressionStatement)
			{
				base.VisitExpressionStatement(expressionStatement);
				var invocation = expressionStatement.Expression as InvocationExpression;
				if (invocation == null)
					return;
				var rr = ctx.Resolve(invocation) as InvocationResolveResult;
				if (rr != null && (rr.Type.IsKnownType(KnownTypeCode.Task) || rr.Type.IsKnownType(KnownTypeCode.TaskOfT))) {
					AddIssue(new CodeIssue(GetNodeToUnderline (invocation.Target), ctx.TranslateString("Exceptions in async call will be silently ignored because the returned task is unused")));
				}
			}
		}
	}
}
