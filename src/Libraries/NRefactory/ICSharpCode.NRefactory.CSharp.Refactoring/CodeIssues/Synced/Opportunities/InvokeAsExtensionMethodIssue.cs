//
// InvokeAsExtensionMethodIssue.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Convert static method call to extension method call",
	                  Description = "If an extension method is called as static method convert it to method syntax",
	                  Category = IssueCategories.Opportunities,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "InvokeAsExtensionMethod")]
	public class InvokeAsExtensionMethodIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		class GatherVisitor : GatherVisitorBase<InvokeAsExtensionMethodIssue>
		{
			public GatherVisitor (BaseRefactoringContext ctx) : base (ctx)
			{
			}

			public override void VisitInvocationExpression(InvocationExpression invocation)
			{
				base.VisitInvocationExpression(invocation);
				var memberReference = invocation.Target as MemberReferenceExpression;
				if (memberReference == null)
					return;
				var firstArgument = invocation.Arguments.FirstOrDefault();
				if (firstArgument is NullReferenceExpression)
					return;
				var invocationRR = ctx.Resolve(invocation) as CSharpInvocationResolveResult;
				if (invocationRR == null)
					return;
				var method = invocationRR.Member as IMethod;
				if (method == null || !method.IsExtensionMethod || invocationRR.IsExtensionMethodInvocation)
					return;

				AddIssue(new CodeIssue(
					memberReference.MemberNameToken,
					ctx.TranslateString("Convert static method call to extension method call"),
					ctx.TranslateString("Convert to extension method call"),
					script => {
						script.Replace (
							invocation, 
							new InvocationExpression(
								new MemberReferenceExpression(firstArgument.Clone(), memberReference.MemberName),
								invocation.Arguments.Skip(1).Select(arg => arg.Clone())
							)
						);
					}
				));
			}
		}

	}
}

