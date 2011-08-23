// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
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
		
		public static int GetPreviousLineEndOffset(this IDocument document, Location location)
		{
			var line = document.GetLineForOffset(document.PositionToOffset(location));
			if (line.LineNumber == 1)
				return -1;
			var previousLine = document.GetLine(line.LineNumber - 1);
			return previousLine.Offset + previousLine.TotalLength;
		}
		
		public static void RemoveRestOfLine(this IDocument document, int offset)
		{
			var line = document.GetLineForOffset(offset);
			int lineEndOffset = line.Offset + line.Length;
			document.Remove(offset, lineEndOffset - offset);
		}
		
		public static DomRegion DomRegion(this INode node)
		{
			return new DomRegion(node.StartLocation.Line, node.StartLocation.Column, node.EndLocation.Line, node.EndLocation.Column);
		}
		
		/// <summary>
		/// Inserts code at the next line after target AST node.
		/// </summary>
		public static void InsertCodeAfter(this ITextEditor editor, AbstractNode target, AbstractNode insert, bool updateCaretPos = false)
		{
			InsertCode(editor, target, insert, editor.Document.GetLineEndOffset(target.EndLocation), updateCaretPos);
		}
		
		/// <summary>
		/// Inserts code at the line before target AST node.
		/// </summary>
		public static void InsertCodeBefore(this ITextEditor editor, AbstractNode target, AbstractNode insert)
		{
			InsertCode(editor, target, insert, editor.Document.GetPreviousLineEndOffset(target.StartLocation), false);
		}
		
		public static void InsertCode(this ITextEditor editor, AbstractNode target, AbstractNode insert, int insertOffset, bool updateCaretPos)
		{
			if (insertOffset < 0)
				return;
			
			var regionCorrectVisitor = new SetRegionInclusionVisitor();
			insert.AcceptVisitor(regionCorrectVisitor, null);
			
			var doc = editor.Document;
			var codeGen = editor.Language.Properties.CodeGenerator;
			
			string indent = DocumentUtilitites.GetWhitespaceAfter(doc, doc.GetLineStartOffset(target.StartLocation));
			string code = codeGen.GenerateCode(insert, indent);
			
			doc.Insert(insertOffset, code);
			if (updateCaretPos) {
				editor.Caret.Offset = insertOffset + code.Length - 1;
			}
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