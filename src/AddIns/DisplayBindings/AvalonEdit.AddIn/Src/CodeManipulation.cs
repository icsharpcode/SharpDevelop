// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Description of ExtendSelection.
	/// </summary>
	public class CodeManipulation
	{
		enum MoveStatementDirection { Up, Down };
		
		class Selection
		{
			public Location Start { get; set; }
			public Location End { get; set; }
		}
		
		public static void MoveStatementUp(ITextEditor editor)
		{
			MoveStatement(editor, MoveStatementDirection.Up);
			editor.ClearSelection();
		}
		
		public static void MoveStatementDown(ITextEditor editor)
		{
			MoveStatement(editor, MoveStatementDirection.Down);
			editor.ClearSelection();
		}
		
		// move selection - find outermost node in selection, swap selection with closest child of its parent to the selection
		static void MoveStatement(ITextEditor editor, MoveStatementDirection direction)
		{
			IList<ISpecial> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			// Find the Statement or Definition containing caret -> Extend selection to Statement or Definition
			
			INode currentStatement;
			Selection statementSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out currentStatement, new Type[] {
			                                               	typeof(Statement),
			                                               	typeof(MemberNode),
			                                               	typeof(FieldDeclaration),
			                                               	typeof(ConstructorDeclaration),
			                                               	typeof(DestructorDeclaration) });
			if (currentStatement == null)
				return;
			statementSelection = TryExtendSelectionToComments(editor.Document, statementSelection, commentsBlankLines);
			// Take its sibling
			if (currentStatement.Parent == null)
				return;
			var siblings = currentStatement.Parent.Children;
			int currentStatementPos = siblings.IndexOf(currentStatement);
			int swapIndex = currentStatementPos + (direction == MoveStatementDirection.Down ? 1 : -1);
			if (swapIndex < 0 || swapIndex >= siblings.Count)
				return;
			INode swapSibling = siblings[swapIndex];
			// Swap them
			string currentNodeText = editor.Document.GetText(statementSelection.Start, statementSelection.End);
			SwapText(editor.Document, statementSelection.Start, statementSelection.End, swapSibling.StartLocation, swapSibling.EndLocation);
			// Move caret to the start of moved statement
			Location upperLocation = new Location[] {statementSelection.Start, swapSibling.StartLocation}.Min();
			if (direction == MoveStatementDirection.Up)
				editor.Caret.Position = upperLocation;
			else {
				// look where current statement ended because it is hard to calculate it correctly
				int currentMovedOffset = editor.Document.Text.IndexOf(currentNodeText, editor.Document.PositionToOffset(upperLocation));
				editor.Caret.Offset = currentMovedOffset;
			}
		}
		
		static Selection TryExtendSelectionToComments(IDocument document, Selection selection, IList<ISpecial> commentsBlankLines)
		{
			var extendedToComments = ExtendSelectionToComments(document, selection, commentsBlankLines);
			if (extendedToComments != null)
				return extendedToComments;
			return selection;
		}
		
		public static void ExtendSelection(ITextEditor editor)
		{
			INode selectedNode = null;
			IList<ISpecial> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			Selection extendedSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out selectedNode, new Type[] { typeof(INode) });	// any node type
			
			SelectText(extendedSelection, editor);
		}
		
		// could work to extend selection to set of adjacent statements - e.g. select 3 lines
		static Selection ExtendSelection(ITextEditor editor, CompilationUnit parsedCU, IList<ISpecial> commentsBlankLines, out INode selectedResultNode, Type[] interestingNodeTypes)
		{
			selectedResultNode = null;
			
			var selectionStart = editor.Document.OffsetToPosition(editor.SelectionStart);
			var selectionEnd = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			Ast.INode currentNode = parsedCU.Children.Select(
				n => EditorContext.FindInnermostNodeContainingSelection(n, selectionStart, selectionEnd)).Where(n => n != null).FirstOrDefault();
			if (currentNode == null) return null;
			if (!IsNodeTypeInteresting(currentNode, interestingNodeTypes)) {
				// ignore uninteresting nodes in the AST
				currentNode = GetInterestingParent(currentNode, interestingNodeTypes);
			}
			if (currentNode == null) return null;
			selectedResultNode = currentNode;

			// whole node already selected -> expand selection
			if (currentNode.StartLocation == selectionStart && currentNode.EndLocation == selectionEnd) {
				
				// selected node stays the same
				var selectionExtendedToComments = ExtendSelectionToComments(editor.Document, selectionStart, selectionEnd, commentsBlankLines);
				if (selectionExtendedToComments != null) {
					// Can be extended to comments -> extend
					selectionStart = selectionExtendedToComments.Start;
					selectionEnd = selectionExtendedToComments.End;
				}
				else {
					var parent = GetInterestingParent(currentNode, interestingNodeTypes);
					// it can happen that parent region exactly matches child region - in this case we need to advance even to the next parent
					// bc otherwise the selection would never move
					while (parent != null && parent.StartLocation == selectionStart && parent.EndLocation == selectionEnd) {
						parent = GetInterestingParent(parent, interestingNodeTypes);
					}
					if (parent == null) {
						return new Selection { Start = selectionStart, End = selectionEnd };
					}
					// Select the parent
					var extendedSelectionStart = parent.StartLocation;
					var extendedLocationEnd = parent.EndLocation;
					selectedResultNode = parent;
					
					// if the extended selection would contain blank lines, extend the selection only to the blank lines/comments on both sides (use siblings)
					//   if the selection contains blank lines or comments on both sides, dont do this
					var blankLines = commentsBlankLines.Where(s => s is BlankLine).Cast<BlankLine>().ToList();
					//if (SelectionContainsBlankLines(extendedSelectionStart, extendedLocationEnd, blankLines)) {
					if (false) { // implement later
						
					} else {
						selectionStart = extendedSelectionStart;
						selectionEnd = extendedLocationEnd;
					}
				}
			} else {
				// select current node
				selectionStart = currentNode.StartLocation;
				selectionEnd = currentNode.EndLocation;
				selectedResultNode = currentNode;
			}
			return new Selection { Start = selectionStart, End = selectionEnd };
		}
		
		static Selection ExtendSelectionToComments(IDocument document, Selection selection, IList<ISpecial> commentsBlankLines)
		{
			if (selection == null)
				throw new ArgumentNullException("selection");
			return ExtendSelectionToComments(document, selection.Start, selection.End, commentsBlankLines);
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, or behind selection on the same line, add it to selection.
		/// </summary>
		static Selection ExtendSelectionToComments(IDocument document, Location selectionStart, Location selectionEnd, IList<ISpecial> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(s => s is Comment).Cast<Comment>().ToList();
			int commentIndex = comments.FindIndex(c => c.EndPosition.Line == selectionStart.Line);
			if (commentIndex >= 0 && IsWhitespaceBetween(document, comments[commentIndex].EndPosition, selectionStart)) {
				var extendedSelection = new Selection { Start = selectionStart, End = selectionEnd };
				while (commentIndex >= 0 && comments[commentIndex].EndPosition.Line == extendedSelection.Start.Line)
				{
					var comment = comments[commentIndex];
					// Move selection start to include comment
					extendedSelection.Start = comment.StartPosition;
					if (comment.CommentStartsLine)
						extendedSelection.Start = new Location(1, extendedSelection.Start.Line);
					
					commentIndex--;
				}
				return extendedSelection;
			} else {
				return null;
			}
		}
		
		/// <summary>
		/// Searches for parent of interesting type. Skips uninteresting parents.
		/// </summary>
		static INode GetInterestingParent(INode node, Type[] interestingNodeTypes)
		{
			var parent = node.Parent;
			while(parent != null) {
				if (IsNodeTypeInteresting(parent, interestingNodeTypes))
					return parent;
				parent = parent.Parent;
			}
			return null;
		}
		
		static bool IsNodeTypeInteresting(INode node, Type[] interestingNodeTypes)
		{
			Type nodeType = node.GetType();
			return interestingNodeTypes.Any(interestingType => interestingType.IsAssignableFrom(nodeType) || (nodeType == interestingType));
		}
		
		static bool SelectionContainsBlankLines(Location selectionStart, Location selectionEnd, List<BlankLine> blankLines)
		{
			return blankLines.Exists(b => b.StartPosition >= selectionStart && b.EndPosition <= selectionEnd);
		}
		
		static bool IsWhitespaceBetween(IDocument document, Location startPos, Location endPos)
		{
			int startOffset = document.PositionToOffset(startPos);
			int endOffset = document.PositionToOffset(endPos);
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
		
		// could depend just on IDocument
		static CompilationUnit ParseDocument(ITextEditor editor, out IList<ISpecial> parsedSpecials)
		{
			parsedSpecials = null;
			var editorLang = EditorContext.GetEditorLanguage(editor);
			if (editorLang == null) return null;
			var parser = ParserFactory.CreateParser(editorLang.Value, editor.Document.CreateReader());
			if (parser == null) return null;
			parser.ParseMethodBodies = true;
			parser.Lexer.SkipAllComments = false;
			parser.Parse();
			var parsedCU = parser.CompilationUnit;
			if (parsedCU == null) return null;
			foreach (var node in parsedCU.Children) {
				// fix StartLocation / EndLocation
				node.AcceptVisitor(new ICSharpCode.NRefactory.Visitors.SetRegionInclusionVisitor(), null);
			}
			parsedSpecials = parser.Lexer.SpecialTracker.CurrentSpecials;
			return parsedCU;
		}
		
		/// <summary>
		/// Swaps 2 ranges of text in a document.
		/// </summary>
		static void SwapText(IDocument document, Location start1, Location end1, Location start2, Location end2)
		{
			if (start1 > start2) {
				Location sw;
				sw = start1; start1 = start2; start2 = sw;
				sw = end1; end1 = end2; end2 = sw;
			}
			if (end1 >= start2)
				throw new InvalidOperationException("Cannot swap overlaping segments");
			int offset1 = document.PositionToOffset(start1);
			int len1 = document.PositionToOffset(end1) - offset1;
			int offset2 = document.PositionToOffset(start2);
			int len2 = document.PositionToOffset(end2) - offset2;
			
			string text1 = document.GetText(offset1, len1);
			string text2 = document.GetText(offset2, len2);
			
			using (var undoGroup = document.OpenUndoGroup()) {
				document.Replace(offset2, len2, text1);
				document.Replace(offset1, len1, text2);
			}
		}
		
		static void SelectText(Selection selection, ITextEditor editor)
		{
			int startOffset, endOffset;
			try {
				startOffset = editor.Document.PositionToOffset(selection.Start);
				endOffset = editor.Document.PositionToOffset(selection.End);
			} catch (ArgumentOutOfRangeException) {
				return;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
	}
}
