// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		/// <summary>
		/// C# only.
		/// </summary>
		public static void AddCodeToMethodStart(IMember m, ITextEditor textArea, string newCode)
		{
			int methodStart = FindMethodStartOffset(textArea.Document, m.BodyRegion);
			if (methodStart < 0)
				return;
			textArea.Select(methodStart, 0);
			using (textArea.Document.OpenUndoGroup()) {
				int startLine = textArea.Caret.Line;
				foreach (string newCodeLine in newCode.Split('\n')) {
					textArea.Document.Insert(textArea.Caret.Offset,
					                         DocumentUtilitites.GetLineTerminator(textArea.Document, textArea.Caret.Line) + newCodeLine);
				}
				int endLine = textArea.Caret.Line;
				textArea.Language.FormattingStrategy.IndentLines(textArea, startLine, endLine);
			}
		}
		
		/// <summary>
		/// C# only.
		/// </summary>
		public static int FindMethodStartOffset(IDocument document, DomRegion bodyRegion)
		{
			if (bodyRegion.IsEmpty)
				return -1;
			int offset = document.PositionToOffset(bodyRegion.BeginLine, bodyRegion.BeginColumn);
			while (offset < document.TextLength) {
				if (document.GetCharAt(offset) == '{') {
					return offset + 1;
				}
				offset++;
			}
			return -1;
		}
	}
}
