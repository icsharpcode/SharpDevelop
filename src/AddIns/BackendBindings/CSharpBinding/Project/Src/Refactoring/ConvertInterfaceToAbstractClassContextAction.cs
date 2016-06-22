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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	[ContextAction("Convert interface to abstract class", Description = "Converts an interface to a class with abstract members.")]
	public class ConvertInterfaceToAbstractClassContextAction : ContextAction
	{
		public override async Task<bool> IsAvailableAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
		{
			SyntaxTree st = await context.GetSyntaxTreeAsync().ConfigureAwait(false);
			Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
			if (identifier == null)
				return false;
			TypeDeclaration typeDeclaration = identifier.Parent as TypeDeclaration;
			return (typeDeclaration != null) && (typeDeclaration.ClassType == ClassType.Interface);
		}

		public override void Execute(EditorRefactoringContext context)
		{
			CSharpFullParseInformation parseInformation = context.GetParseInformation() as CSharpFullParseInformation;
			if (parseInformation != null) {
				SyntaxTree st = parseInformation.SyntaxTree;
				Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
				if (identifier == null)
					return;
				TypeDeclaration interfaceTypeDeclaration = identifier.Parent as TypeDeclaration;
				if (interfaceTypeDeclaration != null) {
					// Generate abstract class from interface and abstract members from interface members
					TypeDeclaration abstractClassTypeNode = (TypeDeclaration) interfaceTypeDeclaration.Clone();
					abstractClassTypeNode.ClassType = ClassType.Class;
					abstractClassTypeNode.Modifiers |= Modifiers.Abstract;
					foreach (var entity in abstractClassTypeNode.Children.OfType<EntityDeclaration>()) {
						entity.Modifiers |= Modifiers.Abstract | Modifiers.Public;
					}
					
					var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
					using (Script script = refactoringContext.StartScript()) {
						// Replace interface node with node of abstract class
						script.Replace(interfaceTypeDeclaration, abstractClassTypeNode);
					}
				}
			}
		}

		public override string DisplayName
		{
			get {
				return "Convert interface to abstract class";
			}
		}
	}
}
