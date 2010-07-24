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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;

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
		
		public static IEnumerable<IClass> GetClassDeclarationsOnCurrentLine(this EditorContext editorContext)
		{
			var currentLineAST = editorContext.CurrentLineAST;
			if (currentLineAST == null)
				yield break;
			var editor = editorContext.Editor;
			foreach (var declaration in currentLineAST.FindTypeDeclarations()) {
				int indexOfClassNameOnTheLine = editorContext.CurrentLine.Text.IndexOf(declaration.Name, declaration.StartLocation.Column/*, declaration.EndLocation.Column + 1 - declaration.StartLocation.Column*/);
				if (indexOfClassNameOnTheLine == -1)
					continue;
				int declarationOffset = editorContext.CurrentLine.Offset + indexOfClassNameOnTheLine;
				var rr = ParserService.Resolve(declarationOffset + 1, editor.Document, editor.FileName);
				if (rr != null && rr.ResolvedType != null) {
					var foundClass = rr.ResolvedType.GetUnderlyingClass();
					if (foundClass != null) {
						yield return foundClass;
					}
				}
			}
		}
	}
}
