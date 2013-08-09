// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
