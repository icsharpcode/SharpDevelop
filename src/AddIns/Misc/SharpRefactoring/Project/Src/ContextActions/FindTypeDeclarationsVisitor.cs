// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Finds TypeDeclarations in AST tree.
	/// </summary>
	public class FindTypeDeclarationsVisitor : AbstractAstVisitor
	{
		List<TypeDeclaration> foundDeclarations;
		public ReadOnlyCollection<TypeDeclaration> Declarations { get; private set; }
		
		public FindTypeDeclarationsVisitor()
		{
			this.foundDeclarations = new List<TypeDeclaration>();
			this.Declarations = this.foundDeclarations.AsReadOnly();
		}
		
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			this.foundDeclarations.Add(typeDeclaration);
			return base.VisitTypeDeclaration(typeDeclaration, data);
		} 
	}
}
