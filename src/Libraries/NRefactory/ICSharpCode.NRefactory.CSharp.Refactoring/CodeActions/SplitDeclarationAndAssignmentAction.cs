//
// SplitDeclarationAndAssignmentAction.cs
// 
// SplitDeclarationAndAssignment.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Mike Krüger <mkrueger@novell.com>
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
using System.Linq;
using ICSharpCode.NRefactory.PatternMatching;
using System.Threading;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Split local variable declaration and assignment", Description = "Splits local variable declaration and assignment.")]
	public class SplitDeclarationAndAssignmentAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			if (context.IsSomethingSelected) {
				yield break;
			}
			AstType type;
			var varInitializer = GetVariableDeclarationStatement(context, out type);
			if (varInitializer == null)
				yield break;
			var statement = varInitializer.GetParent<Statement>();
			var declaration = varInitializer.GetParent<VariableDeclarationStatement>();
			if (declaration == null || (declaration.Modifiers & Modifiers.Const) != 0)
				yield break;

			var selectedNode = varInitializer.GetNodeAt(context.Location) ?? varInitializer;

			yield return new CodeAction(context.TranslateString("Split local variable declaration and assignment"), script => {
				var assign = new AssignmentExpression (new IdentifierExpression (varInitializer.Name), AssignmentOperatorType.Assign, varInitializer.Initializer.Clone());

				if (declaration != null && declaration.Type.IsVar())
					script.Replace(declaration.Type, type);
				if (declaration.Parent is ForStatement) {
					script.InsertBefore(statement, new VariableDeclarationStatement (type, varInitializer.Name));
					script.Replace(declaration, assign);
				} else {
					script.Replace(varInitializer, new IdentifierExpression (varInitializer.Name));
					script.InsertAfter(statement, new ExpressionStatement (assign));
				}

			}, selectedNode);
		}
		
		static VariableInitializer GetVariableDeclarationStatement (RefactoringContext context, out AstType resolvedType, CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = context.GetNode<VariableInitializer> ();
			if (result != null && !result.Initializer.IsNull && context.Location <= result.Initializer.StartLocation) {
				var type = context.Resolve(result).Type;
				if (type.Equals(SpecialType.NullType) || type.Equals(SpecialType.UnknownType)) {
					resolvedType = new PrimitiveType ("object");
				} else {
					resolvedType = context.CreateShortType (type);
				}
				return result;
			}
			resolvedType = null;
			return null;
		}
	}
}

