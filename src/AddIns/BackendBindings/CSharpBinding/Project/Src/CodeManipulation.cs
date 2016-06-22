// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

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
			public TextLocation Start { get; set; }
			public TextLocation End { get; set; }
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
		
		void MoveStatement(ITextEditor editor, MoveStatementDirection direction)
		{
			IList<AstNode> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null) return;
			
			var selectionStart = editor.Document.GetLocation(editor.SelectionStart);
			var selectionEnd = editor.Document.GetLocation(editor.SelectionStart + editor.SelectionLength);
			
			AstNode currentStatement = parsedCU.GetNodeContaining(selectionStart, selectionEnd);
			if (currentStatement == null)
				return;
			var interestingNodeTypes = new[] {
				typeof(Statement), typeof(EntityDeclaration), typeof(UsingDeclaration),
				typeof(NewLineNode), typeof(Comment), typeof(PreProcessorDirective)
			};
			if (!IsNodeTypeInteresting(currentStatement, interestingNodeTypes)) {
				// Ignore non-interesting nodes in the AST
				currentStatement = GetInterestingParent(currentStatement, interestingNodeTypes);
			}
			if (currentStatement == null)
				return;
			
			List<AstNode> currentStatementSiblings = null;
			if ((currentStatement is BlockStatement) &&
			    ((currentStatement.Parent is Statement) || (currentStatement.Parent is EntityDeclaration)) &&
			    !(currentStatement.Parent is BlockStatement)) {
				// Extend current statement to owner of this block statement
				currentStatement = currentStatement.Parent;
			}
			
			if ((currentStatement is Comment) && ((Comment) currentStatement).IsDocumentation &&
			    (currentStatement.Parent is EntityDeclaration)) {
				// Documentation comments belong to their method declaration
				currentStatement = currentStatement.Parent;
			}
			
			currentStatementSiblings = currentStatement.Parent.Children.Where(
				c => IsNodeTypeInteresting(c, interestingNodeTypes) && !(c is NewLineNode)
			).ToList();
			
			// Collect all statements intersecting with selection lines
			Func<AstNode, bool> selectionExpansionPredicate = node =>
				((node.EndLocation.Line >= selectionStart.Line) && (node.StartLocation.Line <= selectionEnd.Line));
			var selectedStatements = currentStatementSiblings.Where(selectionExpansionPredicate);
			if (!selectedStatements.Any())
				return;
			
			// Find out which nodes we have to swap selected statements with
			AstNode swapSibling = null;
			if (direction == MoveStatementDirection.Up) {
				int indexOfFirstSelectedSibling = currentStatementSiblings.IndexOf(selectedStatements.First());
				swapSibling = (indexOfFirstSelectedSibling > 0) ? currentStatementSiblings[indexOfFirstSelectedSibling - 1] : null;
			} else {
				int indexOfLastSelectedSibling = currentStatementSiblings.IndexOf(selectedStatements.Last());
				swapSibling = (indexOfLastSelectedSibling < currentStatementSiblings.Count - 1) ? currentStatementSiblings[indexOfLastSelectedSibling + 1] : null;
			}
			
			IEnumerable<AstNode> swapSiblings = null;
			if (swapSibling != null) {
				Func<AstNode, bool> swapExpansionPredicate = node =>
					((node.EndLocation.Line >= swapSibling.StartLocation.Line) && (node.StartLocation.Line <= swapSibling.EndLocation.Line));
				swapSiblings = currentStatementSiblings.Where(swapExpansionPredicate);
			}
			
			if (!selectedStatements.Any(node => node is PreProcessorDirective)) {
				// Handling moving into neighbour block statements or moving the line out of them
				if (direction == MoveStatementDirection.Up) {
					if (swapSibling != null) {
						var lastSwapSibling = swapSiblings.Last();
						var innerBlockStatement = GetInnerBlockOfControlNode(lastSwapSibling, direction);
						if (innerBlockStatement != null) {
							// Swap with end brace
							swapSiblings = new[] {
								(AstNode) innerBlockStatement.RBraceToken, lastSwapSibling.Children.Last()
							};
						}
					} else {
						// Check if we are inside of a block statement where we can move the lines out from
						var firstSelectedStatement = selectedStatements.First();
						if ((firstSelectedStatement.Parent is BlockStatement) &&
						    NodeSupportsBlockMovement(firstSelectedStatement.Parent.Parent)) {
							// Our swap sibling is the starting brace of block statement and head of parent statement
							swapSiblings = new[] {
								firstSelectedStatement.Parent.Parent.Children.First(),
								((BlockStatement) firstSelectedStatement.Parent).LBraceToken
							};
						}
					}
				} else {
					if (swapSibling != null) {
						var firstSwapSibling = swapSiblings.First();
						var innerBlockStatement = GetInnerBlockOfControlNode(firstSwapSibling, direction);
						if (innerBlockStatement != null) {
							// Swap with start brace of block statement
							swapSiblings = new[] { firstSwapSibling.Children.First(), innerBlockStatement.LBraceToken };
						}
					} else {
						// Check if we are inside of a block statement where we can move the lines out from
						var lastSelectedStatement = selectedStatements.Last();
						if ((lastSelectedStatement.Parent is BlockStatement) &&
						    NodeSupportsBlockMovement(lastSelectedStatement.Parent.Parent)) {
							// Our swap sibling is the ending brace of block statement (and foot statement, like with do...while loops)
							swapSiblings = new[] {
								((BlockStatement) lastSelectedStatement.Parent).RBraceToken,
								lastSelectedStatement.Parent.Parent.Children.Last()
							};
						}
					}
				}
			}
			
			if (swapSiblings == null)
				return;
			
			// Swap lines of current and neighbour statements
			TextLocation movedTextStart = selectedStatements.First().StartLocation;
			TextLocation movedTextEnd = selectedStatements.Last().EndLocation;
			TextLocation swappedTextStart = swapSiblings.First().StartLocation;
			TextLocation swappedTextEnd = swapSiblings.Last().EndLocation;
			
			string currentNodeText = editor.Document.GetText(movedTextStart, movedTextEnd);
			string swappedNodeText = editor.Document.GetText(swappedTextStart, swappedTextEnd);
			SwapText(editor.Document, movedTextStart, movedTextEnd, swappedTextStart, swappedTextEnd);
			
			// Select moved text
