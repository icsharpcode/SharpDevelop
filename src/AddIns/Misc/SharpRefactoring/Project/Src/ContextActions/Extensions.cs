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
		
		public static IEnumerable<IClass> GetClassesOnCurrentLine(this EditorASTProvider editorAST)
		{
			var currentLineAST = editorAST.CurrentLineAST;
			if (currentLineAST == null)
				yield break;
			var editor = editorAST.Editor;
			foreach (var declaration in currentLineAST.FindTypeDeclarations()) {
				
				var rr = ParserService.Resolve(new ExpressionResult(declaration.Name), editor.Caret.Line, editor.Caret.Column, editor.FileName, editor.Document.Text);
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
