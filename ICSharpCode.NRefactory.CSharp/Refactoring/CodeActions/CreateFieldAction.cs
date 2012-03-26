// 
// CreateField.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using ICSharpCode.NRefactory.PatternMatching;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Create field", Description = "Creates a field for a undefined variable.")]
	public class CreateFieldAction : ICodeActionProvider
	{
		public IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var identifier = context.GetNode<IdentifierExpression>();
			if (identifier == null) {
				yield break;
			}
			var statement = context.GetNode<Statement>();
			if (statement == null) {
				yield break;
			}

			if (!(context.Resolve(identifier).IsError)) {
				yield break;
			}
			var guessedType = CreateFieldAction.GuessType(context, identifier);
			if (guessedType == null) {
				yield break;
			}

			yield return new CodeAction(context.TranslateString("Create field"), script => {
				var decl = new FieldDeclaration() {
					ReturnType = guessedType,
					Variables = { new VariableInitializer(identifier.Identifier) }
				};
				script.InsertWithCursor(context.TranslateString("Create field"), decl, Script.InsertPosition.Before);
			});
		}

		#region Type guessing
		static int GetArgumentIndex(InvocationExpression invoke, AstNode parameter)
		{
			int argumentNumber = 0;
			foreach (var arg in invoke.Arguments) {
				if (arg == parameter) {
					return argumentNumber;
				}
				argumentNumber++;
			}
			return -1;
		}

		static IEnumerable<IType> GetAllValidTypesFromInvokation(RefactoringContext context, InvocationExpression invoke, AstNode parameter)
		{
			int index = GetArgumentIndex(invoke, parameter);
			if (index < 0) {
				yield break;
			}
					
			var targetResult = context.Resolve(invoke.Target);
			if (targetResult is MethodGroupResolveResult) {
				foreach (var method in ((MethodGroupResolveResult)targetResult).Methods) {
					if (index < method.Parameters.Count) {
						yield return method.Parameters [index].Type;
					}
				}
			}
		}

		internal static IEnumerable<IType> GetValidTypes(RefactoringContext context, Expression expr)
		{
			if (expr.Parent is DirectionExpression) {
				var parent = expr.Parent.Parent;
				if (parent is InvocationExpression) {
					var invoke = (InvocationExpression)parent;
					return GetAllValidTypesFromInvokation(context, invoke, expr.Parent);
				}
			}

			if (expr.Parent is InvocationExpression) {
				var parent = expr.Parent;
				if (parent is InvocationExpression) {
					var invoke = (InvocationExpression)parent;
					return GetAllValidTypesFromInvokation(context, invoke, expr);
				}
			}

			if (expr.Parent is AssignmentExpression) {
				var assign = (AssignmentExpression)expr.Parent;
				var other = assign.Left == expr ? assign.Right : assign.Left;
				return new [] { context.Resolve(other).Type };
			}

			if (expr.Parent is BinaryOperatorExpression) {
				var assign = (BinaryOperatorExpression)expr.Parent;
				var other = assign.Left == expr ? assign.Right : assign.Left;
				return new [] { context.Resolve(other).Type };
			}

			return Enumerable.Empty<IType>();
		}

		internal static AstType GuessType(RefactoringContext context, IdentifierExpression identifier)
		{
			IType type = null;
			foreach (var t in GetValidTypes(context, identifier)) {
				if (type == null || type.GetAllBaseTypes().Contains(t)) {
					type = t;
					continue;
				}

				if (!t.GetAllBaseTypes().Contains(type)) {
					type = null;
					break;
				}
			}

			if (type == null) {
				return new PrimitiveType("object");
			}

			return context.CreateShortType (type);
		}
		#endregion
	}
}

