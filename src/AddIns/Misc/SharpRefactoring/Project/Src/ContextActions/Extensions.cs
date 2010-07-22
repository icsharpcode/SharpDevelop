// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.ObjectModel;
using ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring.ContextActions
{
	public static class Extensions
	{
		/// <summary>
		/// Finds TypeDeclarations in AST tree.
		/// </summary>
		public static ReadOnlyCollection<TypeDeclaration> FindTypeDeclarations(this INode astTree)
		{
			var findVisitor = new FindTypeDeclarationsVisitor();
			astTree.AcceptVisitor(findVisitor, null);
			return findVisitor.Declarations;
		}
	}
}
