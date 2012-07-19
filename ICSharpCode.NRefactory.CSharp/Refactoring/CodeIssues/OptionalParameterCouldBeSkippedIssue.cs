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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Optional argument has default value and can be skipped",
	                  Description = "Finds calls to functions where optional parameters are used and the passed argument is the same as the default.",
	                  Category = IssueCategories.Redundancies,
	                  Severity = Severity.Hint,
	                  IssueMarker = IssueMarker.GrayOut)]
	public class OptionalParameterCouldBeSkippedIssue : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}
		
		class GatherVisitor : GatherVisitorBase
		{
			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			public override void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
			{
				base.VisitObjectCreateExpression(objectCreateExpression);

				var invocationResolveResult = ctx.Resolve(objectCreateExpression) as CSharpInvocationResolveResult;
				if (invocationResolveResult == null)
					return;
				var arguments = objectCreateExpression.Arguments.ToArray();
				
				var redundantArguments = GetRedundantArguments(arguments, invocationResolveResult);
				foreach (var argument in redundantArguments) {
					string actionMessage = GetActionTitle(objectCreateExpression.Arguments, argument);
					
					var action = new CodeAction(actionMessage, script => {
						var newArgumentList = objectCreateExpression.Arguments
							.Where(arg => arg.StartLocation < argument.StartLocation)
							.Select(arg => arg.Clone());
						var newInvocation = new ObjectCreateExpression(objectCreateExpression.Type.Clone(), newArgumentList);
						script.Replace(objectCreateExpression, newInvocation);
					});
					var issueMessage = ctx.TranslateString("Argument is identical to the default value");
					AddIssue(argument, issueMessage, new [] { action });
				}
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);

				var invocationResolveResult = ctx.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (invocationResolveResult == null)
					return;
				var arguments = invocationExpression.Arguments.ToArray();

				var redundantArguments = GetRedundantArguments(arguments, invocationResolveResult);
				foreach (var argument in redundantArguments) {
					string actionMessage = GetActionTitle(invocationExpression.Arguments, argument);
					
					var action = new CodeAction(actionMessage, script => {
						var newArgumentList = invocationExpression.Arguments
							.Where(arg => arg.StartLocation < argument.StartLocation)
								.Select(arg => arg.Clone());
						var newInvocation = new InvocationExpression(invocationExpression.Target.Clone(), newArgumentList);
						script.Replace(invocationExpression, newInvocation);
					});
					var issueMessage = ctx.TranslateString("Argument is identical to the default value");
					AddIssue(argument, issueMessage, new [] { action });
				}
			}

			IEnumerable<Expression> GetRedundantArguments(Expression[] arguments, CSharpInvocationResolveResult invocationResolveResult)
			{
				var argumentToParameterMap = invocationResolveResult.GetArgumentToParameterMap();
				var resolvedParameters = invocationResolveResult.Member.Parameters;

				for (int i = arguments.Length - 1; i >= 0; i--) {
					var parameter = resolvedParameters[argumentToParameterMap[i]];
					if (parameter.IsParams)
						// before params all optional arguments are needed, otherwise some of the
						// param arguments will be shifted out of the params into the fixed parameters
						break;
					if (!parameter.IsOptional)
						// There can be no optional parameters preceding a required one
						break;
					var argument = arguments[i] as PrimitiveExpression;
					if (argument == null)
						continue;
					var argumentResolveResult = ctx.Resolve(argument) as ConstantResolveResult;
					if (parameter.ConstantValue != argumentResolveResult.ConstantValue)
						// Stop here since any arguments before this one has to be there
						// to enable the passing of this argument
						break;
					yield return argument;
				}
			}

			string GetActionTitle(IEnumerable<Expression> arguments, Expression firstArgumentToRemove)
			{
				var redundantArgumentCount = arguments
					.Where(arg => arg.StartLocation >= firstArgumentToRemove.StartLocation)
					.Count();

				if (redundantArgumentCount == 1) {
					return ctx.TranslateString("Remove this argument");
				}
				else {
					return ctx.TranslateString("Remove this and the following arguments");
				}
			}
		}
	}
}

