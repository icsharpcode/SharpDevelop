// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using Ast = ICSharpCode.NRefactory.Ast;

namespace CSharpBinding
{
	/// <summary>
	/// Description of ExtendSelection.
	/// </summary>
	public class CodeManipulation : IDisposable
	{
		enum MoveStatementDirection { Up, Down };
		
		ITextEditor editor;
		
		public static readonly RoutedCommand ExtendSelectionCommand = new RoutedCommand(
			"ExtendSelection", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control) }
		);
		
		public static readonly RoutedCommand ShrinkSelectionCommand = new RoutedCommand(
			"ShrinkSelection", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Shift) }
		);
		
		public static readonly RoutedCommand MoveStatementUpCommand = new RoutedCommand(
			"MoveStatementUp", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.Up, ModifierKeys.Alt) }
		);
		
		public static readonly RoutedCommand MoveStatementDownCommand = new RoutedCommand(
			"MoveStatementDown", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.Down, ModifierKeys.Alt) }
		);
		
		IEnumerable<CommandBinding> bindings;
		
		public CodeManipulation(ITextEditor editor)
		{
			this.editor = editor;
			this.editor.SelectionChanged += CodeManipulationSelectionChanged;
			
			TextArea area = (TextArea)editor.GetService(typeof(TextArea));
			
			bindings = new List<CommandBinding> {
				new CommandBinding(ExtendSelectionCommand, ExtendSelectionExecuted),
				new CommandBinding(ShrinkSelectionCommand, ShrinkSelectionExecuted),
				new CommandBinding(MoveStatementUpCommand, MoveStatementUpExecuted),
				new CommandBinding(MoveStatementDownCommand, MoveStatementDownExecuted)
			};
			
			area.DefaultInputHandler.CommandBindings.AddRange(bindings);
		}
		
		bool internalSelectionChange;
		
		void ExtendSelectionExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			internalSelectionChange = true;
			try {
				ExtendSelection();
			} finally {
				internalSelectionChange = false;
			}
		}
		
		void ShrinkSelectionExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			internalSelectionChange = true;
			try {
				ShrinkSelection();
			} finally {
				internalSelectionChange = false;
			}
		}
		
		void MoveStatementUpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			MoveStatementUp();
		}
		
		void MoveStatementDownExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			MoveStatementDown();
		}
		
		readonly Stack<ISegment> previousSelections = new Stack<ISegment>();

		class Selection
		{
			public Location Start { get; set; }
			public Location End { get; set; }
		}
		
		void CodeManipulationSelectionChanged(object sender, EventArgs e)
		{
			if (!internalSelectionChange)
				previousSelections.Clear();
		}
		
		public void Dispose()
		{
			this.editor.SelectionChanged -= CodeManipulationSelectionChanged;
			TextArea area = (TextArea)editor.GetService(typeof(TextArea));
			
			foreach (var b in bindings)
				area.DefaultInputHandler.CommandBindings.Remove(b);
		}
		
		public void MoveStatementUp()
		{
			MoveStatement(editor, MoveStatementDirection.Up);
			editor.ClearSelection();
		}
		
		public void MoveStatementDown()
		{
			MoveStatement(editor, MoveStatementDirection.Down);
			editor.ClearSelection();
		}
		
		// move selection - find outermost node in selection, swap selection with closest child of its parent to the selection
		void MoveStatement(ITextEditor editor, MoveStatementDirection direction)
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
				// look where current statement ended up because it is hard to calculate it correctly
				int currentMovedOffset = editor.Document.Text.IndexOf(currentNodeText, editor.Document.PositionToOffset(upperLocation));
				editor.Caret.Offset = currentMovedOffset;
			}
		}
		
		Selection TryExtendSelectionToComments(IDocument document, Selection selection, IList<ISpecial> commentsBlankLines)
		{
			var extendedToComments = ExtendSelectionToComments(document, selection, commentsBlankLines);
			if (extendedToComments != null)
				return extendedToComments;
			return selection;
		}
		
		public void ExtendSelection()
		{
			INode selectedNode = null;
			IList<ISpecial> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			ISegment oldSelection = new TextSegment { StartOffset = editor.SelectionStart, Length = editor.SelectionLength };
			Selection extendedSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out selectedNode, new Type[] { typeof(INode) });	// any node type
			
			SelectText(extendedSelection, editor);
			
			if (previousSelections.Count == 0 || !(previousSelections.Peek().Offset == oldSelection.Offset && previousSelections.Peek().EndOffset == oldSelection.EndOffset)) {
				previousSelections.Push(oldSelection);
				LoggingService.Debug("pushed: " + oldSelection);
			} else {
				LoggingService.Debug("not accepted: " + oldSelection);
			}
		}
		
		public void ShrinkSelection()
		{
			if (previousSelections.Count < 1)
				return;
			var selection = previousSelections.Pop();
			editor.Select(selection.Offset, selection.Length);
			LoggingService.Debug("popped: " + selection);
		}
		
		// could work to extend selection to set of adjacent statements separated by blank lines
		Selection ExtendSelection(ITextEditor editor, CompilationUnit parsedCU, IList<ISpecial> commentsBlankLines, out INode selectedResultNode, Type[] interestingNodeTypes)
		{
			selectedResultNode = null;
			
			var selectionStart = editor.Document.OffsetToPosition(editor.SelectionStart);
			var selectionEnd = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			Ast.INode currentNode = parsedCU.Children.Select(n => EditorContext.FindInnermostNodeContainingSelection(n, selectionStart, selectionEnd)).FirstOrDefault(n => n != null);
			if (currentNode == null) return null;
			if (!IsNodeTypeInteresting(currentNode, interestingNodeTypes)) {
				// ignore uninteresting nodes in the AST
				currentNode = GetInterestingParent(currentNode, interestingNodeTypes);
			}
			if (currentNode == null) return null;
			selectedResultNode = currentNode;

			// whole node already selected -> expand selection
			if (currentNode.StartLocation == selectionStart && currentNode.EndLocation == selectionEnd) {
				
				bool extendToComments = false;
				if (IsNodeTypeInteresting(currentNode, interestingNodeTypes)) {
					// if interesting node already selected, we can try to also add comments
					var selectionExtendedToComments = ExtendSelectionToComments(editor.Document, selectionStart, selectionEnd, commentsBlankLines);
					if (selectionExtendedToComments != null) {
						// Can be extended to comments -> extend
						selectionStart = selectionExtendedToComments.Start;
						selectionEnd = selectionExtendedToComments.End;
						extendToComments = true;
					}
				}
				if (!extendToComments) {
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
					if (false) { // blank line separators - implement later
						
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
		
		Selection ExtendSelectionToComments(IDocument document, Selection selection, IList<ISpecial> commentsBlankLines)
		{
			if (selection == null)
				throw new ArgumentNullException("selection");
			return ExtendSelectionToComments(document, selection.Start, selection.End, commentsBlankLines);
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, or behind selection on the same line, add it to selection.
		/// </summary>
		Selection ExtendSelectionToComments(IDocument document, Location selectionStart, Location selectionEnd, IList<ISpecial> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(s => s is Comment).Cast<Comment>();
			// add "var i = 5; // comments" comments
			Selection extendedSelection = ExtendSelectionToEndOfLineComments(document, selectionStart, selectionEnd, comments);
			if (extendedSelection != null) {
				return extendedSelection;
			} else {
				// if end-line comments already included, add comments on separate lines preceding selection
				return ExtendSelectionToSeparateComments(document, selectionStart, selectionEnd, comments);
			}
		}
		
		/// <summary>
		/// If there is a comment block behind selection on the same line ("var i = 5; // comment"), add it to selection.
		/// </summary>
		Selection ExtendSelectionToEndOfLineComments(IDocument document, Location selectionStart, Location selectionEnd, IEnumerable<Comment> commentsBlankLines)
		{
			var lineComment = commentsBlankLines.FirstOrDefault(c => c.StartPosition.Line == selectionEnd.Line && c.StartPosition >= selectionEnd);
			if (lineComment == null) {
				return null;
			}
			bool isWholeLineSelected = IsWhitespaceBetween(document, new Location(1, selectionStart.Line), selectionStart);
			if (!isWholeLineSelected) {
				// whole line must be selected before we add the comment
				return null;
			}
			// fix the end of comment set to next line incorrectly by the parser
			int fixEndPos = document.PositionToOffset(lineComment.EndPosition) - 1;
			return new Selection { Start = selectionStart, End = document.OffsetToPosition(fixEndPos) };
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, add it to selection.
		/// </summary>
		Selection ExtendSelectionToSeparateComments(IDocument document, Location selectionStart, Location selectionEnd, IEnumerable<Comment> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(c => c.CommentStartsLine).ToList();
			int commentIndex = comments.FindIndex(c => c.EndPosition <= selectionStart && IsWhitespaceBetween(document, c.EndPosition, selectionStart));
			if (commentIndex < 0) {
				return null;
			}
			var extendedSelection = new Selection { Start = selectionStart, End = selectionEnd };
			// start at the selection and keep adding comments upwards as long as they are separated only by whitespace
			while (commentIndex >= 0 && IsWhitespaceBetween(document, comments[commentIndex].EndPosition, extendedSelection.Start)) {
				var comment = comments[commentIndex];
				// Include the "//, /*, ///" since they are not included by the parser
				extendedSelection.Start = ExtendLeft(comment.StartPosition, document, "///", "/*", "//") ;
				commentIndex--;
			}
			return extendedSelection;
		}
		
		/// <summary>
		/// Searches for parent of interesting type. Skips uninteresting parents.
		/// </summary>
		INode GetInterestingParent(INode node, Type[] interestingNodeTypes)
		{
			var parent = node.Parent;
			while(parent != null) {
				if (IsNodeTypeInteresting(parent, interestingNodeTypes))
					return parent;
				parent = parent.Parent;
			}
			return null;
		}
		
		bool IsNodeTypeInteresting(INode node, Type[] interestingNodeTypes)
		{
			Type nodeType = node.GetType();
			return interestingNodeTypes.Any(interestingType => interestingType.IsAssignableFrom(nodeType) || (nodeType == interestingType));
		}
		
		bool SelectionContainsBlankLines(Location selectionStart, Location selectionEnd, List<BlankLine> blankLines)
		{
			return blankLines.Exists(b => b.StartPosition >= selectionStart && b.EndPosition <= selectionEnd);
		}
		
		bool IsWhitespaceBetween(IDocument document, Location startPos, Location endPos)
		{
			int startOffset = document.PositionToOffset(startPos);
			int endOffset = document.PositionToOffset(endPos);
			if (startOffset > endOffset) {
				throw new ArgumentException("Invalid range for (startPos, endPos)");
			}
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
		
		/// <summary>
		/// If the text at startPos is preceded by any of the prefixes, moves the start backwards to include one prefix
		/// (the rightmost one).
		/// </summary>
		Location ExtendLeft(Location startPos, IDocument document, params String[] prefixes)
		{
			int startOffset = document.PositionToOffset(startPos);
			foreach (string prefix in prefixes) {
				if (startOffset < prefix.Length) continue;
				string realPrefix = document.GetText(startOffset - prefix.Length, prefix.Length);
				if (realPrefix == prefix) {
					return document.OffsetToPosition(startOffset - prefix.Length);
				}
			}
			// no prefixes -> do not extend
			return startPos;
		}
		
		// could depend just on IDocument
		CompilationUnit ParseDocument(ITextEditor editor, out IList<ISpecial> parsedSpecials)
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
		void SwapText(IDocument document, Location start1, Location end1, Location start2, Location end2)
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
		
		void SelectText(Selection selection, ITextEditor editor)
		{
			if (selection == null)
				return;
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
