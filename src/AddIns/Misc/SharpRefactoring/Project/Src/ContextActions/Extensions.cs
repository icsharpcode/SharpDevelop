// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
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
		
		public static Location GetStart(this DomRegion region)
		{
			return new Location(region.BeginColumn, region.BeginLine);
		}
		
		public static Location GetEnd(this DomRegion region)
		{
			return new Location(region.EndColumn, region.EndLine);
		}
		
		public static int PositionToOffset(this IDocument document, Location location)
		{
			return document.PositionToOffset(location.Line, location.Column);
		}
		
		/// <summary>
		/// Gets offset for the start of line at which given location is.
		/// </summary>
		public static int GetLineStartOffset(this IDocument document, Location location)
		{
			return document.GetLineForOffset(document.PositionToOffset(location)).Offset;
		}
		
		public static int GetLineEndOffset(this IDocument document, Location location)
		{
			var line = document.GetLineForOffset(document.PositionToOffset(location));
			return line.Offset + line.TotalLength;
		}
		
		public static void RemoveRestOfLine(this IDocument document, int offset)
		{
			var line = document.GetLineForOffset(offset);
			int lineEndOffset = line.Offset + line.Length;
			document.Remove(offset, lineEndOffset - offset);
		}
	}
}
