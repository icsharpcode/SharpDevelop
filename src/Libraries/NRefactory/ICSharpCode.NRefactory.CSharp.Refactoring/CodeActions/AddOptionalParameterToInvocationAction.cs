//
// AddOptionalParameterToInvocationAction.cs
//
// Author:
//      Luís Reis <luiscubal@gmail.com>
//
// Copyright (c) 2013 Luís Reis
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
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Add one or more optional parameters to an invocation, using their default values",
	               Description = "Add one or more optional parameters to an invocation.")]
	public class AddOptionalParameterToInvocationAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var invocationExpression = context.GetNode<InvocationExpression>();
			if (invocationExpression == null)
				yield break;

			var resolveResult = context.Resolve(invocationExpression) as InvocationResolveResult;
			if (resolveResult == null) {
				yield break;
			}

			var method = (IMethod)resolveResult.Member;

			bool foundOptionalParameter = false;
			foreach (var parameter in method.Parameters) {
				if (parameter.IsParams) {
					yield break;
				}

				if (parameter.IsOptional) {
					foundOptionalParameter = true;
					break;
				}
			}

			if (!foundOptionalParameter) {
				yield break;
			}

			//Basic sanity checks done, now see if there are any missing optional arguments
			var missingParameters = new List<IParameter>(method.Parameters);
			if (resolveResult.Arguments.Count != invocationExpression.Arguments.Count) {
				//Extension method
				missingParameters.RemoveAt (0);
			}
			foreach (var argument in invocationExpression.Arguments) {
				var namedArgument = argument as NamedArgumentExpression;
				if (namedArgument == null) {
					missingParameters.RemoveAt(0);
				} else {
					missingParameters.RemoveAll(parameter => parameter.Name == namedArgument.Name);
				}
			}

			foreach (var parameterToAdd in missingParameters) {
				//Add specific parameter
				yield return new CodeAction(string.Format(context.TranslateString("Add optional parameter \"{0}\""),
				                                         parameterToAdd.Name),
				                           script => {

					var newInvocation = (InvocationExpression)invocationExpression.Clone();
					AddArgument(newInvocation, parameterToAdd, parameterToAdd == missingParameters.First());
					script.Replace(invocationExpression, newInvocation);

				}, invocationExpression);
			}

			if (missingParameters.Count > 1) {
				//Add all parameters at once
				yield return new CodeAction(context.TranslateString("Add all optional parameters"),
				                            script => {

					var newInvocation = (InvocationExpression)invocationExpression.Clone();

					foreach (var parameterToAdd in missingParameters) {
						//We'll add the remaining parameters, in the order they were declared in the function
						AddArgument(newInvocation, parameterToAdd, true);
					}
					script.Replace(invocationExpression, newInvocation);

				}, invocationExpression);
			}
		}

		static void AddArgument(InvocationExpression newNode, IParameter parameterToAdd, bool isNextInSequence)
		{
			Expression defaultValue;
			if (parameterToAdd.ConstantValue == null) {
				defaultValue = new NullReferenceExpression();
			}
			else {
				defaultValue = new PrimitiveExpression(parameterToAdd.ConstantValue);
			}
			Expression newArgument;
			if (newNode.Arguments.Any(argument => argument is NamedExpression) || !isNextInSequence) {
				newArgument = new NamedArgumentExpression(parameterToAdd.Name, defaultValue);
			}
			else {
				newArgument = defaultValue;
			}
			newNode.Arguments.Add(newArgument);
		}
	}
}

