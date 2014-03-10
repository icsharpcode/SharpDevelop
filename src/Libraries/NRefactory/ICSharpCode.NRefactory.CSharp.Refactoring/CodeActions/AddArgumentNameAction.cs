// 
// AddArgumentNameAction.cs
//  
// Author:
//       Ji Kun <jikun.nus@gmail.com>
// 
// Copyright (c) 2013 Ji Kun <jikun.nus@gmail.com>
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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	///  Add name for argument
	/// </summary>
	[ContextAction("Add name for argument", Description = "Add name for argument including method, indexer invocation and Attibute Usage")]
	public class AddArgumentNameAction : SpecializedCodeAction<Expression>
	{
		List<Expression> CollectNodes(AstNode parant, AstNode node)
		{
			List<Expression> returned = new List<Expression>();

			var children = parant.GetChildrenByRole(Roles.Argument);
			for (int i = 0; i< children.Count(); i++) {
				if (children.ElementAt(i).Equals(node)) {
					for (int j = i; j< children.Count(); j++) {
						if (children.ElementAt(j) is Expression && children.ElementAt(j).Role == Roles.Argument && !(children.ElementAt(j) is NamedArgumentExpression)) {
							returned.Add(children.ElementAt(j));
						} else {
							break;
						}
					}
				}
			}
			return returned;
		}

		protected override CodeAction GetAction(RefactoringContext context, Expression expression)
		{	
			if (expression == null)
				return null;
			if (expression.Role != Roles.Argument || expression is NamedArgumentExpression)
				return null;
			if (context.Location != expression.StartLocation)
				return null;
			var parent = expression.Parent;
			if (!(parent is CSharp.Attribute) && !(parent is IndexerExpression) && !(parent is InvocationExpression))
				return null;

			var attribute = parent as CSharp.Attribute;
			if (attribute != null) {
				var resolvedResult = context.Resolve(attribute) as CSharpInvocationResolveResult;
				if (resolvedResult == null || resolvedResult.IsError)
					return null;
				var arguments = attribute.Arguments;
				IMember member = resolvedResult.Member;

				int index = 0;
				int temp = 0; 
				List<Expression> nodes = new List<Expression>();
				foreach (var argument in arguments) {
					if (argument.Equals(expression)) {
						nodes = CollectNodes(parent, expression);
						break;
					}
					temp++;
				}
				index = temp;
				if (!nodes.Any())
					return null;
				var method = member as IMethod;
				if (method == null || method.Parameters.Count == 0 || method.Parameters.Last().IsParams)
					return null;

				var parameterMap = resolvedResult.GetArgumentToParameterMap();
				var parameters = method.Parameters;
				if (index >= parameterMap.Count)
					return null;
				var name = parameters[parameterMap[index]].Name;
				return new CodeAction(string.Format(context.TranslateString("Add argument name '{0}'"), name), script => {
					for (int i = 0; i < nodes.Count; i++) {
						int p = index + i;
						if (p >= parameterMap.Count)
							break;
						name = parameters[parameterMap[p]].Name;
						var namedArgument = new NamedArgumentExpression(name, arguments.ElementAt(p).Clone());
						script.Replace(arguments.ElementAt(p), namedArgument);
					}}, 
				expression
				);
			} 

			var indexerExpression = parent as IndexerExpression;
			if (indexerExpression != null) {
				var resolvedResult = context.Resolve(indexerExpression) as CSharpInvocationResolveResult;
				if (resolvedResult == null || resolvedResult.IsError)
					return null;
				var arguments = indexerExpression.Arguments;
				IMember member = resolvedResult.Member;
				
				int index = 0;
				int temp = 0; 
				List<Expression> nodes = new List<Expression>();
				foreach (var argument in arguments) {
					if (argument.Equals(expression)) {
						nodes = CollectNodes(parent, expression);
						break;
					}
					temp++;
				}
				index = temp;
				if (!nodes.Any())
					return null;
				var property = member as IProperty;
				if (property == null || property.Parameters.Count == 0 || property.Parameters.Last().IsParams) {
					return null;
				}
				var parameterMap = resolvedResult.GetArgumentToParameterMap();
				var parameters = property.Parameters;
				if (index >= parameterMap.Count)
					return null;
				var name = parameters[parameterMap[index]].Name;
				return new CodeAction(string.Format(context.TranslateString("Add argument name '{0}'"), name), script => {
					for (int i = 0; i< nodes.Count; i++) {
						int p = index + i;
						if (p >= parameterMap.Count)
							break;
						name = parameters[parameterMap[p]].Name;
						var namedArgument = new NamedArgumentExpression(name, arguments.ElementAt(p).Clone());
						script.Replace(arguments.ElementAt(p), namedArgument);
					}}, 
				expression
				);
			} 

			var invocationExpression = parent as InvocationExpression;
			if (invocationExpression != null) {
				var resolvedResult = context.Resolve(invocationExpression) as CSharpInvocationResolveResult;
				if (resolvedResult == null || resolvedResult.IsError)
					return null;
				var arguments = invocationExpression.Arguments;
				IMember member = resolvedResult.Member;
				
				int index = 0;
				int temp = 0; 
				List<Expression> nodes = new List<Expression>();
				foreach (var argument in arguments) {
					if (argument.Equals(expression)) {
						nodes = CollectNodes(parent, expression);
						break;
					}
					temp++;
				}
				index = temp;
				if (!nodes.Any())
					return null;
			
				var method = member as IMethod;
				if (method == null || method.Parameters.Count == 0 || method.Parameters.Last().IsParams)
					return null;

				var parameterMap = (resolvedResult as CSharpInvocationResolveResult).GetArgumentToParameterMap();
				var parameters = method.Parameters;
				if (index >= parameterMap.Count)
					return null;
				var name = parameters[parameterMap[index]].Name;
				return new CodeAction(string.Format(context.TranslateString("Add argument name '{0}'"), name), script => {
					for (int i = 0; i< nodes.Count; i++) {
						int p = index + i;
						if (p >= parameterMap.Count)
							break;
						name = parameters[parameterMap[p]].Name;
						var namedArgument = new NamedArgumentExpression(name, arguments.ElementAt(p).Clone());
						script.Replace(arguments.ElementAt(p), namedArgument);
					}}, 
				expression
				);
			}
			return null;
		}
	}
}
