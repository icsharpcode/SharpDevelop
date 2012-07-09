//
// OptionalParameterCouldBeSkippedIssue.cs
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Optional argument has default value and can be skipped",
	                  Description = "Finds calls to functions where optional parameters are used and the passed argument is the same as the default.",
	                  Category = IssueCategories.Redundancies,
	                  Severity = Severity.Hint)]
	public class OptionalParameterCouldBeSkippedIssue : ICodeIssueProvider
	{
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
				var invocationResolveResult = context.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				var argumentToParameterMap = invocationResolveResult.GetArgumentToParameterMap();
				var resolvedParameters = invocationResolveResult.Member.Parameters;
				var arguments = invocationExpression.Arguments.ToArray();

				for (int i = arguments.Length - 1; i >= 0; i--) {
					var parameter = resolvedParameters[argumentToParameterMap[i]];
					if (parameter.IsParams)
						// before params optional arguments are needed, or some of the
						// param arguments will be shifted out of the params into the fixed parameters
						break;
					if (!parameter.IsOptional)
						continue;
					var argument = arguments[i] as PrimitiveExpression;
					if (argument == null)
						continue;
					var argumentResolveResult = context.Resolve(argument) as ConstantResolveResult;
					if (parameter.ConstantValue != argumentResolveResult.ConstantValue)
						// Stop here since any arguments before this one has to be there
						// enable passing this argument
						break;
					var message = context.TranslateString("Argument is identical to the default value");
					AddIssue(argument, message, GetRemoveNodeAction(invocationExpression, argument));
				}
			}

			IEnumerable<CodeAction> GetRemoveNodeAction(InvocationExpression invocationExpression, Expression firstArgumentToRemove)
			{
				var argumentsToRemove = invocationExpression.Arguments
					.Where(arg => arg.StartLocation >= firstArgumentToRemove.StartLocation).ToArray();

				string message;
				if (argumentsToRemove.Length == 1) {
					message = context.TranslateString("Remove this argument");
				} else {
					message = context.TranslateString("Remove this and the following arguments");
				}
				yield return new CodeAction(message, script => {
					var newArgumentList = invocationExpression.Arguments
						.Where(arg => !argumentsToRemove.Contains(arg))
							.Select(argument => argument.Clone());
					var newInvocation = new InvocationExpression(invocationExpression.Target.Clone(), newArgumentList);
					script.Replace(invocationExpression, newInvocation);

				});
			}
		}
	}
}

