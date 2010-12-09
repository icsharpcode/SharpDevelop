// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ExtendSelection.
	/// </summary>
	public class ExtendSelection
	{
		public static void Run(ITextEditor editor)
		{
			var editorLang = EditorContext.GetEditorLanguage(editor);
			if (editorLang == null) return;
			var parser = ParserFactory.CreateParser(editorLang.Value, editor.Document.CreateReader());
			if (parser == null) return;
			parser.ParseMethodBodies = true;
			parser.Lexer.SkipAllComments = false;
			parser.Parse();
			var parsedCU = parser.CompilationUnit;
			if (parsedCU == null) return;
			var selectionStart = editor.Document.OffsetToPosition(editor.SelectionStart);
			var selectionEnd = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			foreach (var node in parsedCU.Children) {
				// fix StartLocation / EndLocation
				node.AcceptVisitor(new ICSharpCode.NRefactory.Visitors.SetRegionInclusionVisitor(), null);
			}
			Ast.INode currentNode = parsedCU.Children.Select(
				n => EditorContext.FindInnermostNodeContainingSelection(n, selectionStart, selectionEnd)).Where(n => n != null).FirstOrDefault();
			if (currentNode == null) return;

			// whole node already selected -> expand selection
			if (currentNode.StartLocation == selectionStart && currentNode.EndLocation == selectionEnd) {
				var commentsBlankLines = parser.Lexer.SpecialTracker.CurrentSpecials;
				
				// if there is a comment block immediately before selection, or behind selection on the same line, add it to selection
				var comments = commentsBlankLines.Where(s => s is Comment).Cast<Comment>().ToList();
				int commentIndex = comments.FindIndex(c => c.EndPosition.Line == currentNode.StartLocation.Line);
				if (commentIndex >= 0 && IsWhitespaceBetween(editor.Document, comments[commentIndex].EndPosition, selectionStart)) {
					while (commentIndex >= 0 && comments[commentIndex].EndPosition.Line == selectionStart.Line)
					{
						var comment = comments[commentIndex];
						selectionStart = comment.StartPosition;
						if (comment.CommentStartsLine)
							selectionStart.Column = 1;
						commentIndex--;
					}
				}
				else {
					// if the extended selection would contain blank lines, extend the selection only to the blank lines/comments on both sides (use siblings)
					//   if the selection contains blank lines or comments on both sides, dont do this
					
					
					var parent = currentNode.Parent;
					// it can happen that parent region exactly matches child region - in this case we need to advance even to the next parent
					// bc otherwise the selection would never move
					while (parent != null && parent.StartLocation == selectionStart && parent.EndLocation == selectionEnd) {
						parent = parent.Parent;
					}
					if (parent == null)
						return;
					selectionStart = parent.StartLocation;
					selectionEnd = parent.EndLocation;
				}
			} else {
				// select current node
				selectionStart = currentNode.StartLocation;
				selectionEnd = currentNode.EndLocation;
			}
			int startOffset, endOffset;
			try {
				startOffset = editor.Document.PositionToOffset(selectionStart);
				endOffset = editor.Document.PositionToOffset(selectionEnd);
			} catch(ArgumentOutOfRangeException) {
				return;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
		
		static bool IsWhitespaceBetween(IDocument document, Location startPos, Location endPos)
		{
			int startOffset = document.PositionToOffset(startPos);
			int endOffset = document.PositionToOffset(endPos);
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
	}
}
