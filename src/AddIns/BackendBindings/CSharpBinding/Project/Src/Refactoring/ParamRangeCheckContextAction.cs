// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	[ContextAction("Add range check for parameter", Description = "Adds a parameter range check.")]
	public class ParamRangeCheckContextAction : ContextAction
	{
		public override async Task<bool> IsAvailableAsync(EditorRefactoringContext context, System.Threading.CancellationToken cancellationToken)
		{
			SyntaxTree st = await context.GetSyntaxTreeAsync().ConfigureAwait(false);
			Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
			if (identifier == null)
				return false;
			ParameterDeclaration parameterDeclaration = identifier.Parent as ParameterDeclaration;
			if (parameterDeclaration == null)
				return false;
			ICompilation compilation = await context.GetCompilationAsync().ConfigureAwait(false);
			ITypeReference typeRef = parameterDeclaration.Type.ToTypeReference();
			if (typeRef == null)
				return false;
			IType type = typeRef.Resolve(compilation);
			if (type == null)
				return false;
			return type.HasRange();
		}
		
		public override void Execute(EditorRefactoringContext context)
		{
			CSharpFullParseInformation parseInformation = context.GetParseInformation() as CSharpFullParseInformation;
			if (parseInformation != null) {
				SyntaxTree st = parseInformation.SyntaxTree;
				Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
				if (identifier == null)
					return;
				ParameterDeclaration parameterDeclaration = identifier.Parent as ParameterDeclaration;
				if (parameterDeclaration == null)
					return;
				
				AstNode grandparent = identifier.Parent.Parent;
				if ((grandparent is MethodDeclaration) || (grandparent is ConstructorDeclaration)) {
					// Range check condition
					var rangeCheck = new IfElseStatement(
						new BinaryOperatorExpression(
							new BinaryOperatorExpression(new IdentifierExpression(identifier.Name), BinaryOperatorType.LessThan, new IdentifierExpression("lower")),
							BinaryOperatorType.ConditionalOr,
							new BinaryOperatorExpression(new IdentifierExpression(identifier.Name), BinaryOperatorType.GreaterThan, new IdentifierExpression("upper"))
						),
						new ThrowStatement(
							new ObjectCreateExpression(
								new SimpleType("ArgumentOutOfRangeException"),
								new List<Expression>() { new PrimitiveExpression(identifier.Name, '"' + identifier.Name + '"'), new IdentifierExpression(identifier.Name), new BinaryOperatorExpression(new PrimitiveExpression("Value must be between "), BinaryOperatorType.Add, new BinaryOperatorExpression(new IdentifierExpression("lower"), BinaryOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(" and "), BinaryOperatorType.Add, new IdentifierExpression("upper")))) }
							)
						)
					);
					
					// Add range check as first statement in method's/constructor's body
					var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
					using (Script script = refactoringContext.StartScript()) {
						if (grandparent is MethodDeclaration) {
							var methodDeclaration = (MethodDeclaration) grandparent;
							script.AddTo(methodDeclaration.Body, rangeCheck);
						} else if (grandparent is ConstructorDeclaration) {
							var ctorDeclaration = (ConstructorDeclaration) grandparent;
							script.AddTo(ctorDeclaration.Body, rangeCheck);
						}
					}
				}
			}
		}
		
		public override string DisplayName {
			get {
				return "Add range check for parameter";
			}
		}
	}
}