//			editor.Select(editor.Document.GetOffset(swappedTextStart.Line, swappedTextStart.Column), 5);//currentNodeText.Length);
//			SelectText(new Selection {
//			           	Start = new TextLocation(swappedTextStart.Line, 0),
//			           	End = new TextLocation(swappedTextStart.Line + (movedTextEnd.Line - movedTextStart.Line), 200) },
//			           editor);
			
//			currentStatement = parsedCU.GetNodeContaining(selectionStart, selectionEnd);
			
			// Move caret to the start of moved statement
			TextLocation upperLocation = new TextLocation[] { movedTextStart, swappedTextStart }.Min();
			int currentMovedOffset = editor.Document.Text.IndexOf(currentNodeText, editor.Document.GetOffset(upperLocation));
			int swappedMovedOffset = editor.Document.Text.IndexOf(swappedNodeText, editor.Document.GetOffset(upperLocation));
			if (direction == MoveStatementDirection.Up) {
				editor.Caret.Location = upperLocation;
			} else {
				// look where current statement ended up because it is hard to calculate it correctly
				editor.Caret.Offset = currentMovedOffset;
			}
			
			// Correct indentation
			editor.Language.FormattingStrategy.IndentLines(
				editor,
				editor.Document.GetLineForOffset(currentMovedOffset).LineNumber,
				editor.Document.GetLineForOffset(currentMovedOffset + currentNodeText.Length).LineNumber);
			editor.Language.FormattingStrategy.IndentLines(
				editor,
				editor.Document.GetLineForOffset(swappedMovedOffset).LineNumber,
				editor.Document.GetLineForOffset(swappedMovedOffset + swappedNodeText.Length).LineNumber);
		}
		
		bool NodeSupportsBlockMovement(AstNode node)
		{
			var blockNodeTypes = new[] {
				typeof(IfElseStatement),
				typeof(ForStatement),
				typeof(ForeachStatement),
				typeof(WhileStatement),
				typeof(DoWhileStatement),
				typeof(UsingStatement)
			};
			
			return blockNodeTypes.Contains(node.GetType());
		}
		
		BlockStatement GetInnerBlockOfControlNode(AstNode node, MoveStatementDirection direction)
		{
			if (node is IfElseStatement) {
				if (direction == MoveStatementDirection.Up) {
					var ifStatement = (IfElseStatement) node;
					if (ifStatement.FalseStatement.IsNull)
						return ifStatement.TrueStatement as BlockStatement;
					return ifStatement.FalseStatement as BlockStatement;
				} else {
					return ((IfElseStatement) node).TrueStatement as BlockStatement;
				}
			}
			if (node is ForStatement) {
				return ((ForStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is ForeachStatement) {
				return ((ForeachStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is WhileStatement) {
				return ((WhileStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is DoWhileStatement) {
				return ((DoWhileStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is UsingStatement) {
				return ((UsingStatement) node).EmbeddedStatement as BlockStatement;
			}
			
			return null;
		}
		
		Selection TryExtendSelectionToComments(IDocument document, Selection selection, IList<AstNode> commentsBlankLines)
		{
			var extendedToComments = ExtendSelectionToComments(document, selection, commentsBlankLines);
			if (extendedToComments != null)
				return extendedToComments;
			return selection;
		}
		
		public void ExtendSelection()
		{
			AstNode selectedNode = null;
			IList<AstNode> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			ISegment oldSelection = new TextSegment { StartOffset = editor.SelectionStart, Length = editor.SelectionLength };
			Selection extendedSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out selectedNode, new Type[] { typeof(AstNode) });	// any node type
			
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
		Selection ExtendSelection(ITextEditor editor, SyntaxTree parsedCU, IList<AstNode> commentsBlankLines, out AstNode selectedResultNode, Type[] interestingNodeTypes)
		{
			selectedResultNode = null;
			
			var selectionStart = editor.Document.GetLocation(editor.SelectionStart);
			var selectionEnd = editor.Document.GetLocation(editor.SelectionStart + editor.SelectionLength);
			
			AstNode currentNode = parsedCU.GetNodeContaining(selectionStart, selectionEnd);
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
					// var blankLines = commentsBlankLines.Where(s => s is BlankLine).Cast<BlankLine>().ToList();
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
		
		Selection ExtendSelectionToComments(IDocument document, Selection selection, IList<AstNode> commentsBlankLines)
		{
			if (selection == null)
				throw new ArgumentNullException("selection");
			return ExtendSelectionToComments(document, selection.Start, selection.End, commentsBlankLines);
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, or behind selection on the same line, add it to selection.
		/// </summary>
		Selection ExtendSelectionToComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IList<AstNode> commentsBlankLines)
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
		Selection ExtendSelectionToEndOfLineComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IEnumerable<AstNode> commentsBlankLines)
		{
			var lineComment = commentsBlankLines.FirstOrDefault(c => c.StartLocation.Line == selectionEnd.Line && c.StartLocation >= selectionEnd);
			if (lineComment == null) {
				return null;
			}
//			bool isWholeLineSelected = IsWhitespaceBetween(document, new TextLocation(selectionStart.Line, 1), selectionStart);
//			if (!isWholeLineSelected) {
//				// whole line must be selected before we add the comment
//				return null;
//			}
			int endPos = document.GetOffset(lineComment.EndLocation);
			return new Selection { Start = selectionStart, End = document.GetLocation(endPos) };
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, add it to selection.
		/// </summary>
		Selection ExtendSelectionToSeparateComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IEnumerable<AstNode> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(c => (c is Comment) && ((Comment) c).StartsLine).ToList();
			int commentIndex = comments.FindIndex(c => c.EndLocation <= selectionStart && IsWhitespaceBetween(document, c.EndLocation, selectionStart));
			if (commentIndex < 0) {
				return null;
			}
			var extendedSelection = new Selection { Start = selectionStart, End = selectionEnd };
			// start at the selection and keep adding comments upwards as long as they are separated only by whitespace
			while (commentIndex >= 0 && IsWhitespaceBetween(document, comments[commentIndex].EndLocation, extendedSelection.Start)) {
				var comment = comments[commentIndex];
				// Include the "//, /*, ///" since they are not included by the parser
				extendedSelection.Start = ExtendLeft(comment.StartLocation, document, "///", "/*", "//") ;
				commentIndex--;
			}
			return extendedSelection;
		}
		
		/// <summary>
		/// Searches for parent of interesting type. Skips uninteresting parents.
		/// </summary>
		AstNode GetInterestingParent(AstNode node, Type[] interestingNodeTypes)
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
		
		bool SelectionContainsBlankLines(TextLocation selectionStart, TextLocation selectionEnd, List<AstNode> blankLines)
		{
			return blankLines.Exists(b => b.StartLocation >= selectionStart && b.EndLocation <= selectionEnd);
		}
		
		bool IsWhitespaceBetween(IDocument document, TextLocation startPos, TextLocation endPos)
		{
			int startOffset = document.GetOffset(startPos);
			int endOffset = document.GetOffset(endPos);
			if (startOffset > endOffset) {
				throw new ArgumentException("Invalid range for (startPos, endPos)");
			}
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
		
		/// <summary>
		/// If the text at startPos is preceded by any of the prefixes, moves the start backwards to include one prefix
		/// (the rightmost one).
		/// </summary>
		TextLocation ExtendLeft(TextLocation startPos, IDocument document, params String[] prefixes)
		{
			int startOffset = document.GetOffset(startPos);
			foreach (string prefix in prefixes) {
				if (startOffset < prefix.Length) continue;
				string realPrefix = document.GetText(startOffset - prefix.Length, prefix.Length);
				if (realPrefix == prefix) {
					return document.GetLocation(startOffset - prefix.Length);
				}
			}
			// no prefixes -> do not extend
			return startPos;
		}
		
		// could depend just on IDocument
		SyntaxTree ParseDocument(ITextEditor editor, out IList<AstNode> parsedSpecials)
		{
			parsedSpecials = null;
			CompilerSettings compilerSettings = new CompilerSettings();
			var parser = new CSharpParser();
			if (parser == null) return null;
			var syntaxTree = parser.Parse(editor.Document.CreateReader());
			if (syntaxTree == null) return null;
			parsedSpecials = new List<AstNode>(syntaxTree.Descendants.OfType<Comment>());
			return syntaxTree;
		}
		
		/// <summary>
		/// Swaps 2 ranges of text in a document.
		/// </summary>
		void SwapText(IDocument document, TextLocation start1, TextLocation end1, TextLocation start2, TextLocation end2)
		{
			if (start1 > start2) {
				TextLocation sw;
				sw = start1; start1 = start2; start2 = sw;
				sw = end1; end1 = end2; end2 = sw;
			}
			if (end1 >= start2)
				throw new InvalidOperationException("Cannot swap overlaping segments");
			int offset1 = document.GetOffset(start1);
			int len1 = document.GetOffset(end1) - offset1;
			int offset2 = document.GetOffset(start2);
			int len2 = document.GetOffset(end2) - offset2;
			
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
				startOffset = editor.Document.GetOffset(selection.Start);
				endOffset = editor.Document.GetOffset(selection.End);
			} catch (ArgumentOutOfRangeException) {
				return;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
	}
}
